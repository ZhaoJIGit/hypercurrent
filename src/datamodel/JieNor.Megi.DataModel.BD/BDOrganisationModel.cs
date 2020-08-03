using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDOrganisationModel : BDModel
	{
		[DataMember]
		public string MOrgName
		{
			get;
			set;
		}

		[DataMember]
		public string MLegalTradingName
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgBusiness
		{
			get;
			set;
		}

		[DataMember]
		public int MRegProgress
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MConversionDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MGLConversionDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpiredDate
		{
			get;
			set;
		}

		[DataMember]
		public bool MInitBalanceOver
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
		public string MSystemLanguage
		{
			get;
			set;
		}

		[DataMember]
		public int MVersionID
		{
			get;
			set;
		}

		public BDOrganisationModel()
			: base("t_bas_organisation")
		{
		}
	}
}
