using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class ListFilter : EasyUIComboboxFilter
	{
		public string QueryString
		{
			get
			{
				return base.q;
			}
		}

		public string MID
		{
			get
			{
				return base.v;
			}
			set
			{
				base.v = value;
			}
		}
	}
}
