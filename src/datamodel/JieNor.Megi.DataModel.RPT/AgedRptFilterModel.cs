using JieNor.Megi.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	[KnownType(typeof(AgedByField))]
	[KnownType(typeof(AgedShowType))]
	[KnownType(typeof(RPTAgedRptFilterEnum))]
	[KnownType(typeof(List<string>))]
	public class AgedRptFilterModel : ReportFilterBase
	{
		public DateTime MEndDate
		{
			get
			{
				//DateTime mEndDateExt = MEndDateExt;
				//if (false)
				//{
				//	MEndDateExt = DateTime.Now;
				//}
				DateTime dateTime = MEndDateExt;
				DateTime dateTime2 = dateTime.AddMonths(1);
				dateTime = Convert.ToDateTime(dateTime2.Year + "-" + dateTime2.Month + "-01");
				return dateTime.AddDays(-1.0);
			}
			set
			{
			}
		}

		[DataMember]
		public DateTime MEndDateExt
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MContactIDS
		{
			get;
			set;
		}

		[DataMember]
		public AgedByField AgedByField
		{
			get;
			set;
		}

		[DataMember]
		public bool ShowInvoice
		{
			get;
			set;
		}

		[DataMember]
		public AgedShowType AgedShowType
		{
			get;
			set;
		}

		public DateTime MEndDate1
		{
			get
			{
				return MEndDate.AddMonths(-1);
			}
			set
			{
			}
		}

		public DateTime MEndDate2
		{
			get
			{
				return MEndDate.AddMonths(-2);
			}
			set
			{
			}
		}

		public DateTime MEndDate3
		{
			get
			{
				return MEndDate.AddMonths(-3);
			}
			set
			{
			}
		}

		public DateTime MEndOldDate
		{
			get
			{
				return MEndDate.AddMonths(-4);
			}
			set
			{
			}
		}

		[DataMember]
		public RPTAgedRptFilterEnum AgedType
		{
			get;
			set;
		}

		public AgedRptFilterModel()
		{
			MContactIDS = new List<string>();
			MEndDateExt = DateTime.Now;
		}
	}
}
