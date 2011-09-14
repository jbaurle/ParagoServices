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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ParagoServices
{
	[DebuggerDisplay("Count = {Count}")]
	[DataContract(Name = "DataCollectionOf{0}", Namespace = ParagoServiceNamespaces.Contract)]
	public class DataCollection<T> : DataObject, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
	{
		List<T> _items;

		[DataMember]
		public List<T> Items
		{
			get { return _items; }
			set { _items = (value == null) ? new List<T>() : value; }
		}

		public T this[int index]
		{
			get { return Items[index]; }
			set
			{
				if(index < 0 || index >= Items.Count)
					throw new ArgumentOutOfRangeException("index");

				SetItem(index, value);
			}
		}

		object IList.this[int index]
		{
			get { return ((IList)Items)[index]; }
			set
			{
				if(index < 0 || index >= Items.Count)
					throw new ArgumentOutOfRangeException("index");

				VerifyValueType(value);

				SetItem(index, (T)value);
			}
		}

		public int Count
		{
			get { return Items.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public DataCollection()
		{
			// NOTE: This contructor is not called by the DataContractSerializer during deserialization.

			Items = new List<T>();
		}

		public DataCollection(IEnumerable<T> collection)
		{
			if(collection == null)
				throw new ArgumentNullException("collection");

			CopyFrom(collection);
		}

		public DataCollection(List<T> list)
		{
			CopyFrom(list);
		}

		public void Add(T item)
		{
			InsertItem(Items.Count, item);
		}

		int IList.Add(object value)
		{
			VerifyValueType(value);

			Add((T)value);
			return Count - 1;
		}

		public void Insert(int index, T item)
		{
			if(index < 0 || index > Items.Count)
				new ArgumentOutOfRangeException("index");

			InsertItem(index, item);
		}

		void IList.Insert(int index, object value)
		{
			VerifyValueType(value);

			Insert(index, (T)value);
		}

		public bool Remove(T item)
		{
			int index = Items.IndexOf(item);

			if(index < 0)
				return false;

			RemoveItem(index);
			return true;
		}

		void IList.Remove(object value)
		{
			if(DataCollection<T>.IsCompatibleObject(value))
				Remove((T)value);
		}

		public void RemoveAt(int index)
		{
			if(index < 0 || index >= Items.Count)
				new ArgumentOutOfRangeException("index");

			RemoveItem(index);
		}

		public void Clear()
		{
			ClearItems();
		}

		public int IndexOf(T item)
		{
			return Items.IndexOf(item);
		}

		int IList.IndexOf(object value)
		{
			if(DataCollection<T>.IsCompatibleObject(value))
				return IndexOf((T)value);

			return -1;
		}

		public bool Contains(T item)
		{
			return Items.Contains(item);
		}

		bool IList.Contains(object value)
		{
			return DataCollection<T>.IsCompatibleObject(value) && Contains((T)value);
		}

		public void CopyFrom(IEnumerable<T> collection)
		{
			if(collection != null)
			{
				using(IEnumerator<T> enumerator = collection.GetEnumerator())
				{
					while(enumerator.MoveNext())
					{
						Items.Add(enumerator.Current);
					}
				}
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Items.CopyTo(array, arrayIndex);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)Items).CopyTo(array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		protected virtual void ClearItems()
		{
			Items.Clear();
		}

		protected virtual void InsertItem(int index, T item)
		{
			Items.Insert(index, item);
		}

		protected virtual void RemoveItem(int index)
		{
			Items.RemoveAt(index);
		}

		protected virtual void SetItem(int index, T item)
		{
			Items[index] = item;
		}

		protected static void VerifyValueType(object value)
		{
			if(!IsCompatibleObject(value))
				throw new ArgumentException("Wrong value type");
		}

		protected static bool IsCompatibleObject(object value)
		{
			return value is T || (value != null && typeof(T).IsValueType);
		}
	}
}