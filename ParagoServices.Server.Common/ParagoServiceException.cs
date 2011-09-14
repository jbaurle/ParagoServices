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
using System.Security.Permissions;
using System.Text;

namespace ParagoServices
{
	[Serializable]
	public class ParagoServiceException : Exception
	{
		public bool HasFaultMessage { get; protected set; }
		public string ServerSource { get; protected set; }
		public string ServerException { get; protected set; }

		public ParagoServiceFault Fault
		{
			get { return new ParagoServiceFault(Message, ServerSource, ServerException); }
		}

		public ParagoServiceException()
		{
		}

		public ParagoServiceException(string message)
			: base(message)
		{
		}

		public ParagoServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ParagoServiceException(string format, params object[] arg)
			: this(string.Format(format, arg))
		{
		}

		internal ParagoServiceException(ParagoServiceFault fault)
			: base(fault.Message)
		{
			HasFaultMessage = true;
			ServerSource = fault.Source;
			ServerException = fault.Exception;
		}

		protected ParagoServiceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if(info == null)
				throw new ArgumentNullException("info");

			ServerSource = info.GetString("ServerSource");
			ServerException = info.GetString("ServerException");

			if(ServerSource.Length == 0)
				ServerSource = null;
			if(ServerException.Length == 0)
				ServerException = null;
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");

			base.GetObjectData(info, context);

			info.AddValue("ServerSource", ServerSource != null ? ServerSource : string.Empty);
			info.AddValue("ServerException", ServerException != null ? ServerException : string.Empty);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			if(!string.IsNullOrEmpty(ServerException))
			{
				sb.AppendFormat("Server exception (Source: {0})", ServerSource ?? "n/a").AppendLine();
				sb.AppendLine(ServerException);
				sb.AppendLine("------------------------");
#if !SILVERLIGHT
				sb.AppendFormat("Client exception (Source: {0})", Source ?? "n/a").AppendLine();
#else
				sb.AppendFormat("Client exception").AppendLine();
#endif
			}

			sb.Append(base.ToString());

			return sb.ToString();
		}
	}
}