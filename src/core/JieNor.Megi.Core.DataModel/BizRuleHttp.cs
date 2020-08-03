using Fasterflect;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleHttp : BizRuleBase
	{
		[DataMember]
		public override string RuleName
		{
			get
			{
				return "网址合法性检查";
			}
		}

		public BizRuleHttp(string propertyName, string properDesc)
			: base(propertyName, properDesc)
		{
		}

		public override void Verification(MContext ctx)
		{
			if (base.BizData != null && !string.IsNullOrWhiteSpace(base.PropertyName))
			{
				PropertyInfo property = base.BizData.GetType().GetProperty(base.PropertyName);
				if (!(property.PropertyType != typeof(string)))
				{
					object propertyValue = base.BizData.GetPropertyValue(base.PropertyName);
					if (propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString()))
					{
						string text = propertyValue.ToString();
						if (!Regex.IsMatch(text, "^[a-zA-Z]+://(\\w+(-\\w+)*)(\\.(\\w+(-\\w+)*))*(\\?\\S*)?$"))
						{
							string message = string.Format("{0}：输入的Url地址不合法，请重新输入", base.ProperDesc);
							base.SetMessageInfor(message, AlertEnum.Error);
						}
						else
						{
							if (!text.Contains("http://") && !text.Contains("https://"))
							{
								text = "http://" + text;
							}
							try
							{
								HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
								httpWebRequest.Method = "HEAD";
								httpWebRequest.Timeout = 10000;
								HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
								if (httpWebResponse.StatusCode != HttpStatusCode.OK)
								{
									string message2 = string.Format("{0}：输入的Url地址不能访问，请注意地址是否正确", base.ProperDesc);
									base.SetMessageInfor(message2, AlertEnum.Error);
								}
							}
							catch
							{
								string message3 = string.Format("{0}：输入的Url地址不能访问，请注意地址是否正确", base.ProperDesc);
								base.SetMessageInfor(message3, AlertEnum.Warning);
							}
						}
					}
				}
			}
		}
	}
}
