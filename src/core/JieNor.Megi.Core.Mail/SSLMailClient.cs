using JieNor.Megi.Common.Logger;
using System;
using System.Threading;
using System.Web.Mail;

namespace JieNor.Megi.Core.Mail
{
	internal class SSLMailClient : MailClientBase, IMail
	{
		internal SSLMailClient(MailConfig config, MailModel model)
			: base(config, model)
		{
		}

		public void Send()
		{
			if (base._mailModel.ToAddressList.Count != 0)
			{
				try
				{
					Thread thread = new Thread(SendMail);
					thread.Start();
				}
				catch (Exception ex)
				{
					MLogger.Log(ex);
				}
			}
		}

		private void SendMail()
		{
			try
			{
				foreach (string toAddress in base._mailModel.ToAddressList)
				{
					MailMessage mailMessage = new MailMessage();
					mailMessage.To = toAddress;
					mailMessage.From = base._mailCfg.UserName;
					mailMessage.Subject = base._mailModel.Subject;
					mailMessage.BodyFormat = MailFormat.Html;
					mailMessage.Body = base._mailModel.Body;
					if (base._mailModel.AttachmentList != null && base._mailModel.AttachmentList.Count > 0)
					{
						foreach (string attachment in base._mailModel.AttachmentList)
						{
							if (!string.IsNullOrEmpty(attachment) && attachment.Trim().Length != 0)
							{
								mailMessage.Attachments.Add(new MailAttachment(attachment));
							}
						}
					}
					mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
					mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", base._mailCfg.UserName);
					mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", base._mailCfg.Password);
					mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", base._mailCfg.Port);
					mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", "true");
					mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", base._mailCfg.Smtp);
					SmtpMail.SmtpServer = base._mailCfg.Smtp;
					SmtpMail.Send(mailMessage);
				}
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
		}
	}
}
