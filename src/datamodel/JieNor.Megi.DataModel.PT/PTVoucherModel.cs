using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PT
{
	[DataContract]
	public class PTVoucherModel : BDModel
	{
		[DataMember]
		public bool MIsSys
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsDefault
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAllowCustomize
		{
			get;
			set;
		}

		[DataMember]
		public string MTemplateType
		{
			get;
			set;
		}

		[DataMember]
		public int MPaperType
		{
			get;
			set;
		}

		[DataMember]
		public int MPaperDirection
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsPrintLine
		{
			get;
			set;
		}

		[DataMember]
		public decimal MLeftOffset
		{
			get;
			set;
		}

		[DataMember]
		public decimal MUpOffset
		{
			get;
			set;
		}

		[DataMember]
		public string MPreviewImage
		{
			get;
			set;
		}

		[DataMember]
		public int MEntryCount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRowHeight
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsForeignCurrency
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTopMargin
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBottomMargin
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> TemplateTypeList
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> PaperTypeList
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		public PTVoucherModel()
			: base("t_pt_voucher")
		{
		}

		public PTVoucherModel(string tableName)
			: base(tableName)
		{
		}
	}
}
