using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BDVoucherSettingCategoryModel : BDModel
	{
		[DataMember]
		public int MModuleID
		{
			get;
			set;
		}

		[DataMember]
		public int MColumnID
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
		public int MDC
		{
			get;
			set;
		}

		[DataMember]
		public int MTypeID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCheckBox
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
		public int MControlStatus
		{
			get;
			set;
		}

		[DataMember]
		public List<BDVoucherSettingModel> MSettingList
		{
			get;
			set;
		}

		public BDVoucherSettingCategoryModel()
			: base("T_BD_VoucherSettingCategory")
		{
		}

		public BDVoucherSettingCategoryModel(string tableName)
			: base(tableName)
		{
		}
	}
}
