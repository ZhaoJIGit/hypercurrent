using JieNor.Megi.DataModel.BD;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVAccountTransactionsExportModel
	{
		[DataMember]
		public List<IVAccountTransactionsModel> AccountTransactionList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVTransferModel> TransferList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVReceiveModel> ReceiveList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVPaymentModel> PaymentList
		{
			get;
			set;
		}

		[DataMember]
		public List<BDBankAccountEditModel> BankAccountList
		{
			get;
			set;
		}
	}
}
