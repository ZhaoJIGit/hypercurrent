using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import.PA
{
	[DataContract]
	public class ImportSalaryModel
	{
		[DataMember]
		public ImportPaySettingModel PaySetting
		{
			get;
			set;
		}

		[DataMember]
		public List<ImportSalaryListModel> SalaryList
		{
			get;
			set;
		}

		[DataMember]
		public List<FieldMappingModel> FieldMappingList
		{
			get;
			set;
		}

		[DataMember]
		public List<string> DisabledFieldList
		{
			get;
			set;
		}

		[DataMember]
		public List<PAPayItemGroupModel> EditablePresetGroupList
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> MultiLangGroupNameList
		{
			get;
			set;
		}

		[DataMember]
		public int ImportStep
		{
			get;
			set;
		}

		[DataMember]
		public List<BDEmployeesModel> ActiveEmployeeList
		{
			get;
			set;
		}

		[DataMember]
		public List<PAPayItemGroupAmtModel> UserPayItemList
		{
			get;
			set;
		}
	}
}
