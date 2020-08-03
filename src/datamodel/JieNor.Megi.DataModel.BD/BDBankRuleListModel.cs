using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankRuleListModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MBankName
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrency
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
		public bool MChkAmount
		{
			get;
			set;
		}

		[DataMember]
		public bool MChkPayee
		{
			get;
			set;
		}

		[DataMember]
		public bool MChkRef
		{
			get;
			set;
		}

		[DataMember]
		public bool MChkTransDate
		{
			get;
			set;
		}
	}
}
