using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD.Param
{
	[DataContract]
	public class BDAccountQueryParam : SqlWhere
	{
		[DataMember]
		public bool IsBank
		{
			get;
			set;
		}

		[DataMember]
		public string GroupName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsActive
		{
			get;
			set;
		}
	}
}
