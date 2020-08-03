using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO
{
	[DataContract]
	public class IOSolutionConfigModel : IOModel, ICloneable
	{
		[DataMember]
		public int MTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MConfigID
		{
			get;
			set;
		}

		[DataMember]
		public string MColumnName
		{
			get;
			set;
		}

		[DataMember]
		public string MConfigStandardName
		{
			get;
			set;
		}

		[DataMember]
		public string MConfigName
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
		public bool MIsDataRequired
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsKey
		{
			get;
			set;
		}

		[DataMember]
		public string MExpression
		{
			get;
			set;
		}

		[DataMember]
		public string MDataType
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

		public IOSolutionConfigModel()
			: base("T_IO_SolutionConfig")
		{
		}

		public IOSolutionConfigModel(string tableName)
			: base(tableName)
		{
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
