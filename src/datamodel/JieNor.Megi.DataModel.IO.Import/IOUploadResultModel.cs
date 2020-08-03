using JieNor.Megi.EntityModel.Enum;

namespace JieNor.Megi.DataModel.IO.Import
{
	public class IOUploadResultModel
	{
		public string ObjectID
		{
			get;
			set;
		}

		public ImportTypeEnum MType
		{
			get;
			set;
		}

		public bool Success
		{
			get;
			set;
		}

		public string MFileName
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}
	}
}
