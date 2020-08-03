using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPFapiaoReconcileModel : IComparable
	{
		[DataMember]
		public FPTableViewModel MTable
		{
			get;
			set;
		}

		[DataMember]
		public List<FPFapiaoModel> MFapiaoList
		{
			get;
			set;
		}

		[DataMember]
		public List<FPFapiaoModel> MReconciledFapiaoList
		{
			get;
			set;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			FPFapiaoReconcileModel fPFapiaoReconcileModel = obj as FPFapiaoReconcileModel;
			if ((MFapiaoList.Count > 0 && fPFapiaoReconcileModel.MFapiaoList.Count > 0) || (MFapiaoList.Count == 0 && fPFapiaoReconcileModel.MFapiaoList.Count == 0))
			{
				if (int.Parse(MTable.MNumber) < int.Parse(fPFapiaoReconcileModel.MTable.MNumber))
				{
					return 1;
				}
				if (int.Parse(MTable.MNumber) > int.Parse(fPFapiaoReconcileModel.MTable.MNumber))
				{
					return -1;
				}
				if (int.Parse(MTable.MNumber) == int.Parse(fPFapiaoReconcileModel.MTable.MNumber))
				{
					return 0;
				}
			}
			else
			{
				if (MFapiaoList.Count > 0 && fPFapiaoReconcileModel.MFapiaoList.Count == 0)
				{
					return -1;
				}
				if (MFapiaoList.Count == 0 && fPFapiaoReconcileModel.MFapiaoList.Count > 0)
				{
					return 1;
				}
			}
			return 1;
		}
	}
}
