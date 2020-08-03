using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAccountTypeListModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctGroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MNumber
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
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsDelete
		{
			get;
			set;
		}

		[DataMember]
		public string MCreatorID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCreateDate
		{
			get;
			set;
		}

		[DataMember]
		public string MModifierID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MModifyDate
		{
			get;
			set;
		}
	}
}
