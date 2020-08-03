using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class FilterList : List<SqlFilter>
	{
	}
}
