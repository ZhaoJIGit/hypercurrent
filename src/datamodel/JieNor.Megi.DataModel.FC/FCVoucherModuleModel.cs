using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FC
{
	[DataContract]
	public class FCVoucherModuleModel : BDModel
	{
		[DataMember]
		public string MDescription
		{
			get;
			set;
		}

		[DataMember]
		public string MCreatorName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsMulti
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebitTotal
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCreditTotal
		{
			get;
			set;
		}

		[DataMember]
		public string MFastCode
		{
			get;
			set;
		}

		[DataMember]
		public string MLCID
		{
			get;
			set;
		}

		public string MFullName
		{
			get
			{
				return MFastCode + "-" + MDescription;
			}
		}

		[DataMember]
		public string ErrorMessage
		{
			get;
			set;
		}

		[DataMember]
		public bool Success
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCoding
		{
			get;
			set;
		}

		[DataMember]
		[ModelEntry]
		[ApiDetail]
		public List<FCVoucherModuleEntryModel> MVoucherModuleEntrys
		{
			get;
			set;
		}

		public FCVoucherModuleModel()
			: base("T_FC_VoucherModule")
		{
		}

		public FCVoucherModuleModel(string tableName)
			: base(tableName)
		{
		}
	}
}
