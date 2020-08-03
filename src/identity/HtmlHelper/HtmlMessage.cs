using JieNor.Megi.Identity.AutoManager;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public class HtmlMessage
	{
		public static int GetReceiveMessageCount()
		{
			return MSGMessageManager.GetReceiveMessageCount();
		}
	}
}
