using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.Core
{
	public interface IBasicBusiness<T> where T : BaseModel, new()
	{
		DataGridJson<T> Get(MContext ctx, GetParam param);

		List<T> Post(MContext ctx, PostParam<T> param);

		T Delete(MContext ctx, DeleteParam param);
	}
}
