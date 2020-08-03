using JieNor.Megi.Core.Attribute;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVReceiveModel : IVRPBaseModel<IVReceiveEntryModel>
	{
		[DataMember]
		[ApiMember("ReceiptID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MReceiptID
		{
			get
			{
				return base.MID;
			}
			set
			{
				base.MID = value;
			}
		}

		[DataMember]
		public string MBranding
		{
			get;
			set;
		}

		[DataMember]
		[ModelEntry]
		[ApiDetail]
		public List<IVReceiveEntryModel> ReceiveEntry
		{
			get
			{
				return base.MEntryList;
			}
			set
			{
				base.MEntryList = value;
			}
		}

		[DataMember]
		public List<IVReceiveAttachmentModel> ReceiveAttachment
		{
			get;
			set;
		}

		[DataMember]
		public string MContactName
		{
			get;
			set;
		}

		[DataMember]
		public IVReceivePermissionModel MActionPermission
		{
			get;
			set;
		}

		[DataMember]
		public string[] TrackItemNameList
		{
			get;
			set;
		}

		public IVReceiveModel()
			: base("T_IV_Receive")
		{
			ReceiveEntry = new List<IVReceiveEntryModel>();
			ReceiveAttachment = new List<IVReceiveAttachmentModel>();
			base.Verification = new List<IVVerificationListModel>();
			MActionPermission = new IVReceivePermissionModel();
		}
	}
}
