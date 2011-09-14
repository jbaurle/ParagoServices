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

namespace ParagoServices
{
	public sealed class ParagoServiceClient
	{
		ParagoServiceApplicationProxy _proxy;

		public ParagoServiceClient()
			: this(SPServiceContext.Current)
		{
		}

		public ParagoServiceClient(SPServiceContext serviceContext)
		{
			if(serviceContext == null)
				throw new ArgumentNullException("serviceContext");

			_proxy = serviceContext.GetDefaultProxy(typeof(ParagoServiceApplicationProxy)) as ParagoServiceApplicationProxy;

			if(_proxy == null)
				throw new InvalidOperationException("Parago Service Application Proxy not found");
		}

		public ParagoServiceClient(ParagoServiceApplicationProxy proxy)
		{
			if(proxy == null)
				throw new ArgumentNullException("proxy");

			_proxy = proxy;
		}

		#region IParagoService Members

		internal string ReadSetting(string key)
		{
			return _proxy.ReadSetting(key);
		}

		internal void UpdateSetting(string key, string value)
		{
			_proxy.UpdateSetting(key, value);
		}

		public string GetDataSize(DataCollection<String> data)
		{
			return _proxy.GetDataSize(data);
		}

		#endregion
	}
}