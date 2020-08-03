using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace JieNor.Megi.EntityModel.Context
{
	[Serializable]
	public class MJsonResult : JsonResult
	{
		[DataMember]
		public List<MActionResultCodeEnum> Codes = new List<MActionResultCodeEnum>();

		[DataMember]
		public List<string> Messages = new List<string>();

		[DataMember]
		public bool IsJsonResult = true;

		[DataMember]
		public bool Success
		{
			get
			{
				return Codes == null || Codes.Count == 0 || (Codes.Count == 1 && Codes[0] == MActionResultCodeEnum.Success);
			}
			private set
			{
			}
		}

		[DataMember]
		public new object Data
		{
			get;
			set;
		}

		[DataMember]
		public string hiddenToken
		{
			get;
			set;
		}

		[DataMember]
		public string hiddenPageID
		{
			get;
			set;
		}

		[DataMember]
		public int RowCount
		{
			get;
			set;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			base.ExecuteResult(context);
		}
	}
}
