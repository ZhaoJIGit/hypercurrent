using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO
{
	[DataContract]
	public class IOImportDataModel
	{
		[DataMember]
		public DataTable EffectiveData
		{
			get;
			set;
		}

		[DataMember]
		public string[] TrackItemNameList
		{
			get;
			set;
		}

		[DataMember]
		public ImportTypeEnum TemplateType
		{
			get;
			set;
		}

		[DataMember]
		public DataSet EffectiveDataSet
		{
			get;
			set;
		}

		[DataMember]
		public string SolutionID
		{
			get;
			set;
		}

		[DataMember]
		public string FileName
		{
			get;
			set;
		}

		[DataMember]
		public int FaPiaoType
		{
			get;
			set;
		}

		[DataMember]
		public int Type
		{
			get;
			set;
		}

		[DataMember]
		public int SourceDataFrom
		{
			get;
			set;
		}

		[DataMember]
		public string MSolutionName
		{
			get;
			set;
		}

		[DataMember]
		public int MDataRowIndex
		{
			get;
			set;
		}

		[DataMember]
		public int MHeaderRowIndex
		{
			get;
			set;
		}

		[DataMember]
		public List<IOSolutionConfigModel> MConfig
		{
			get;
			set;
		}

		[DataMember]
		public DataTable SourceData
		{
			get;
			set;
		}

		[DataMember]
		public bool IsSaveData
		{
			get;
			set;
		}
	}
}
