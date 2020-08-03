using JieNor.Megi.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.BD.AccountItem
{
	public class AccountItemTreeModel
	{
		[DataMember]
		public string id
		{
			get;
			set;
		}

		[DataMember]
		public string text
		{
			get;
			set;
		}

		[DataMember]
		public List<AccountItemTreeModel> children
		{
			get;
			set;
		}

		[DataMember]
		public string state
		{
			get;
			set;
		}

		[DataMember]
		public bool IsActive
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountGroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctTypeName
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTableID
		{
			get;
			set;
		}

		[DataMember]
		public string MNumber
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
		public string MTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxTypeName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsExpItem
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsPay
		{
			get;
			set;
		}

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSys
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCheckForCurrency
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCanRelateContact
		{
			get;
			set;
		}

		[DataMember]
		public string MCode
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MFullName
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
		public List<NameValueModel> CheckGroupNameList
		{
			get;
			set;
		}

		[DataMember]
		public bool MCreateInitBill
		{
			get;
			set;
		}

		[DataMember]
		public int Index
		{
			get;
			set;
		}
	}
}
