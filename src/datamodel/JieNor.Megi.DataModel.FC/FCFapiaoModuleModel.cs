using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FC
{
	[DataContract]
	public class FCFapiaoModuleModel : BizDataModel
	{
		[DataMember]
		public string MFastCode
		{
			get;
			set;
		}

		[DataMember]
		public string MLCID
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemIDName
		{
			get;
			set;
		}

		[DataMember]
		public bool MNewItem
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
		public string MDescription
		{
			get;
			set;
		}

		[DataMember]
		public string MExplanation
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5Name
		{
			get;
			set;
		}

		[DataMember]
		public bool MNewTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public bool MNewTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public bool MNewTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public bool MNewTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public bool MNewTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public string MDebitAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MDebitAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAccountName
		{
			get;
			set;
		}

		public FCFapiaoModuleModel()
			: base("T_FC_FapiaoModule")
		{
		}

		public FCFapiaoModuleModel(string tableName)
			: base(tableName)
		{
		}
	}
}
