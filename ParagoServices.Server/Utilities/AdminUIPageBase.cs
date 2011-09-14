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
using Microsoft.SharePoint.ApplicationPages;
using Microsoft.SharePoint.Utilities;

namespace ParagoServices.AdminUI
{
	public class AdminUIPageBase : GlobalAdminPageBase
	{
		protected Guid ServiceApplicationID
		{
			get { return (Request.QueryString.Count == 0 || Request.QueryString["ID"] == null) ? Guid.Empty : new Guid(Request.QueryString["ID"].ToString()); }
		}

		protected override bool AccessibleByDelegatedAdminGroup
		{
			get { return AdminUIHelper.HasUserReadPermissionForCurrentPage(ServiceApplicationID); }
		}
	}
}
