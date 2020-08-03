using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDDepartmentModel : BDModel
	{
		[DataMember]
		public string id
		{
			get;
			set;
		}

		[DataMember]
		public string name
		{
			get;
			set;
		}

		[DataMember]
		public string _topId
		{
			get;
			set;
		}

		[DataMember]
		public string _parentId
		{
			get;
			set;
		}

		[DataMember]
		public string state
		{
			get;
			set;
		}

		[DataMember]
		public string type
		{
			get;
			set;
		}

		[DataMember]
		public string MDeptAttribute
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		public BDDepartmentModel()
			: base("T_BD_Department")
		{
		}
	}
}
