using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class SalaryItemModel : ExportListBaseModel, ICloneable
	{
		[DataMember]
		public int Type
		{
			get;
			set;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DisplayName("Description")]
		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DisplayName("Amount")]
		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCreateDate
		{
			get;
			set;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
