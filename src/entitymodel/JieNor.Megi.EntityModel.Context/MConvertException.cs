using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Context
{
	[Serializable]
	public class MConvertException : Exception
	{
		[DataMember]
		public string FieldName
		{
			get;
			set;
		}

		[DataMember]
		public string FieldValue
		{
			get;
			set;
		}

		[DataMember]
		public int RowIndex
		{
			get;
			set;
		}

		[DataMember]
		private string _message
		{
			get;
			set;
		}

		public override string Message => FieldName + ":" + FieldValue + ":" + RowIndex + ":" + _message;

		public MConvertException(Exception ex, string fieldName, object fieldValue, int rowIndex = 0)
		{
			FieldName = fieldName;
			FieldValue = fieldValue?.ToString();
			RowIndex = rowIndex;
			_message = ex.Message;
		}
	}
}
