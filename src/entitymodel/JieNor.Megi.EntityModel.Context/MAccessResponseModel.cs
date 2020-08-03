using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Context
{
	[DataContract]
	public class MAccessResponseModel
	{
		[DataMember]
		public Dictionary<string, bool> Access
		{
			get;
			set;
		}

		[DataMember]
		public MContext ctx
		{
			get;
			set;
		}

		[DataMember]
		public bool And
		{
			get
			{
				if (Access == null || Access.Keys.Count == 0)
				{
					return false;
				}
				foreach (KeyValuePair<string, bool> item in Access)
				{
					if (!item.Value)
					{
						return false;
					}
				}
				return true;
			}
			set
			{
			}
		}

		[DataMember]
		public bool Or
		{
			get
			{
				if (Access == null || Access.Keys.Count == 0)
				{
					return false;
				}
				foreach (KeyValuePair<string, bool> item in Access)
				{
					if (item.Value)
					{
						return true;
					}
				}
				return false;
			}
			set
			{
			}
		}
	}
}
