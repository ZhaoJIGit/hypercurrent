using JieNor.Megi.DataModel.BD;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import.Account
{
	public class IOAccountModel
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
		public List<IOAccountModel> children
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
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MMatchNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MNewNumber
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
		public bool MIsCheckForCurrency
		{
			get;
			set;
		}

		[DataMember]
		public IOAccountMatchResultEnum MatchResult
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupNames
		{
			get;
			set;
		}

		[DataMember]
		public List<BDAccountEditModel> MatchResultList
		{
			get;
			set;
		}

		[DataMember]
		public List<BDAccountListModel> SystemAccountList
		{
			get;
			set;
		}

		[DataMember]
		public string Message
		{
			get;
			set;
		}

		[DataMember]
		public bool IsNeedTransfer
		{
			get;
			set;
		}
	}
}
