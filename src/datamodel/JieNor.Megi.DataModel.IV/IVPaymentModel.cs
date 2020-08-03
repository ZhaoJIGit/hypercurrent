using JieNor.Megi.Core.Attribute;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVPaymentModel : IVRPBaseModel<IVPaymentEntryModel>
	{
		[DataMember]
		[ApiMember("PaymentID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MPaymentID
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
		public string MDepartment
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAdvances
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeID
		{
			get;
			set;
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
		public List<IVPaymentEntryModel> PaymentEntry
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
		public List<IVPaymentAttachmentModel> PaymentAttachment
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
		public IVPaymentPermissionModel MActionPermission
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

		public IVPaymentModel()
			: base("T_IV_Payment")
		{
			PaymentAttachment = new List<IVPaymentAttachmentModel>();
			PaymentEntry = new List<IVPaymentEntryModel>();
			base.Verification = new List<IVVerificationListModel>();
			MActionPermission = new IVPaymentPermissionModel();
		}
	}
}
