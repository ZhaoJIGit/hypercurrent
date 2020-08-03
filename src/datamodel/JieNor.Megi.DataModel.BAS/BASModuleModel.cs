using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASModuleModel : BDModel
	{
		[DataMember]
		public string MAppID
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

		public string MCode
		{
			get;
			set;
		}

		public string MModuleCode
		{
			get;
			set;
		}

		public string MParentCode
		{
			get;
			set;
		}

		public string MSystemCode
		{
			get;
			set;
		}

		public string MName
		{
			get;
			set;
		}

		public string MUrl
		{
			get;
			set;
		}

		public int MSequence
		{
			get;
			set;
		}

		public int MLevel
		{
			get;
			set;
		}

		public string MPath
		{
			get;
			set;
		}

		public BASModuleModel()
			: base("T_Bas_Module")
		{
		}
	}
}
