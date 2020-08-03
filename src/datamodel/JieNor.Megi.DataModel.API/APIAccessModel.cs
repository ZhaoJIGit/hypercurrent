using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.API
{
	[DataContract]
	public class APIAccessModel
	{
		public string MItemID
		{
			get;
			set;
		}

		public string MOrgID
		{
			get;
			set;
		}

		public string MAppID
		{
			get;
			set;
		}

		public string MSecret
		{
			get;
			set;
		}

		public string MToken
		{
			get;
			set;
		}

		public DateTime MExpireTime
		{
			get;
			set;
		}

		public DateTime MRefreshTime
		{
			get;
			set;
		}

		public int MRefreshCount
		{
			get;
			set;
		}

		public bool MIsActive
		{
			get;
			set;
		}
	}
}
