//
// Parago Media GmbH & Co. KG, Jürgen Bäurle (http://www.parago.de)
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System.Collections.Generic;
using System.Reflection;

namespace ParagoServices
{
	public static class ParagoServiceSettings
	{
		public static Setting<string> Code = new Setting<string>();

		static Dictionary<string, FieldInfo> _settings;

		static ParagoServiceSettings()
		{
			FieldInfo[] fields = typeof(ParagoServiceSettings).GetFields(BindingFlags.Public | BindingFlags.Static);
			_settings = new Dictionary<string, FieldInfo>();

			if(fields != null)
			{
				foreach(FieldInfo field in fields)
				{
					// Set field name as Key property value of Setting object instance
					PropertyInfo property = field.FieldType.GetProperty("Key");
					property.SetValue(field.GetValue((object)null), field.Name, null);

					if(field.FieldType.Name == typeof(Setting<>).Name && !_settings.ContainsKey(field.Name))
						_settings.Add(field.Name, field);
				}
			}
		}

		public static bool ValidateKey(string key)
		{
			return !string.IsNullOrEmpty(key) && key.Trim().Length > 0 && _settings.ContainsKey(key);
		}

		public static bool ValidateKeyValue(string key, string value)
		{
			if(ValidateKey(key) && _settings.ContainsKey(key))
			{
				FieldInfo field = _settings[key];

				// Check if setting type is correct
				if((bool)field.FieldType.InvokeMember("Validate", BindingFlags.InvokeMethod, null, field.GetValue((object)null), new object[] { value }))
					return true;
			}

			return false;
		}

		public static bool ValidateKeyValue(string key, bool value)
		{
			return ValidateKeyValue(key, value.ToString());
		}

		public static bool ValidateKeyValue(string key, int value)
		{
			return ValidateKeyValue(key, value.ToString());
		}
	}
}
