using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.Log
{
	public class FPFapiaoLogHelper
	{
		private static readonly FPFapiaoLogBase svcLog = new FPFapiaoLogBase();

		public static void SaveLog(MContext ctx, FPFapiaoModel model)
		{
			if (model.IsNew)
			{
				svcLog.CreateLog(ctx, model);
			}
			else
			{
				svcLog.EditLog(ctx, model);
			}
		}

		public static void DeleteLog(MContext ctx, FPFapiaoModel model)
		{
			svcLog.DeleteLog(ctx, model);
		}

		public static List<CommandInfo> BatchUpdateFPStatusLog(MContext ctx, List<FPFapiaoModel> fpList, int status)
		{
			return svcLog.BatchUpdateFPStatusLog(ctx, fpList, status);
		}

		public static List<CommandInfo> BatchUpdateFPVerifyTypeLog(MContext ctx, List<FPFapiaoModel> entityList, List<FPFapiaoModel> modelList)
		{
			return svcLog.BatchUpdateFPVerifyTypeLog(ctx, entityList, modelList);
		}

		public static List<CommandInfo> GetCreateLogCmd(MContext ctx, FPFapiaoModel model)
		{
			return new List<CommandInfo>
			{
				svcLog.GetCreateLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetEditLogCmd(MContext ctx, FPFapiaoModel model)
		{
			return new List<CommandInfo>
			{
				svcLog.GetEditLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetEditOrAddLogCmd(MContext ctx, FPFapiaoModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (model.IsNew)
			{
				list.Add(svcLog.GetCreateLogCmd(ctx, model));
			}
			else
			{
				list.Add(svcLog.GetEditLogCmd(ctx, model));
			}
			return list;
		}
	}
}
