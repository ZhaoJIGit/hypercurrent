using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.Context
{
	[DataContract]
	public class ParamBase
	{
		[DataMember]
		public string OrgID
		{
			get;
			set;
		}

		[DataMember]
		public string KeyIDs
		{
			get;
			set;
		}

		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string KeyIDSWithSingleQuote
		{
			get
			{
				string[] array = new string[0];
				if (KeyIDs != null)
				{
					array = KeyIDs.Split(',');
				}
				List<string> list = new List<string>();
				string[] array2 = array;
				foreach (string text in array2)
				{
					list.Add(string.Format("'{0}'", text.Trim().Replace("'", "")));
				}
				return string.Join(",", list);
			}
			set
			{
			}
		}

		[DataMember]
		public string KeyIDSWithNoSingleQuote
		{
			get
			{
				string[] array = new string[0];
				if (KeyIDs != null)
				{
					array = KeyIDs.Split(',');
				}
				List<string> list = new List<string>();
				string[] array2 = array;
				foreach (string text in array2)
				{
					list.Add(text.Trim().Replace("'", ""));
				}
				return string.Join(",", list);
			}
			set
			{
			}
		}

		public List<string> MKeyIDList
		{
			get
			{
				List<string> list = new List<string>();
				if (string.IsNullOrEmpty(KeyIDs))
				{
					return list;
				}
				string[] array = KeyIDs.Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					list.Add(text.Trim().Replace("'", ""));
				}
				return list;
			}
			set
			{
			}
		}

		[DataMember]
		public string MOperationID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsInit
		{
			get;
			set;
		}

		[DataMember]
		public bool IsDelete
		{
			get;
			set;
		}
	}
}
