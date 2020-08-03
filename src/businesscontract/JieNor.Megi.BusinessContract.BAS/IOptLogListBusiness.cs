using JieNor.Megi.Core;
using JieNor.Megi.Core.Log;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IOptLogListBusiness
	{
		DataGridJson<OptLogListModel> GetOptLogList(MContext ctx, OptLogListFilter filter);
	}
}
