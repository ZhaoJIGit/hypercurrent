using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import
{
	[DataContract]
	public class IOTemplateConfigModel
	{
		[DataMember]
		public string MFieldName
		{
			get;
			set;
		}

		[DataMember]
		public IOTemplateFieldType MFieldType
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsRequired
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsMainData
		{
			get;
			set;
		}

		[DataMember]
		public Dictionary<string, string> MLangList
		{
			get;
			set;
		}

		[DataMember]
		public int DecimalPlaces
		{
			get;
			set;
		}

		[DataMember]
		public string MComment
		{
			get;
			set;
		}

		public IOTemplateConfigModel(string fieldName, Dictionary<string, string> langList, IOTemplateFieldType fieldType, bool isRequired = false, bool isMainData = false, int decimalPlaces = 2, string comment = null)
		{
			MFieldName = fieldName;
			MIsRequired = isRequired;
			MFieldType = fieldType;
			MLangList = langList;
			MIsMainData = isMainData;
			DecimalPlaces = decimalPlaces;
			MComment = comment;
		}

		public IOTemplateConfigModel(string fieldName, Dictionary<string, string> langList, bool isRequired = false, bool isMainData = false, int decimalPlaces = 2, string comment = null)
		{
			MFieldName = fieldName;
			MIsRequired = isRequired;
			MFieldType = IOTemplateFieldType.Text;
			MLangList = langList;
			MIsMainData = isMainData;
			DecimalPlaces = decimalPlaces;
			MComment = comment;
		}

		public IOTemplateConfigModel(string fieldName, Dictionary<string, string> langList, string comment, bool isRequired = false)
		{
			MFieldName = fieldName;
			MIsRequired = isRequired;
			MFieldType = IOTemplateFieldType.Text;
			MLangList = langList;
			MComment = comment;
		}

		public IOTemplateConfigModel(string fieldName, Dictionary<string, string> langList, IOTemplateFieldType fieldType, string comment, bool isRequired = false)
		{
			MFieldName = fieldName;
			MIsRequired = isRequired;
			MFieldType = fieldType;
			MLangList = langList;
			MComment = comment;
			DecimalPlaces = 2;
		}
	}
}
