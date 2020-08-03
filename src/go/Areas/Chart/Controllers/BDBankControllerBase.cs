using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.BD;

namespace JieNor.Megi.Go.Web.Areas.Controllers
{
	public class BDBankControllerBase : GoControllerBase
	{
		protected IBDBankAccount _bdAccount;

		public BDBankControllerBase(IBDBankAccount bdAccount)
		{
			base.SetModule(SysModuleEnum.Account);
		}

		public BDBankControllerBase()
		{
			base.SetModule(SysModuleEnum.Account);
		}
	}
}
