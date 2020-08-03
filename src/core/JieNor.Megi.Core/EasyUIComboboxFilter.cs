using System.Configuration;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class EasyUIComboboxFilter
	{
		private static readonly string _maxCount = ConfigurationManager.AppSettings["ComboboxMaxCount"];

		[DataMember]
		public string q
		{
			get;
			set;
		}

		[DataMember]
		public string v
		{
			get;
			set;
		}

		[DataMember]
		public int MaxCount
		{
			get;
			set;
		}

		public EasyUIComboboxFilter()
		{
			if (string.IsNullOrEmpty(_maxCount))
			{
				MaxCount = 50;
			}
			else
			{
				int num = 0;
				if (!int.TryParse(_maxCount, out num) || num <= 0)
				{
					MaxCount = 50;
				}
				MaxCount = num;
			}
		}
	}
}
