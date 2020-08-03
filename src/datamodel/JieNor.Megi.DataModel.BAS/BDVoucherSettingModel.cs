using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BDVoucherSettingModel : BDModel
	{
		[DataMember]
		public int MModuleID
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
		public string MID
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
		public bool MStatus
		{
			get;
			set;
		}

		public BDVoucherSettingModel()
			: base("T_BD_VoucherSetting")
		{
		}

		public BDVoucherSettingModel(string tableName)
			: base(tableName)
		{
		}
	}
}
