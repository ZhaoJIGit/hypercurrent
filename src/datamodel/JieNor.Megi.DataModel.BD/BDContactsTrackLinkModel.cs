using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsTrackLinkModel : BizDataModel
	{
		[DataMember]
		[ApiMember("TrackID")]
		public string MTrackID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ContactID")]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SalTrackId")]
		public string MSalTrackId
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PurTrackId")]
		public string MPurTrackId
		{
			get;
			set;
		}

		public BDContactsTrackLinkModel()
			: base("T_BD_ContactsTrackLink")
		{
		}
	}
}
