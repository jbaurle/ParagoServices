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
using System.Linq;
using Microsoft.SharePoint.Administration;

namespace ParagoServices
{
	internal static class ServiceHelper
	{
		#region Service Application Helper Methods

		public static ParagoServiceApplication GetServiceApplication(string applicationName)
		{
			if(!string.IsNullOrEmpty(applicationName))
			{
				ParagoService service = ParagoService.Local;

				if(service != null)
					return service.Applications.GetValue<ParagoServiceApplication>(applicationName);
			}

			return null;
		}

		#endregion

		#region Service Application Proxy Helper Methods

		public static ParagoServiceApplicationProxy GetServiceApplicationProxy(string applicationName)
		{
			ParagoServiceApplication application = GetServiceApplication(applicationName);

			if(application != null)
			{
				ParagoServiceProxy serviceProxy = ParagoServiceProxy.Local;

				if(serviceProxy != null)
				{
					foreach(ParagoServiceApplicationProxy serviceApplicationProxy in serviceProxy.ApplicationProxies)
					{
						if(serviceApplicationProxy.Name == application.Name)
							return serviceApplicationProxy;
					}
				}
			}

			return null;
		}

		public static ParagoServiceApplicationProxy GetDefaultServiceApplicationProxy()
		{
			SPServiceApplicationProxyGroup proxyGroup = SPServiceApplicationProxyGroup.Default;

			return proxyGroup == null ? null :
					 proxyGroup.DefaultProxies.OfType<ParagoServiceApplicationProxy>().FirstOrDefault();
		}

		public static void SetDefaultServiceApplicationProxy(ParagoServiceApplicationProxy serviceApplicationProxy)
		{
			if(serviceApplicationProxy != null)
			{
				SPServiceApplicationProxyGroup proxyGroup = SPServiceApplicationProxyGroup.Default;

				if(proxyGroup != null)
				{
					// Remove all existing service application proxies available
					foreach(ParagoServiceApplicationProxy proxy in proxyGroup.DefaultProxies.OfType<ParagoServiceApplicationProxy>().ToList())
						proxyGroup.Remove(proxy.Id);

					proxyGroup.Add(serviceApplicationProxy);
					proxyGroup.Update(true);
				}
			}
		}

		#endregion

		#region Service Helper Methods

		public static ParagoService CreateParagoServiceWithServiceInstance()
		{
			SPFarm farm = SPFarm.Local;
			SPServer server = SPServer.Local;

			if(farm == null)
				throw new InvalidOperationException("No farm object available");
			if(server == null)
				throw new InvalidOperationException("No server object available");

			ParagoService service = ParagoService.Local;
			ParagoServiceInstance serviceInstance = null;
			ParagoServiceProxy serviceProxy = null;

			if(service == null)
			{
				// NOTE: There cases when service class got not detected, but is still avialable. 
				// ---
				service = new ParagoService(farm);

				try
				{
					service.Unprovision();
				}
				catch
				{
				}

				try
				{
					service.Delete();
				}
				catch
				{
				}
				// ---

				// NOTE: Must be set new after Delete is called!
				service = new ParagoService(farm);

				try
				{
					service.Update();

					if(service.Status != SPObjectStatus.Online)
						service.Provision();

					serviceInstance = server.ServiceInstances.GetValue<ParagoServiceInstance>(ParagoServiceInstance.DefaultName);

					if(serviceInstance == null)
					{
						serviceInstance = new ParagoServiceInstance(server, service);
						serviceInstance.Update();
					}

					// NOTE: Provisioning the service instance will create the IIS web service, when first service
					// appliation will be created, otherwise it will not work!
					if(serviceInstance.Status != SPObjectStatus.Online)
						serviceInstance.Provision();

					serviceProxy = farm.ServiceProxies.GetValue<ParagoServiceProxy>(ParagoServiceProxy.DefaultName);

					if(serviceProxy == null)
					{
						serviceProxy = new ParagoServiceProxy(farm);
						serviceProxy.Update();
					}

					if(serviceProxy.Status != SPObjectStatus.Online)
						serviceProxy.Provision();
				}
				catch
				{
					try
					{
						if(serviceProxy != null)
						{
							if(serviceProxy.Status == SPObjectStatus.Online)
								serviceProxy.Unprovision();

							serviceProxy.Delete();
						}

						if(serviceInstance != null)
						{
							if(serviceInstance.Status == SPObjectStatus.Online)
								serviceInstance.Unprovision();

							serviceInstance.Delete();
						}

						if(service != null)
						{
							if(service.Status == SPObjectStatus.Online)
								service.Unprovision();

							service.Delete();
						}
					}
					catch { }

					throw;
				}
			}

			return service;
		}

		public static void RemoveParagoServiceAndAllDependentObjects()
		{
			if(SPFarm.Local == null)
				throw new InvalidOperationException("No farm object available");

			ParagoService service = ParagoService.Local;

			if(service == null)
				throw new InvalidOperationException("No service object available");

			foreach(SPServiceProxy serviceProxy in SPFarm.Local.ServiceProxies)
			{
				if(serviceProxy is ParagoServiceProxy)
				{
					foreach(SPServiceApplicationProxy serviceApplicationProxy in serviceProxy.ApplicationProxies)
					{
						if(serviceApplicationProxy is ParagoServiceApplicationProxy)
						{
							if(serviceApplicationProxy.Status == SPObjectStatus.Online)
								serviceApplicationProxy.Unprovision();

							serviceApplicationProxy.Delete();
						}
					}

					if(serviceProxy.Status == SPObjectStatus.Online)
						serviceProxy.Unprovision();

					serviceProxy.Delete();
				}
			}

			foreach(SPServiceInstance serviceInstance in service.Instances)
			{
				if(serviceInstance is ParagoServiceInstance)
				{
					if(serviceInstance.Status == SPObjectStatus.Online)
						serviceInstance.Unprovision();

					serviceInstance.Delete();
				}
			}

			foreach(SPServiceApplication serviceApplication in service.Applications)
			{
				if(serviceApplication is ParagoServiceApplication)
				{
					if(serviceApplication.Status == SPObjectStatus.Online)
						serviceApplication.Unprovision();

					serviceApplication.Delete();
				}
			}

			if(service.Status == SPObjectStatus.Online)
				service.Unprovision();

			service.Delete();
		}

		#endregion
	}
}
