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
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace ParagoServices.AdminUI
{
	public partial class CreateServiceApplicationPage : AdminUIPageBase
	{
		SPIisWebServiceApplicationPool _applicationPool;

		protected DialogMaster DialogMaster
		{
			get { return (DialogMaster)Page.Master; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			DialogMaster.OkButton.Click += OnOkButtonClick;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if(!Page.IsPostBack)
			{
				if(ServiceApplicationID == Guid.Empty)
					DialogMaster.OkButton.Text = "Create";
				else
				{
					ParagoServiceApplication serviceApplication = GetServiceApplication();

					if(!AdminUIHelper.HasUserReadPermissionForCurrentPage(serviceApplication))
						SPUtility.HandleAccessDenied(new UnauthorizedAccessException("You are not authorized to access this page."));

					PopulateSettings(serviceApplication);
				}
			}
		}

		protected void OnApplicationPoolValidate(object source, ServerValidateEventArgs e)
		{
			if(e != null && e.IsValid)
			{
				string uniqueID = DialogMaster.OkButton.UniqueID;
				string eventTarget = Request.Params["__EVENTTARGET"];

				if(!string.IsNullOrEmpty(eventTarget) && eventTarget == uniqueID)
				{
					try
					{
						IisWebServiceApplicationPoolSection section = ApplicationPoolSection as IisWebServiceApplicationPoolSection;
						_applicationPool = (section == null) ? null : section.GetOrCreateApplicationPool();
					}
					catch(Exception ex)
					{
						if(ex is SPDuplicateObjectException)
							ApplicationPoolValidator.ErrorMessage = "Application Pool with the given name already exists";
						else
							ApplicationPoolValidator.ErrorMessage = "Application Pool could not be created";

						e.IsValid = false;
					}
				}
			}
		}

		protected void OnOkButtonClick(object sender, EventArgs e)
		{
			if(Page.IsValid && _applicationPool != null)
			{
				if(ServiceApplicationID == Guid.Empty)
				{
					ParagoServiceApplication serviceApplication = null;
					ParagoServiceApplicationProxy serviceApplicationProxy = null;

					using(SPLongOperation operation = new SPLongOperation(this))
					{
						operation.Begin();

						try
						{
							string name = ServiceApplicationNameTextBox.Text.Trim();

							serviceApplication = ParagoServiceApplication.Create(name, ParagoService.Local, _applicationPool);

							if(serviceApplication == null)
								throw new NullReferenceException("ParagoServiceApplication");

							serviceApplication.UpdateSettingInternal(ParagoServiceSettings.Code.Key, "PARAGO");

							serviceApplication.Update(true);
							serviceApplication.Provision();

							serviceApplicationProxy = (ParagoServiceApplicationProxy)ParagoService.Local.CreateProxy(name, true, serviceApplication, null);

							if(serviceApplicationProxy == null)
								throw new NullReferenceException("ParagoServiceApplicationProxy");

							serviceApplicationProxy.Update(true);
							serviceApplicationProxy.Provision();

							ServiceHelper.SetDefaultServiceApplicationProxy(serviceApplicationProxy);

							serviceApplicationProxy.Update(true);

							SendResponseForPopUI();
						}
						catch(Exception ex)
						{
							if(serviceApplicationProxy != null)
							{
								serviceApplicationProxy.Unprovision();
								serviceApplicationProxy.Delete();
							}

							if(serviceApplication != null)
							{
								serviceApplication.Unprovision();
								serviceApplication.Delete();
							}

							throw new SPException("Failed to create Parago Service Application", ex);
						}
					}
				}
				else
				{
					ParagoServiceApplication serviceApplication = GetServiceApplication();
					UpdateSettings(serviceApplication);
					serviceApplication.Update(true);

					SendResponseForPopUI();
				}
			}
		}

		protected ParagoServiceApplication GetServiceApplication()
		{
			try
			{
				return AdminUIHelper.GetServiceApplication(ServiceApplicationID);
			}
			catch(Exception)
			{
				RedirectToErrorPage("Could not connect to Parago Service Application. Please check the event logs for more information.");
			}

			return null;
		}

		protected void PopulateSettings(ParagoServiceApplication serviceApplication)
		{
			try
			{
				if(serviceApplication == null)
					throw new NullReferenceException("serviceApplication");

				// NOTE: Do not allow to edit service application name, because service application proxy name 
				// is the same and we need to reset the default service application (see below)!
				ServiceApplicationNameTextBox.Text = serviceApplication.Name;
				ServiceApplicationNameTextBox.Enabled = false;

				//CodeTextBox.Text = serviceApplication.GetSettingInternal(ParagoServiceSettings.Code.Key);

				ApplicationPoolSection.SetSelectedApplicationPool(serviceApplication.ApplicationPool);
			}
			catch(Exception)
			{
				RedirectToErrorPage("Unable to populate Parago Service Application settings.");
			}
		}

		protected void UpdateSettings(ParagoServiceApplication serviceApplication)
		{
			try
			{
				if(serviceApplication == null)
					throw new NullReferenceException("serviceApplication");

				//serviceApplication.UpdateSettingInternal(ParagoServiceSettings.Code.Key, CodeTextBox.Text);
			}
			catch(Exception)
			{
				RedirectToErrorPage("Unable to update Parago Service Application settings.");
			}
		}

		void SendResponseForPopUI()
		{
			Context.Response.Write("<script type='text/javascript'>window.frameElement.commitPopup();</script>");
			Context.Response.Flush();
			Context.Response.End();
		}
	}
}
