namespace JieNor.Megi.Tools.Resubmit
{
	public interface IPageTokenView
	{
		string GetLastPageToken
		{
			get;
		}

		string GetLastPageID
		{
			get;
		}

		bool TokensMatch
		{
			get;
		}

		string GeneratePageToken(string pageId);
	}
}
