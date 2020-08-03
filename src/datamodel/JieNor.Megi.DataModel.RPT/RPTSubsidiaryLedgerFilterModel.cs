using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTSubsidiaryLedgerFilterModel : GLReportBaseFilterModel
	{
		[DataMember]
		public string MBaseCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public string OrgIds
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountIDs
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountNames
		{
			get;
			set;
		}

		[DataMember]
		public Dictionary<string, string> AccountIDNameList
		{
			get;
			set;
		}

		[DataMember]
		public string NavReportID
		{
			get;
			set;
		}

		[DataMember]
		public string ApiModule
		{
			get;
			set;
		}

		[DataMember]
		public MContext Context
		{
			get;
			set;
		}
	}
}
