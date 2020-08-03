using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	public class FPConfigSettingSaveModel
	{
		[DataMember]
		public FPImportTypeConfigModel SpecialTypeModel
		{
			get;
			set;
		}

		[DataMember]
		public FPImportTypeConfigModel ProfessionalType
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public bool IsInfoAll
		{
			get;
			set;
		}

		[DataMember]
		public bool IsDetailAll
		{
			get;
			set;
		}
	}
}
