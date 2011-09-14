//
// Parago Media GmbH & Co. KG, Jürgen Bäurle (http://www.parago.de)
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace ParagoServices
{
	[Guid("7B75FB12-61F4-446C-BB41-DC04423AA4C0")]
	[IisWebServiceApplicationProxyBackupBehavior]
	public sealed class ParagoServiceApplicationProxy : SPIisWebServiceApplicationProxy
	{
		[Persisted]
		SPServiceLoadBalancer _loadBalancer;

		ChannelFactory<IParagoServiceApplication> _channelFactory;
		object _channelFactoryLock = new object();
		string _endpointConfigurationName;

		public static string DefaultName
		{
			get { return "Parago Service Application Proxy"; }
		}

		public override string TypeName
		{
			get { return DefaultName; }
		}

		public ParagoServiceApplicationProxy()
			: base()
		{
		}

		public ParagoServiceApplicationProxy(string name, ParagoServiceProxy serviceProxy, Uri serviceApplicationAddress)
			: base(name, serviceProxy, serviceApplicationAddress)
		{
			_loadBalancer = new SPRoundRobinServiceLoadBalancer(serviceApplicationAddress);
		}

		public override void Provision()
		{
			Status = SPObjectStatus.Provisioning;
			Update();

			_loadBalancer.Provision();

			base.Provision();

			Status = SPObjectStatus.Online;
			Update();
		}

		public override void Unprovision(bool deleteData)
		{
			Status = SPObjectStatus.Unprovisioning;
			Update();

			_loadBalancer.Unprovision();

			base.Unprovision(deleteData);

			Status = SPObjectStatus.Disabled;
			Update();
		}

		public ParagoServiceClient GetClient()
		{
			return new ParagoServiceClient(this);
		}

		#region Service Operation Execution

		IParagoServiceApplication GetChannel(Uri address, ParagoServiceExecuteOptions options)
		{
			string endpointConfigurationName = GetEndpointConfigurationName(address);

			// Check for a cached channel factory for the endpoint configuration
			if(_channelFactory == null || endpointConfigurationName != _endpointConfigurationName)
			{
				lock(_channelFactoryLock)
				{
					// Double check to be sure the channel factory will not be created twice
					if(_channelFactory == null || endpointConfigurationName != _endpointConfigurationName)
					{
						// NOTE: It is not thread-safe to dispose the channel factory in use. We let the 
						// GC clean up because changing the endpoint configuration is a rare operation.

						_channelFactory = CreateChannelFactory<IParagoServiceApplication>(endpointConfigurationName);
						_endpointConfigurationName = endpointConfigurationName;
					}
				}
			}

			if(options == ParagoServiceExecuteOptions.AsProcess)
				return _channelFactory.CreateChannelAsProcess<IParagoServiceApplication>(new EndpointAddress(address));
			else
				return _channelFactory.CreateChannelActingAsLoggedOnUser<IParagoServiceApplication>(new EndpointAddress(address));
		}

		ChannelFactory<T> CreateChannelFactory<T>(string endpointConfigurationName)
		{
			// Open the Client.config file from the WebClients folder
			string clientConfigurationPath = SPUtility.GetGenericSetupPath(@"WebClients\ParagoServices");
			Configuration clientConfiguration = OpenClientConfiguration(clientConfigurationPath);
			ConfigurationChannelFactory<T> factory = new ConfigurationChannelFactory<T>(endpointConfigurationName, clientConfiguration, null);

			// Configure the channel factory for IDFx claims authentication
			factory.ConfigureCredentials(SPServiceAuthenticationMode.Claims);

			return factory;
		}

		string GetEndpointConfigurationName(Uri address)
		{
			if(address == null)
				throw new ArgumentNullException("address");

			if(address.Scheme == Uri.UriSchemeHttps)
				return "https";
			else if(address.Scheme == Uri.UriSchemeHttp)
				return "http";

			throw new NotSupportedException("Unsupported endpoint address");
		}

		T Execute<T>(string operationName, Func<IParagoServiceApplication, T> operation) where T : class
		{
			if(Status != SPObjectStatus.Online)
				throw new ParagoServiceException("Parago Service Application Proxy is not online");

			T result = null;

			using(new SPMonitoredScope("ParagoServiceApplicationProxy.Execute:" + operationName))
			{
				SPServiceLoadBalancerContext loadBalancerContext = null;

				using(new SPMonitoredScope("LoadBalancerContext:BeginOperation"))
					loadBalancerContext = _loadBalancer.BeginOperation();

				try
				{
					IChannel channel;

					using(new SPMonitoredScope("GetChannel:" + loadBalancerContext.EndpointAddress))
						channel = (IChannel)GetChannel(loadBalancerContext.EndpointAddress, ParagoServiceExecuteOptions.AsLoggedOnUser);

					try
					{
						using(new SPMonitoredScope("ExecuteServiceOperation"))
							result = operation((IParagoServiceApplication)channel);
					}
					finally
					{
						try
						{
							channel.Close();
						}
						catch(CommunicationObjectFaultedException)
						{
							channel.Abort();
							throw;
						}
						catch(TimeoutException)
						{
							channel.Abort();
							throw;
						}
					}
				}
				catch(FaultException<ParagoServiceFault> e)
				{
					throw new ParagoServiceException(e.Detail);
				}
				catch(Exception)
				{
					if(loadBalancerContext != null)
						loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed;

					throw;
				}
				finally
				{
					if(loadBalancerContext != null)
					{
						using(new SPMonitoredScope("LoadBalancerContext:EndOperation"))
							loadBalancerContext.EndOperation();
					}
				}
			}

			return result;
		}

		#endregion

		#region IParagoService Members

		internal string ReadSetting(string key)
		{
			return Execute<string>("ReadSetting", serviceApplication => serviceApplication.ReadSetting(key));
		}

		internal void UpdateSetting(string key, string value)
		{
			Execute<object>("UpdateSetting",
				serviceApplication => {
					serviceApplication.UpdateSetting(key, value);
					return null;
				});
		}

		internal string GetDataSize(DataCollection<String> data)
		{
			return Execute<string>("GetDataSize", serviceApplication => serviceApplication.GetDataSize(data));
		}

		#endregion
	}
}