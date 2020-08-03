namespace JieNor.Megi.Tools.Resubmit
{
	public abstract class PageTokenViewBase : IPageTokenView
	{
		public static readonly string HiddenTokenName = "hiddenToken";

		public static readonly string HiddenPageIDName = "hiddenPageID";

		public abstract string GetLastPageToken
		{
			get;
		}

		public abstract string GetLastPageID
		{
			get;
		}

		public abstract bool TokensMatch
		{
			get;
		}

		public abstract string GeneratePageToken(string pageID);
	}
}
