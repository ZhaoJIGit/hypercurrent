using System.Web;
using System.Web.Util;

namespace JieNor.Megi.Tools
{
	public class CustomRequestValidator : RequestValidator
	{
		protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
		{
			if (requestValidationSource == RequestValidationSource.Form)
			{
				int num = value.ToLower().IndexOf("<script>");
				if (num != -1)
				{
					validationFailureIndex = num;
					return false;
				}
				validationFailureIndex = 0;
				return true;
			}
			return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
		}
	}
}
