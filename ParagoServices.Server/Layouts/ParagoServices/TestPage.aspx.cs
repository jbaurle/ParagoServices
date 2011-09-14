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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ParagoServices.Layouts.ParagoServices
{
	public partial class TestPage : LayoutsPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ParagoServiceClient client = new ParagoServiceClient();

			DataCollection<string> data = new DataCollection<string>();
			data.Add("Microsoft");
			data.Add("Oracle");
			data.Add("SAP");
			data.Add("IBM");
			data.Add("Google");

			OutputLabel.Text = client.GetDataSize(data);
		}
	}
}
