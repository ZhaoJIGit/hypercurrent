using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	public class BDEmployeesListModel : BDEmployeesModel
	{
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MLinkUser
		{
			get;
			set;
		}

		[DataMember]
		public string MPAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MPStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MPRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MRealAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MRealStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MRealRegion
		{
			get;
			set;
		}

		public BDEmployeesListModel()
		{
			base.MultiLanguage = new List<MultiLanguageFieldList>();
		}
	}
}
