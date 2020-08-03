using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.BD.AccountItem
{
	public class AccountInitBalanceTreeModel
	{
		public string id
		{
			get;
			set;
		}

		public string text
		{
			get;
			set;
		}

		public string MNumber
		{
			get;
			set;
		}

		public List<AccountInitBalanceTreeModel> children
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
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDate
		{
			get;
			set;
		}

		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
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
		public string MContactID
		{
			get;
			set;
		}

		public bool IsCanRelateContact
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
		public string MCode
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
		public List<InitBalanceModel> Balances
		{
			get;
			set;
		}
	}
}
