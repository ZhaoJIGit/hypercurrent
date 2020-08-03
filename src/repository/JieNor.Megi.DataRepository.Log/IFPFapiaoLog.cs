using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.Log
{
	public interface IFPFapiaoLog
	{
		void CreateLog(MContext ctx, FPFapiaoModel model);

		void EditLog(MContext ctx, FPFapiaoModel model);

		void DeleteLog(MContext ctx, FPFapiaoModel model);

		CommandInfo GetCreateLogCmd(MContext ctx, FPFapiaoModel model);

		CommandInfo GetEditLogCmd(MContext ctx, FPFapiaoModel model);

		List<CommandInfo> BatchUpdateFPStatusLog(MContext ctx, List<FPFapiaoModel> fpList, int status);

		List<CommandInfo> BatchUpdateFPVerifyTypeLog(MContext ctx, List<FPFapiaoModel> entityList, List<FPFapiaoModel> modelList);
	}
}
