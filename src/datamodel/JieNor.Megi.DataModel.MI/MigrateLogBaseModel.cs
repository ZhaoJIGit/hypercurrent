using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI
{
	[DataContract]
	public class MigrateLogBaseModel : BDModel
	{
		[DataMember]
		public string MMigrationID
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceID
		{
			get;
			set;
		}

		[DataMember]
		public string MSourcePKFields
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceCode
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceName
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiID
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiCode
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiName
		{
			get;
			set;
		}

		public MigrateLogBaseModel()
			: base("t_mi_log")
		{
		}
	}
}
