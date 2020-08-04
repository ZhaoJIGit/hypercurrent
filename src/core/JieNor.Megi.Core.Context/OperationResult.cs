using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace JieNor.Megi.Core.Context
{
	[DataContract]
	public class OperationResult
	{
		private int haveError = 0;

		private int haveDanger = 0;

		[DataMember]
		private int haveWarning = 0;

		[DataMember]
		private string objectId = string.Empty;

		private string message = string.Empty;

		private string allMessage = string.Empty;

		[DataMember]
		private string _errorMessageDetail = string.Empty;

		[DataMember]
		public List<string> SuccessModelID = new List<string>();

		[DataMember]
		public List<string> FailModelID = new List<string>();

		[DataMember]
		public List<BizVerificationInfor> VerificationInfor = new List<BizVerificationInfor>();

		public List<BaseModel> FailModel = new List<BaseModel>();


		public List<BaseModel> SuccessModel = new List<BaseModel>();

		[DataMember]
		public int _success
		{
			get;
			set;
		}

		[DataMember]
		public bool Success
		{
			get
			{
				if (_success != 0)
				{
					return _success == 1;
				}
				return VerificationInfor == null || !VerificationInfor.Any((BizVerificationInfor f) => f.Level == AlertEnum.Error || f.Level == AlertEnum.Danger);
			}
			set
			{
				_success = (value ? 1 : (-1));
			}
		}

		[DataMember]
		public bool HaveError
		{
			get
			{
				if (haveError == 0)
				{
					haveError = ((VerificationInfor != null && VerificationInfor.Any((BizVerificationInfor f) => f.Level == AlertEnum.Error)) ? 1 : (-1));
				}
				return haveError == 1;
			}
			set
			{
				haveError = (value ? 1 : (-1));
			}
		}

		[DataMember]
		public bool HaveDanger
		{
			get
			{
				if (haveDanger == 0)
				{
					haveDanger = ((VerificationInfor != null && VerificationInfor.Any((BizVerificationInfor f) => f.Level == AlertEnum.Danger)) ? 1 : (-1));
				}
				return haveDanger == 1;
			}
			set
			{
				haveDanger = (value ? 1 : (-1));
			}
		}

		[DataMember]
		public bool HaveWarning
		{
			get
			{
				if (haveWarning == 0)
				{
					haveWarning = ((VerificationInfor != null && VerificationInfor.Any((BizVerificationInfor f) => f.Level == AlertEnum.Warning)) ? 1 : (-1));
				}
				return haveWarning == 1;
			}
			set
			{
				haveWarning = (value ? 1 : (-1));
			}
		}

		[DataMember]
		public string ObjectID
		{
			get
			{
				if (string.IsNullOrEmpty(objectId))
				{
					objectId = ((Success && SuccessModelID != null && SuccessModelID.Count > 0) ? SuccessModelID[0] : ((FailModelID != null && FailModelID.Count > 0) ? FailModelID[0] : string.Empty));
				}
				return objectId;
			}
			set
			{
				objectId = value;
			}
		}

		[DataMember]
		public string Message
		{
			get
			{
				if (string.IsNullOrWhiteSpace(message))
				{
					return ErrorMessageDetail;
				}
				return message;
			}
			set
			{
				message = value;
			}
		}

		[DataMember]
		public string AllVerifMessage
		{
			get
			{
				if (string.IsNullOrWhiteSpace(allMessage))
				{
					string result = string.Empty;
					if (VerificationInfor != null && VerificationInfor.Count > 0)
					{
						result = string.Join(Environment.NewLine, VerificationInfor);
					}
					return result;
				}
				return allMessage;
			}
			set
			{
				allMessage = value;
			}
		}

		[DataMember]
		public string ErrorMessageDetail
		{
			get
			{
				if (Success)
				{
					return string.Empty;
				}
				if (string.IsNullOrEmpty(_errorMessageDetail))
				{
					_errorMessageDetail = BuildErrorMessage();
				}
				return _errorMessageDetail;
			}
			set
			{
				_errorMessageDetail = value;
			}
		}

		[DataMember]
		public string Tag
		{
			get;
			set;
		}

		[DataMember]
		public string Code
		{
			get;
			set;
		}

		public List<CommandInfo> OperationCommands
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MessageList
		{
			get;
			set;
		}

		public OperationResult()
		{
			MessageList = new List<string>();
		}

		private string BuildErrorMessage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<BizVerificationInfor> list = (from f in VerificationInfor
			where f.Level == AlertEnum.Error
			select f).ToList();
			foreach (BizVerificationInfor item in list)
			{
				stringBuilder.AppendLine(item.Message);
			}
			return stringBuilder.ToString();
		}
	}
}
