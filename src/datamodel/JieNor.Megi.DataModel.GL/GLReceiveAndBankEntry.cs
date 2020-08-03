using JieNor.Megi.DataModel.IV;
using System.Collections.Generic;

namespace JieNor.Megi.DataModel.GL
{
	public class GLReceiveAndBankEntry
	{
		public IVReceiveModel receive
		{
			get;
			set;
		}

		public IVPaymentModel payment
		{
			get;
			set;
		}

		public IVBankBillEntryModel billEntry
		{
			get;
			set;
		}

		public List<IVBankBillEntryModel> billEntrys
		{
			get;
			set;
		}

		public GLReceiveAndBankEntry()
		{
			billEntrys = new List<IVBankBillEntryModel>();
		}
	}
}
