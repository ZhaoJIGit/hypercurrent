using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FA
{
	[DataContract]
	public class FAFixAssetsChangeModel : FAFixAssetsModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MLastChangeID
		{
			get;
			set;
		}

		[DataMember]
		public int MIndex
		{
			get;
			set;
		}

		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
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
		public string MUserName
		{
			get;
			set;
		}

		[DataMember]
		public string MAction
		{
			get;
			set;
		}

		[DataMember]
		public string MNote
		{
			get;
			set;
		}

		public FAFixAssetsChangeModel()
			: base("T_FA_FixAssetsChange")
		{
		}

		public FAFixAssetsChangeModel(string tableName)
			: base(tableName)
		{
		}
	}
}
