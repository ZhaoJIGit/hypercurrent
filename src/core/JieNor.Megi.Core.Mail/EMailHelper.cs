using JieNor.Megi.Common.Logger;
using System;
using System.Configuration;

namespace JieNor.Megi.Core.Mail
{
	public class EMailHelper
	{
		private static string _mailString = "";

		public EMailHelper()
		{
			if (string.IsNullOrEmpty(_mailString))
			{
				_mailString = ConfigurationManager.AppSettings["MailString"];
			}
		}

		public EMailHelper(string mailString)
		{
			_mailString = mailString;
		}

		public void Send(MailModel model)
		{
			try
			{
				MailConfig mailConfig = GetMailConfig();
				IMail mail = null;
				mail = (IMail)((mailConfig.Port != 465 && mailConfig.Port != 587) ? ((object)new MailClient(mailConfig, model)) : ((object)new SSLMailClient(mailConfig, model)));
				mail.Send();
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
		}

		private MailConfig GetMailConfig()
		{
			MailConfig mailConfig = new MailConfig();
			string[] array = _mailString.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split('=');
				string text2 = array3[0];
				string text3 = array3[1];
				switch (text2)
				{
				case "smtp":
					mailConfig.Smtp = text3;
					break;
				case "uid":
					mailConfig.UserName = text3;
					break;
				case "pwd":
					mailConfig.Password = text3;
					break;
				case "displayName":
					mailConfig.DisplayName = text3;
					break;
				case "port":
					mailConfig.Port = Convert.ToInt32(text3);
					break;
				}
			}
			return mailConfig;
		}
	}
}
