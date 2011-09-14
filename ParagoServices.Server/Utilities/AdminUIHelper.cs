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
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.AccessControl;

namespace ParagoServices
{
	internal static class AdminUIHelper
	{
		public static ParagoServiceApplication GetServiceApplication(Guid applicationID)
		{
			ParagoService service = ParagoService.Local;

			if(service == null)
				throw new InvalidOperationException("Parago Service is not installed");

			return service.Applications.GetValue<ParagoServiceApplication>(applicationID);
		}

		public static bool HasUserReadPermissionForCurrentPage(ParagoServiceApplication serviceApplication)
		{
			if(serviceApplication == null)
				return false;

			if(!SPFarm.Local.CurrentUserIsAdministrator())
			{
				if((!serviceApplication.CheckCentralAdministrationRights(SPCentralAdministrationRights.Read) && !serviceApplication.CheckCentralAdministrationRights(SPCentralAdministrationRights.Write)) && (!serviceApplication.CheckCentralAdministrationRights(SPCentralAdministrationRights.None | SPCentralAdministrationRights.ChangePermissions) && !serviceApplication.CheckCentralAdministrationRights(~SPCentralAdministrationRights.None)))
					return false;
			}

			return true;
		}

		public static bool HasUserReadPermissionForCurrentPage(Guid serviceApplicationID)
		{
			return AdminUIHelper.HasUserReadPermissionForCurrentPage(GetServiceApplication(serviceApplicationID));
		}
	}
}
