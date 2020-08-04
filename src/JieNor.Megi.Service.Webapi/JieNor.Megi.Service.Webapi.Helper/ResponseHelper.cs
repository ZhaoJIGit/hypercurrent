using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.EntityModel.Enum;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Service.Webapi.Helper
{
	public class ResponseHelper
	{
		public static HttpResponseMessage toJson(object obj, bool success, string message = null, bool encode = true)
		{
			ResponseMessageModel responseMessageModel = new ResponseMessageModel();
			responseMessageModel.Success = success;
			if (string.IsNullOrWhiteSpace(message) && !success)
			{
				responseMessageModel.Message = "没有登录，或者登录失效，请重新登录";
			}
			else
			{
				responseMessageModel.Message = message;
			}
			if (encode)
			{
				responseMessageModel.Result = MText.JsonEncode(obj);
			}
			else
			{
				responseMessageModel.Result = obj;
			}
			string content = new JavaScriptSerializer
			{
				MaxJsonLength = 2147483647
			}.Serialize(responseMessageModel);
			return new HttpResponseMessage
			{
				Content = new StringContent(content, Encoding.GetEncoding("UTF-8"), "application/json")
			};
		}

		public static LoginStateEnum CheckTokenIsValid(string token)
		{
			LoginStateEnum result = LoginStateEnum.Expired;
			if (!string.IsNullOrWhiteSpace(token))
			{
				result = ContextHelper.ValidateAccessToken(null, token, null, null, null);
			}
			return result;
		}
	}
}
