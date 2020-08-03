using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.DataRepository.Log.GlLog
{
	public interface IGLVoucherLog
	{
		void CreateLog(MContext ctx, GLVoucherModel model);

		void EditLog(MContext ctx, GLVoucherModel model);

		void DeleteLog(MContext ctx, GLVoucherModel model);

		void ApproveLog(MContext ctx, GLVoucherModel model);

		void ReverseApproveLog(MContext ctx, GLVoucherModel model);

		CommandInfo GetDeleteLogCmd(MContext ctx, GLVoucherModel table);

		CommandInfo GetApproveLogCmd(MContext ctx, GLVoucherModel model);

		CommandInfo GetReverseApproveLogCmd(MContext ctx, GLVoucherModel model);

		CommandInfo GetCreateLogCmd(MContext ctx, GLVoucherModel model);

		CommandInfo GetEditLogCmd(MContext ctx, GLVoucherModel model);
	}
}
