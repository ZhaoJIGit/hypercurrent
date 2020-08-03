using System.Collections.Generic;

namespace JieNor.Megi.Core.Mail
{
	public class SendMail
	{
		private delegate void SendMultiEMail(List<MailHelper> list);

		private static object thisLock = new object();

		public static void SendSMTPEMail(string strto, string strSubject, string strBody, string fromUserName = "Megi", string filePath = "")
		{
			EMailHelper eMailHelper = new EMailHelper();
			MailModel model = new MailModel
			{
				Subject = strSubject,
				Body = strBody,
				ToAddressList = new List<string>
				{
					strto
				},
				AttachmentList = new List<string>
				{
					filePath
				}
			};
			eMailHelper.Send(model);
		}

		public static void SendSMTPEMail(string strto, string strSubject, string strBody, List<string> files, string fromUserName = "Megi")
		{
			EMailHelper eMailHelper = new EMailHelper();
			MailModel model = new MailModel
			{
				Subject = strSubject,
				Body = strBody,
				ToAddressList = new List<string>
				{
					strto
				},
				AttachmentList = files
			};
			eMailHelper.Send(model);
		}

		public static void SendSMTPEMail(List<string> emailList, string strSubject, string strBody, string fromUserName = "Megi", string filePath = "")
		{
			EMailHelper eMailHelper = new EMailHelper();
			MailModel model = new MailModel
			{
				Subject = strSubject,
				Body = strBody,
				ToAddressList = emailList,
				AttachmentList = new List<string>
				{
					filePath
				}
			};
			eMailHelper.Send(model);
		}

		public static void SendSMTPEMail(List<MailHelper> emailHelperList)
		{
			if (emailHelperList != null && emailHelperList.Count != 0)
			{
				EMailHelper eMailHelper = new EMailHelper();
				foreach (MailHelper emailHelper in emailHelperList)
				{
					eMailHelper.Send(emailHelper.MailModel);
				}
			}
		}

		private static void AsyncSendSMTPEMail(List<MailHelper> emailHelperList)
		{
			EMailHelper eMailHelper = new EMailHelper();
			foreach (MailHelper emailHelper in emailHelperList)
			{
				eMailHelper.Send(emailHelper.MailModel);
			}
		}

		public static MailHelper GetSendMailHelper(string email, string strSubject, string strBody, string fromUserName = "Megi", string filePath = "")
		{
			return GetSendMailHelper(new List<string>
			{
				email
			}, strSubject, strBody, fromUserName, filePath);
		}

		public static MailHelper GetSendMailHelper(List<string> emailList, string strSubject, string strBody, string fromUserName = "Megi", string filePath = "")
		{
			if (emailList == null || emailList.Count == 0)
			{
				return null;
			}
			MailHelper mailHelper = new MailHelper();
			MailModel obj = new MailModel
			{
				Subject = strSubject,
				Body = strBody,
				ToAddressList = emailList,
				AttachmentList = new List<string>
				{
					filePath
				}
			};
			MailModel mailModel2 = mailHelper.MailModel = obj;
			return mailHelper;
		}
	}
}
