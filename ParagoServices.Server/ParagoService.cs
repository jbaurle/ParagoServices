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
	[Guid("A1CE4CEB-08E1-4140-990E-AF65660D175F")]
	public sealed class ParagoService : SPIisWebService, IServiceAdministration
	{
		public static ParagoService Local
		{
			get { return SPFarm.Local.Services.GetValue<ParagoService>(DefaultName); }
		}

		public static string DefaultName
		{
			get { return "Parago Service"; }
		}

		public override string TypeName
		{
			get { return DefaultName; }
		}

		public ParagoService()
		{
		}

		public ParagoService(SPFarm farm)
			: this(DefaultName, farm)
		{
		}

		public ParagoService(string name, SPFarm farm)
			: base(farm)
		{
			// NOTE: This constructor is required if this service is installed by the command
			// psconfig -cmd services -install.

			Name = name;
		}

		// NOTE: Implement IServiceAdministration to participate in eval-mode setup, the 
		// farm configuration wizard, and the Service Applications UI in the central 
		// administration web site.
		#region IServiceAdministration Members

		public Type[] GetApplicationTypes()
		{
			return new Type[] { typeof(ParagoServiceApplication) };
		}

		public SPPersistedTypeDescription GetApplicationTypeDescription(Type serviceApplicationType)
		{
			if(serviceApplicationType != typeof(ParagoServiceApplication))
				throw new NotSupportedException();

			return new SPPersistedTypeDescription("Parago Service Application", "A sample service application.");
		}

		public override SPAdministrationLink GetCreateApplicationLink(Type serviceApplicationType)
		{
			// NOTE: Since there can be only one instance of this service, and there is only one 
			// application type, the target page does not require any querystring parameters.
			return new SPAdministrationLink("/_admin/ParagoServices/CreateServiceApplication.aspx");
		}

		public SPServiceApplication CreateApplication(string name, Type serviceApplicationType, SPServiceProvisioningContext provisioningContext)
		{
			if(provisioningContext == null)
				throw new ArgumentNullException("provisioningContext");

			if(serviceApplicationType != typeof(ParagoServiceApplication))
				throw new NotSupportedException();

			// Ensure this is re-entrant; check if an application already exists
			ParagoServiceApplication serviceApplication = Farm.GetObject(name, Id, serviceApplicationType) as ParagoServiceApplication;

			if(serviceApplication == null)
				serviceApplication = ParagoServiceApplication.Create(name, this, provisioningContext.IisWebServiceApplicationPool);

			return serviceApplication;
		}

		public SPServiceApplicationProxy CreateProxy(string name, SPServiceApplication serviceApplication, SPServiceProvisioningContext provisioningContext)
		{
			return CreateProxy(name, true, serviceApplication, provisioningContext);
		}

		public SPServiceApplicationProxy CreateProxy(string name, bool checkIfExist, SPServiceApplication serviceApplication, SPServiceProvisioningContext provisioningContext)
		{
			if(serviceApplication == null)
				throw new ArgumentNullException("serviceApplication");

			Type serviceApplicationType = serviceApplication.GetType();

			if(serviceApplicationType == null || serviceApplicationType != typeof(ParagoServiceApplication))
				throw new NotSupportedException();

			ParagoServiceProxy serviceProxy = (ParagoServiceProxy)Farm.GetObject(ParagoServiceProxy.DefaultName, Farm.Id, typeof(ParagoServiceProxy));

			if(serviceProxy == null)
			{
				serviceProxy = new ParagoServiceProxy(Farm);
				serviceProxy.Update(true);
				serviceProxy.Provision();
			}

			if(checkIfExist)
			{
				ParagoServiceApplicationProxy serviceApplicationProxy = serviceProxy.ApplicationProxies.GetValue<ParagoServiceApplicationProxy>(name);

				if(serviceApplicationProxy != null)
					return serviceApplicationProxy;
			}

			return new ParagoServiceApplicationProxy(name, serviceProxy, ((ParagoServiceApplication)serviceApplication).Uri);
		}

		#endregion
	}
}