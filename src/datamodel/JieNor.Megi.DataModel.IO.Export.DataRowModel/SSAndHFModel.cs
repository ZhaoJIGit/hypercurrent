using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class SSAndHFModel : ExportListBaseModel, ICloneable
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

		[DataMember]
		public decimal EmployeeAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal EmployerAmount
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
