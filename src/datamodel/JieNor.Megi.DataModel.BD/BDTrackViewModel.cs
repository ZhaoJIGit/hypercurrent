using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDTrackViewModel
	{
		[DataMember]
		public string MPKID
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryPKID
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryName
		{
			get;
			set;
		}
	}
}
