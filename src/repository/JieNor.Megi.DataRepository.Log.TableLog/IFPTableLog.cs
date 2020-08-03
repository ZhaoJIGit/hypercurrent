using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.DataRepository.Log.TableLog
{
	public interface IFPTableLog
	{
		void CreateLog(MContext ctx, FPTableViewModel table);

		void EditLog(MContext ctx, FPTableViewModel table);

		void DeleteLog(MContext ctx, FPTableViewModel table);

		CommandInfo GetDeleteLogCmd(MContext ctx, FPTableViewModel table);

		CommandInfo GetCreateLogCmd(MContext ctx, FPTableViewModel table);

		CommandInfo GetEditLogCmd(MContext ctx, FPTableViewModel table);
	}
}
