using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class BDAccountViewModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public BDCurrrencyDataViewModel MCurrencyDataModel
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueViewModel MCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupViewModel MCheckGroupModel
		{
			get;
			set;
		}
	}
}
