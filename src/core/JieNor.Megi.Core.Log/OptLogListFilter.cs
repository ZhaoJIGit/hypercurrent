using JieNor.Megi.EntityModel.Context;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.Log
{
	[DataContract]
	public class OptLogListFilter : SqlWhere
	{
		[DataMember]
		public string Sort
		{
			get;
			set;
		}

		[DataMember]
		public string Order
		{
			get;
			set;
		}

		[DataMember]
		public MContext MContext
		{
			get;
			set;
		}

		[DataMember]
		public string MBizObject
		{
			get;
			set;
		}

		[DataMember]
		public string MPKID
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFromPrint
		{
			get;
			set;
		}
	}
}
