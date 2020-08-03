using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import.Account
{
	[DataContract]
	public class IOAccountMatchModel
	{
		[DataMember]
		public List<IOAccountModel> ManualMatchList
		{
			get;
			set;
		}
	}
}
