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
using System.Runtime.Serialization;

namespace ParagoServices
{
	[DataContract(Namespace = ParagoServiceNamespaces.Contract)]
	public class ParagoServiceFault
	{
		[DataMember]
		public string Message { get; set; }
		[DataMember]
		public string Source { get; set; }
		[DataMember]
		public string Exception { get; set; }

		public ParagoServiceFault(Exception exception)
		{
			if(exception == null || string.IsNullOrEmpty(exception.Message))
				throw new ArgumentNullException("exception");

			Message = exception.Message;
			Source = exception.Source;
			Exception = exception.ToString();
		}

		public ParagoServiceFault(string message, string source, string exception)
		{
			Message = message;
			Source = source;
			Exception = exception;
		}
	}
}