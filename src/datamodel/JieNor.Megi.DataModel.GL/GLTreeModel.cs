using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[Serializable]
	public class GLTreeModel : IEqualityComparer<GLTreeModel>
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		public string id
		{
			get;
			set;
		}

		[DataMember]
		public string text
		{
			get;
			set;
		}

		[DataMember]
		public string parentId
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		[DataMember]
		public bool MIsNew
		{
			get;
			set;
		}

		[DataMember]
		public string jsonData
		{
			get;
			set;
		}

		[DataMember]
		public List<GLTreeModel> children
		{
			get;
			set;
		}

		public bool Equals(GLTreeModel x, GLTreeModel y)
		{
			if (y == null && x == null)
			{
				return true;
			}
			if (x == y)
			{
				return true;
			}
			return (x.id ?? string.Empty) == (y.id ?? string.Empty) && (x.text ?? string.Empty) == (y.text ?? string.Empty) && (x.parentId ?? string.Empty) == (y.parentId ?? string.Empty);
		}

		public int GetHashCode(GLTreeModel obj)
		{
			int num = (obj.id != null) ? obj.id.GetHashCode() : 0;
			int num2 = (obj.text != null) ? obj.text.GetHashCode() : 0;
			int num3 = (obj.parentId != null) ? obj.parentId.GetHashCode() : 0;
			return num ^ num2 ^ num3;
		}
	}
}
