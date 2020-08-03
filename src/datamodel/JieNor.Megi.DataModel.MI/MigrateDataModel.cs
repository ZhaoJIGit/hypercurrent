using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI
{
	[DataContract]
	public class MigrateDataModel
	{
		[DataMember]
		public string Token
		{
			get;
			set;
		}

		[DataMember]
		public string MigrationID
		{
			get;
			set;
		}

		[DataMember]
		public MigrateTypeEnum Type
		{
			get;
			set;
		}

		[DataMember]
		public List<MigrateLogBaseModel> LogList
		{
			get;
			set;
		}

		[DataMember]
		public string DataListJson
		{
			get;
			set;
		}
	}
}
