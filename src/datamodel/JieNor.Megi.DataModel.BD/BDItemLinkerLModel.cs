using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDItemLinkerLModel : BDEntryModel
	{
		[DataMember]
		[Email("Email Address", OperateTime.Save)]
		public string MEmail
		{
			get;
			set;
		}

		[DataMember]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		public BDItemLinkerLModel()
			: base("T_BD_ItemLinker_L")
		{
		}
	}
}
