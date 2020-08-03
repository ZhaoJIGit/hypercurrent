using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASMenuModel : BDEntryModel
	{
		[DataMember]
		public string MAppID
		{
			get;
			set;
		}

		[DataMember]
		public string MModuleID
		{
			get;
			set;
		}

		[DataMember]
		public string MStandardName
		{
			get;
			set;
		}

		[DataMember]
		public int MShowIndex
		{
			get;
			set;
		}

		[DataMember]
		public string MCode
		{
			get;
			set;
		}

		[DataMember]
		public string MModuleCode
		{
			get;
			set;
		}

		[DataMember]
		public string MParentCode
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemCode
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
		public string MUrl
		{
			get;
			set;
		}

		[DataMember]
		public int MSequence
		{
			get;
			set;
		}

		[DataMember]
		public int MLevel
		{
			get;
			set;
		}

		[DataMember]
		public string MPath
		{
			get;
			set;
		}

		public BASMenuModel()
			: base("T_Bas_Menu")
		{
		}
	}
}
