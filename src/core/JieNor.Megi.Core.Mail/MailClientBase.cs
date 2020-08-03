namespace JieNor.Megi.Core.Mail
{
	internal class MailClientBase
	{
		protected MailModel _mailModel = null;

		protected MailConfig _mailCfg = null;

		internal MailClientBase(MailConfig config, MailModel model)
		{
			_mailCfg = config;
			_mailModel = model;
		}
	}
}
