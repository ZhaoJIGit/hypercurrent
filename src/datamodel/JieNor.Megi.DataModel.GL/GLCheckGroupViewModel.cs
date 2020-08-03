using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLCheckGroupViewModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public int MContactID
		{
			get;
			set;
		}

		[DataMember]
		public int MEmployeeID
		{
			get;
			set;
		}

		[DataMember]
		public int MMerItemID
		{
			get;
			set;
		}

		[DataMember]
		public int MExpItemID
		{
			get;
			set;
		}

		[DataMember]
		public int MPaItemID
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem5
		{
			get;
			set;
		}
	}
}
