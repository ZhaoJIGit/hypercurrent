using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDEmailTemplateModel : BDModel
	{
		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSys
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
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
		public string MSubject
		{
			get;
			set;
		}

		[DataMember]
		public string MContent
		{
			get;
			set;
		}

		public BDEmailTemplateModel()
			: base("T_BD_EmailTemplate")
		{
		}

		public BDEmailTemplateModel(string tableName)
			: base(tableName)
		{
		}
	}
}
