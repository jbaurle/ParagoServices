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
using System.Linq;

namespace ParagoServices
{
	public class Setting<T>
	{
		public string Key { get; set; }
		public T DefaultValue { get; set; }

		internal Setting(string key)
			: this(key, default(T))
		{
		}

		public Setting(string key, T defaultValue)
		{
			if(!new string[] { "String", "Boolean", "Int32" }.Contains(typeof(T).Name))
				throw new ArgumentException("Setting type is not supported");

			Key = key;
			DefaultValue = defaultValue;
		}

		internal Setting()
			: this(null, default(T))
		{
		}

		internal Setting(T defaultValue)
			: this(null, defaultValue)
		{
		}

		public bool Validate(string value)
		{
			switch(typeof(T).Name)
			{
				case "String":
					return true;
				case "Boolean":
					bool boolValue;
					return bool.TryParse(value, out boolValue);
				case "Int32":
					int intValue;
					return int.TryParse(value, out intValue);
				default:
					return false;
			}
		}

		public override string ToString()
		{
			return Key ?? base.ToString();
		}
	}
}
