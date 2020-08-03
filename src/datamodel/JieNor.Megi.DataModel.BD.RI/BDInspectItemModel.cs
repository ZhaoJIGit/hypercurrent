using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.RI;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD.RI
{
	public class BDInspectItemModel : BDModel
	{
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public int MIndex
		{
			get;
			set;
		}

		[DataMember]
		public bool MEnable
		{
			get;
			set;
		}

		[DataMember]
		public bool MRequirePass
		{
			get;
			set;
		}

		[DataMember]
		public string MSettingID
		{
			get;
			set;
		}

		public string MSettingParamID
		{
			get;
			set;
		}

		public RICategorySettingParamModel MParameter
		{
			get;
			set;
		}

		public BDInspectItemModel()
			: base("t_ri_category")
		{
		}

		public BDInspectItemModel(string tableName)
			: base(tableName)
		{
		}
	}
}
