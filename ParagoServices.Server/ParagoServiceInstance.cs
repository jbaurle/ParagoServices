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
	[Guid("C1132475-CA50-45b2-8077-79066250C336")]
	public sealed class ParagoServiceInstance : SPIisWebServiceInstance
	{
		public static string DefaultName
		{
			get { return "Parago Service Instance"; }
		}

		public override string DisplayName
		{
			get { return DefaultName; }
		}

		// NOTE: TypeName is returning 'Parago Service' instead of 'Parago Service Instance'. The type name
		// displayed in the list of 'Services on Server' in the CA, not the DisplayName.
		public override string TypeName
		{
			get { return "Parago Service"; }
		}

		public ParagoServiceInstance()
		{
		}

		internal ParagoServiceInstance(SPServer server, ParagoService service)
			: this(DefaultName, server, service)
		{
		}

		internal ParagoServiceInstance(string name, SPServer server, ParagoService service)
			: base(server, service)
		{
			Name = name;
		}
	}
}
