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
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Administration;

namespace ParagoServices
{
	[Guid("A025A738-2C51-4572-BDB7-1775B27B82D1")]
	[SupportedServiceApplication("3B908AAA-BFE8-4B54-8539-43E0D51B1345", "14.0.0.0", typeof(ParagoServiceApplicationProxy))]
	public sealed class ParagoServiceProxy : SPIisWebServiceProxy, IServiceProxyAdministration
	{
		public static ParagoServiceProxy Local
		{
			get { return SPFarm.Local.ServiceProxies.GetValue<ParagoServiceProxy>(DefaultName); }
		}

		public static string DefaultName
		{
			get { return "Parago Service Proxy"; }
		}

		public override string TypeName
		{
			get { return DefaultName; }
		}

		public ParagoServiceProxy()
		{
		}

		public ParagoServiceProxy(SPFarm farm)
			: this(DefaultName, farm)
		{
		}

		public ParagoServiceProxy(string name, SPFarm farm)
			: base(farm)
		{
			// NOTE: This constructor is required if this service proxy is installed by the 
			// command psconfig -cmd services -install.

			Name = name;
		}

		#region IServiceProxyAdministration Members

		// NOTE: The interface IServiceProxyAdministration supports connections to service 
		// applications in remote farms using the "Connect" experience in the ribbon on 
		// the service application management page.

		public SPServiceApplicationProxy CreateProxy(Type serviceApplicationProxyType, string name, Uri serviceApplicationUri, SPServiceProvisioningContext provisioningContext)
		{
			if(serviceApplicationProxyType != typeof(ParagoServiceApplicationProxy))
				throw new NotSupportedException();

			// Create a proxy for the specified (typically remote) service application
			return new ParagoServiceApplicationProxy(name, this, serviceApplicationUri);
		}

		public SPPersistedTypeDescription GetProxyTypeDescription(Type serviceApplicationProxyType)
		{
			return new SPPersistedTypeDescription(DefaultName, "A sample service application.");
		}

		public Type[] GetProxyTypes()
		{
			return new Type[] { typeof(ParagoServiceApplicationProxy) };
		}

		#endregion
	}
}
