using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.REG;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPBaseDataModel
	{
		[DataMember]
		public List<BDContactsModel> MContact
		{
			get;
			set;
		}

		[DataMember]
		public List<BDAccountModel> MAccount
		{
			get;
			set;
		}

		[DataMember]
		public List<BDItemModel> MMerItem
		{
			get;
			set;
		}

		[DataMember]
		public List<FCFapiaoModuleModel> MFastCode
		{
			get;
			set;
		}

		[DataMember]
		public List<REGTaxRateModel> MTaxRate
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckTypeDataModel MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckTypeDataModel MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckTypeDataModel MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckTypeDataModel MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckTypeDataModel MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public FPCodingSettingModel MSetting
		{
			get;
			set;
		}

		[DataMember]
		public List<BDContactsTrackLinkModel> MTrackLink
		{
			get;
			set;
		}

		[DataMember]
		public List<GLVoucherReferenceModel> MExplanation
		{
			get;
			set;
		}
	}
}
