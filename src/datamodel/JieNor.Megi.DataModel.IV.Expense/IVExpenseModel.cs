using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataModel.BD;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Expense
{
	[DataContract]
	public class IVExpenseModel : IVBaseModel<IVExpenseEntryModel>
	{
		[DataMember]
		[ApiMember("ExpenseID", IsPKField = true)]
		public string MExpenseID
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

		[DataMember(Order = 201)]
		[ApiMember("Type")]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("EmployeeID")]
		public string MEmployee
		{
			get;
			set;
		}

		[DataMember]
		public string MDepartment
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("DueDate")]
		public DateTime MDueDate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ExpectedDate")]
		public DateTime MExpectedDate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Status")]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public override string MTaxID
		{
			get
			{
				return "Tax_Inclusive";
			}
			set
			{
				base.MTaxID = "Tax_Inclusive";
			}
		}

		[DataMember]
		public string VerificationInforPurchase
		{
			get;
			set;
		}

		[DataMember]
		[ModelEntry]
		[ApiDetail]
		[ApiMember("ExpenseEntry")]
		public List<IVExpenseEntryModel> ExpenseEntry
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
		public List<IVExpenseAttachmentModel> ExpenseAttachment
		{
			get;
			set;
		}

		[DataMember]
		public IVExpensePermissionModel MActionPermission
		{
			get;
			set;
		}

		[DataMember]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		public int MRowIndex
		{
			get;
			set;
		}

		public List<int> MRowIndexList
		{
			get;
			set;
		}

		[DataMember]
		public string MNextID
		{
			get;
			set;
		}

		[DataMember]
		public List<BDBankBasicModel> MBankList
		{
			get;
			set;
		}

		public string[] TrackItemNameList
		{
			get;
			set;
		}

		public IVExpenseModel()
			: base("T_IV_Expense")
		{
			ExpenseAttachment = new List<IVExpenseAttachmentModel>();
			ExpenseEntry = new List<IVExpenseEntryModel>();
			base.Verification = new List<IVVerificationListModel>();
			MActionPermission = new IVExpensePermissionModel();
			base.MTaxID = "Tax_Inclusive";
		}
	}
}
