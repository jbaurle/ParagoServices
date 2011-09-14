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
using System.ServiceModel;

namespace ParagoServices
{
	internal static class Extensions
	{
		#region Exception Extensions

		public static FaultException<ParagoServiceFault> GetFaultException(this Exception e)
		{
			if(e == null)
				throw new ArgumentNullException("e");

			ParagoServiceFault fault = new ParagoServiceFault(e);

			return new FaultException<ParagoServiceFault>(fault, fault.Message);
		}

		public static Exception CreateFaultException(this Exception e)
		{
			try
			{
				if(!(e is FaultException<ParagoServiceFault>))
				{
					if(OperationContext.Current != null)
						return e.GetFaultException();
				}
			}
			catch { }

			return e;
		}

		#endregion
	}
}
