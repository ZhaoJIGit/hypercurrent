using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI
{
	[DataContract]
	public class MigrateConfigModel : BDModel
	{
		private string _activeLocaleIDS;

		[DataMember]
		public int __active__ = 1;

		[DataMember]
		public string MSource
		{
			get;
			set;
		}

		[DataMember]
		public int MDbType
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceOrgName
		{
			get;
			set;
		}

		[DataMember]
		public string MIP
		{
			get;
			set;
		}

		[DataMember]
		public string MUserName
		{
			get;
			set;
		}

		[DataMember]
		public string MPassword
		{
			get;
			set;
		}

		[DataMember]
		public int MSourceOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MDbName
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAllPeriod
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public int MConfirmedType
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgName
		{
			get;
			set;
		}

		[DataMember]
		public string MModifierName
		{
			get;
			set;
		}

		[DataMember]
		public string MActiveLocaleIDS
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_activeLocaleIDS))
				{
					return string.Empty;
				}
				List<string> list = new List<string>();
				string text = "0x7804";
				string[] array = _activeLocaleIDS.Split(',');
				if (array.Contains(text))
				{
					list.Add(text);
				}
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (text2 != text)
					{
						list.Add(text2);
					}
				}
				return string.Join(",", list);
			}
			set
			{
				_activeLocaleIDS = value;
			}
		}

		[DataMember]
		public string MMegiUserName
		{
			get;
			set;
		}

		[DataMember]
		public string MigratedTypes
		{
			get;
			set;
		}

		[DataMember]
		public int MVersionID
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

		public MigrateConfigModel()
			: base("t_mi_config")
		{
		}
	}
}
