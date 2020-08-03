using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FA
{
	[DataContract]
	public class FAFixAssetsTypeModel : BDModel
	{
		[DataMember]
		public string MFixAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MDepAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MExpAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public string MFixCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MDepCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MExpCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MDepreciationTypeID
		{
			get;
			set;
		}

		[DataMember]
		public bool MDepreciationFromCurrentPeriod
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSys
		{
			get;
			set;
		}

		[DataMember]
		public int MUsefulPeriods
		{
			get;
			set;
		}

		[DataMember]
		public string MUsefulYears
		{
			get
			{
				int num;
				if (MUsefulPeriods % 12 != 0)
				{
					object str;
					if (MUsefulPeriods >= 12)
					{
						num = MUsefulPeriods / 12;
						str = num.ToString() + "+";
					}
					else
					{
						str = "";
					}
					num = MUsefulPeriods % 12;
					return (string)str + num.ToString() + "/12";
				}
				num = MUsefulPeriods / 12;
				return num.ToString();
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MRateOfSalvage
		{
			get;
			set;
		}

		[DataMember]
		public string MRemark
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, false, false)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MFixAccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MDepAccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MExpAccountFullName
		{
			get;
			set;
		}

		public FAFixAssetsTypeModel()
			: base("T_FA_FixAssetsType")
		{
		}

		public FAFixAssetsTypeModel(string tableName)
			: base(tableName)
		{
		}
	}
}
