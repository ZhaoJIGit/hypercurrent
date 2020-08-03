using JieNor.Megi.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLDocVoucherFilterModel : SqlWhere
	{
		[DataMember]
		public string Keyword
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DecimalKeyword
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? DatetimeKeyword
		{
			get;
			set;
		}

		[DataMember]
		public string Number
		{
			get;
			set;
		}

		[DataMember]
		public string Status
		{
			get;
			set;
		}

		[DataMember]
		public int Year
		{
			get;
			set;
		}

		[DataMember]
		public int Period
		{
			get;
			set;
		}

		[DataMember]
		public int Type
		{
			get;
			set;
		}

		[DataMember]
		public string MDocID
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MEntryIDs
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MDocIDs
		{
			get;
			set;
		}

		[DataMember]
		public string MDocVoucherID
		{
			get;
			set;
		}
	}
}
