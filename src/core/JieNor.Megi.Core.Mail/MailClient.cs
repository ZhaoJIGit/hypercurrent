using JieNor.Megi.Common.Logger;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace JieNor.Megi.Core.Mail
{
	internal class MailClient : MailClientBase, IMail
	{
		internal MailClient(MailConfig config, MailModel model)
			: base(config, model)
		{
		}

		public void Send()
		{
			if (base._mailModel.ToAddressList.Count != 0)
			{
				try
				{
					if (base._mailModel.ToAddressList.Count > 1)
					{
						SendMail();
					}
					else
					{
						Thread thread = new Thread(SendMail);
						thread.Start();
					}
				}
				catch (Exception ex)
				{
					MLogger.Log(ex);
				}
			}
		}

		private void SendMail()
		{
			MailMessage mailMessage = new MailMessage();
			try
			{
				mailMessage.Priority = MailPriority.High;
				mailMessage.From = new MailAddress(base._mailCfg.UserName, base._mailCfg.DisplayName);
				foreach (string toAddress in base._mailModel.ToAddressList)
				{
					mailMessage.To.Add(toAddress);
				}
				mailMessage.Subject = base._mailModel.Subject;
				mailMessage.Body = base._mailModel.Body;
				mailMessage.IsBodyHtml = true;
				foreach (string attachment in base._mailModel.AttachmentList)
				{
					if (!string.IsNullOrEmpty(attachment) && attachment.Trim().Length != 0)
					{
						mailMessage.Attachments.Add(new Attachment(attachment));
					}
				}
				SmtpClient smtpClient = new SmtpClient();
				smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
				smtpClient.Port = base._mailCfg.Port;
				smtpClient.Host = base._mailCfg.Smtp;
				smtpClient.EnableSsl = false;
				smtpClient.UseDefaultCredentials = false;
				smtpClient.Credentials = new NetworkCredential(base._mailCfg.UserName, base._mailCfg.Password);
				smtpClient.Send(mailMessage);
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
				if (ex.InnerException != null)
				{
					MLogger.Log(ex.InnerException);
					if (ex.InnerException.InnerException != null)
					{
						MLogger.Log(ex.InnerException.InnerException);
					}
				}
			}
			finally
			{
				foreach (Attachment attachment2 in mailMessage.Attachments)
				{
					attachment2.Dispose();
				}
			}
		}
	}
}
