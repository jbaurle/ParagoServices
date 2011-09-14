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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.AccessControl;
using Microsoft.SharePoint.Utilities;

namespace ParagoServices
{
	[Guid("7435BDAC-44B5-4A44-AA31-8D29117DD329")]
	[IisWebServiceApplicationBackupBehavior]
	public sealed class ParagoServiceApplication : SPIisWebServiceApplication, IParagoServiceApplication
	{
		[Persisted]
		Dictionary<string, string> _settings;

		// NOTE: The type is displayed in the Service Applications UI in the central administration web site; 
		// the application proxy name should display the defined name, otherwise the name would be twice the 
		// same.
		public override string DisplayName
		{
			get { return string.Format("Parago Service {0}", Name); }
		}

		// NOTE: The type is displayed in the Service Applications UI in the central administration web site.
		public override string TypeName
		{
			get { return "Parago Service Application"; }
		}

		public override Guid ApplicationClassId
		{
			get { return new Guid("3B908AAA-BFE8-4B54-8539-43E0D51B1345"); }
		}

		public Dictionary<string, string> Settings
		{
			get { return _settings; }
		}

		public override SPAdministrationLink ManageLink
		{
			get { return new SPAdministrationLink("/_admin/ParagoServices/ManageServiceApplication.aspx?ID=" + Id.ToString()); }
		}

		// NOTE: Settings on this page are intended for farm administrators only. Delegated administrators cannot 
		// access it, ribbon item is disabled!
		public override SPAdministrationLink PropertiesLink
		{
			get { return new SPAdministrationLink("/_admin/ParagoServices/CreateServiceApplication.aspx?ID=" + Id.ToString()); }
		}

		protected override string DefaultEndpointName
		{
			get { return "http"; }
		}

		protected override string InstallPath
		{
			get { return SPUtility.GetGenericSetupPath(@"WebServices\ParagoServices"); }
		}

		protected override string VirtualPath
		{
			get { return "ParagoService.svc"; }
		}

		// NOTE: The returned items will be used for display in the ACL editor UI control and the PowerShell Grant 
		// and Revoke Cmdlets.
		protected override SPNamedCentralAdministrationRights[] AdministrationAccessRights
		{
			get
			{
				return new SPNamedCentralAdministrationRights[] { SPNamedCentralAdministrationRights.FullControl };
			}
		}

		// NOTE: The returned items will be used for display in the ACL editor UI control and the PowerShell Grant 
		// and Revoke Cmdlets.
		protected override SPNamedIisWebServiceApplicationRights[] AccessRights
		{
			get
			{
				return new SPNamedIisWebServiceApplicationRights[] { SPNamedIisWebServiceApplicationRights.FullControl };
			}
		}

		public ParagoServiceApplication()
		{
			_settings = new Dictionary<string, string>();
		}

		ParagoServiceApplication(string name, ParagoService service, SPIisWebServiceApplicationPool applicationPool)
			: base(name, service, applicationPool)
		{
			_settings = new Dictionary<string, string>();
		}

		public override void Provision()
		{
			Status = SPObjectStatus.Provisioning;
			Update();

			base.Provision();

			Status = SPObjectStatus.Online;
			Update();
		}

		public override void Unprovision(bool deleteData)
		{
			Status = SPObjectStatus.Unprovisioning;
			Update();

			base.Unprovision(deleteData);

			Status = SPObjectStatus.Disabled;
			Update();
		}

		public static ParagoServiceApplication Create(string name, ParagoService service, SPIisWebServiceApplicationPool applicationPool)
		{
			if(null == name)
				throw new ArgumentNullException("name");
			if(null == service)
				throw new ArgumentNullException("service");
			if(null == applicationPool)
				throw new ArgumentNullException("applicationPool");

			ParagoServiceApplication serviceApplication = new ParagoServiceApplication(name, service, applicationPool);
			serviceApplication.Update();

			serviceApplication.AddServiceEndpoint("http", SPIisWebServiceBindingType.Http);
			serviceApplication.AddServiceEndpoint("https", SPIisWebServiceBindingType.Https, "secure");

			// NOTE: It seems redundant, but update needs to be called before AND after the endpoint gets provisioned.
			serviceApplication.Update();

			return serviceApplication;
		}

		internal bool CheckCentralAdministrationRights(SPCentralAdministrationRights rights)
		{
			return CheckAdministrationAccess(rights);
		}

		internal string ReadSettingInternal(string key)
		{
			if(!ParagoServiceSettings.ValidateKey(key))
				throw new ParagoServiceException("Setting key is not valid");
			
			return _settings.ContainsKey(key) ? _settings[key] : null;
		}

		internal void UpdateSettingInternal(string key, string value)
		{
			if(!ParagoServiceSettings.ValidateKeyValue(key, value))
				throw new ParagoServiceException("Setting key or value is not valid");

			if(_settings.ContainsKey(key))
				_settings[key] = value;
			else
				_settings.Add(key, value);
		}

		internal void DeleteSettingInternal(string key)
		{
			if(!ParagoServiceSettings.ValidateKey(key))
				throw new ParagoServiceException("Setting key is not valid");

			if(_settings.ContainsKey(key))
				_settings.Remove(key);
		}

		T Execute<T>(string operationName, Func<T> operation) where T : class
		{
			T result = null;

			try
			{
				result = operation();
			}
			catch(Exception e)
			{
				throw e.CreateFaultException();
			}

			return result;
		}

		#region IParagoService Members

		public string ReadSetting(string key)
		{
			return Execute<string>("ReadSetting", () => {

				DemandAdministrationAccess(SPCentralAdministrationRights.FullControl);

				return ReadSettingInternal(key);

			});
		}

		public void UpdateSetting(string key, string value)
		{
			Execute<object>("UpdateSetting", () => {

				DemandAdministrationAccess(SPCentralAdministrationRights.FullControl);

				UpdateSettingInternal(key, value);
				return null;

			});
		}

		public string GetDataSize(DataCollection<String> data)
		{
			return Execute<string>("GetDataSize", () => {

				DemandAdministrationAccess(SPCentralAdministrationRights.FullControl);

				return string.Format("Code: {0}; Count: {1}", ReadSettingInternal(ParagoServiceSettings.Code.Key) ?? "n/a", data != null ? data.Count : 0);

			});
		}

		#endregion
	}
}