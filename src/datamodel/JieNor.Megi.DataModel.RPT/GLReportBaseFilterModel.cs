using JieNor.Megi.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class GLReportBaseFilterModel : ReportFilterBase
	{
		[DataMember]
		public string MStartPeroid
		{
			get;
			set;
		}

		[DataMember]
		public string MEndPeroid
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public string MStartAccountID
		{
			get;
			set;
		}

		[DataMember]
		public int AccountStartIndex
		{
			get;
			set;
		}

		[DataMember]
		public int AccountEndIndex
		{
			get;
			set;
		}

		[DataMember]
		public string MEndAccountID
		{
			get;
			set;
		}

		public List<string> AccountIdList
		{
			get;
			set;
		}

		public List<string> ChildrenAccountList
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public bool MDisplayNoAccurrenceAmount
		{
			get;
			set;
		}

		[DataMember]
		public bool MDisplayZeorEndBalance
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeCheckType
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeUnapprovedVoucher
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeDisabledAccount
		{
			get;
			set;
		}

		[DataMember]
		public int AccountLevel
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> CheckTypeValueList
		{
			get;
			set;
		}

		[DataMember]
		public string CheckGroupValueId
		{
			get;
			set;
		}

		[DataMember]
		public bool OnlyCheckType
		{
			get;
			set;
		}

		[DataMember]
		public bool IsLeafAccount
		{
			get;
			set;
		}

		public GLReportBaseFilterModel Copy(GLReportBaseFilterModel filter)
		{
			GLReportBaseFilterModel gLReportBaseFilterModel = new GLReportBaseFilterModel();
			gLReportBaseFilterModel.MStartPeroid = Convert.ToString(filter.MStartPeroid);
			gLReportBaseFilterModel.MEndPeroid = Convert.ToString(filter.MEndPeroid);
			gLReportBaseFilterModel.MCurrencyID = filter.MCurrencyID;
			gLReportBaseFilterModel.MDisplayNoAccurrenceAmount = filter.MDisplayNoAccurrenceAmount;
			gLReportBaseFilterModel.MDisplayZeorEndBalance = filter.MDisplayZeorEndBalance;
			gLReportBaseFilterModel.MStartAccountID = filter.MStartAccountID;
			gLReportBaseFilterModel.MEndAccountID = filter.MEndAccountID;
			gLReportBaseFilterModel.OnlyCheckType = filter.OnlyCheckType;
			gLReportBaseFilterModel.IsLeafAccount = filter.IsLeafAccount;
			gLReportBaseFilterModel.AccountLevel = filter.AccountLevel;
			gLReportBaseFilterModel.CheckGroupValueId = filter.CheckGroupValueId;
			gLReportBaseFilterModel.ChildrenAccountList = filter.ChildrenAccountList;
			gLReportBaseFilterModel.IncludeCheckType = filter.IncludeCheckType;
			gLReportBaseFilterModel.CheckTypeValueList = filter.CheckTypeValueList;
			gLReportBaseFilterModel.AccountStartIndex = filter.AccountStartIndex;
			gLReportBaseFilterModel.AccountEndIndex = filter.AccountEndIndex;
			gLReportBaseFilterModel.AccountIdList = filter.AccountIdList;
			gLReportBaseFilterModel.MAccountID = filter.MAccountID;
			gLReportBaseFilterModel.IncludeDisabledAccount = filter.IncludeDisabledAccount;
			gLReportBaseFilterModel.IncludeUnapprovedVoucher = filter.IncludeUnapprovedVoucher;
			return gLReportBaseFilterModel;
		}
	}
}
