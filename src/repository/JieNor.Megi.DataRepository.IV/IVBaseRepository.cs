using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVBaseRepository<T1> : DataServiceT<T1> where T1 : BaseModel
	{
		public static OperationResult DeleteBill<T>(MContext ctx, string keyId) where T : BizDataModel
		{
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, keyId);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetDeleteFlagCmd<T>(ctx, keyId));
			list.AddRange((IEnumerable<CommandInfo>)GLInterfaceRepository.GetDeleteVoucherByDocIDCmds(ctx, keyId));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = (num > 0),
				ObjectID = keyId
			};
		}

		public static OperationResult DeleteBill<T>(MContext ctx, ParamBase param) where T : BizDataModel
		{
			List<string> list = Enumerable.ToList<string>((IEnumerable<string>)param.KeyIDSWithNoSingleQuote.Split(','));
			List<CommandInfo> list2 = new List<CommandInfo>();
			OperationResult operationResult = new OperationResult();
			string message = string.Empty;
			if (param.MIsInit && ctx.MRegProgress >= 13 && ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InitBalanceIsOver", "The initial balance has been completed and is not allowed to initialize the document operation!");
				return operationResult;
			}
			OperationResult operationResult2 = GLInterfaceRepository.IsDocCanOperate(ctx, Enumerable.ToList<string>((IEnumerable<string>)list));
			if (!operationResult2.Success)
			{
				operationResult.Success = false;
				operationResult.Message = operationResult2.Message;
				operationResult.ErrorMessageDetail = operationResult2.Message;
			}
			Type typeFromHandle = typeof(T);
			foreach (string item in list)
			{
				if (typeFromHandle == typeof(IVReceiveModel))
				{
					if (!IVVerificationRepository.CheckIsCanEditOrDelete(ctx, "Receive", item))
					{
						message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！");
						operationResult.Success = false;
						break;
					}
				}
				else if (typeFromHandle == typeof(IVPaymentModel) && !IVVerificationRepository.CheckIsCanEditOrDelete(ctx, "Payment", item))
				{
					message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！");
					operationResult.Success = false;
					break;
				}
			}
			if (!operationResult.Success)
			{
				return new OperationResult
				{
					Success = false,
					Message = message
				};
			}
			list2.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetDeleteFlagCmd<T>(ctx, Enumerable.ToList<string>((IEnumerable<string>)list)));
			list2.AddRange((IEnumerable<CommandInfo>)GLInterfaceRepository.GetDeleteVoucherByDocIDsCmds(ctx, Enumerable.ToList<string>((IEnumerable<string>)list)));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list2);
			return new OperationResult
			{
				Success = true
			};
		}

		public static List<CommandInfo> GetDeleteBillCmd<T>(MContext ctx, ParamBase param, OperationResult result = null) where T : BizDataModel
		{
			List<string> source = Enumerable.ToList<string>((IEnumerable<string>)param.KeyIDSWithNoSingleQuote.Split(','));
			List<CommandInfo> list = new List<CommandInfo>();
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, Enumerable.ToList<string>((IEnumerable<string>)source));
			if (!operationResult.Success)
			{
				result = (result ?? new OperationResult());
				result.Success = false;
				result.Message = operationResult.Message;
				result.ErrorMessageDetail = operationResult.Message;
				return new List<CommandInfo>();
			}
			list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetDeleteFlagCmd<T>(ctx, Enumerable.ToList<string>((IEnumerable<string>)source)));
			list.AddRange((IEnumerable<CommandInfo>)GLInterfaceRepository.GetDeleteVoucherByDocIDsCmds(ctx, Enumerable.ToList<string>((IEnumerable<string>)source)));
			return list;
		}

		public static OperationResult ArchiveBill<T>(MContext ctx, string keyId) where T : BizDataModel
		{
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, keyId);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetArchiveFlagCmd<T>(ctx, keyId));
			list.AddRange((IEnumerable<CommandInfo>)GLInterfaceRepository.GetDeleteVoucherByDocIDCmds(ctx, keyId));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = (num > 0),
				ObjectID = keyId
			};
		}

		public static OperationResult ArchiveBill<T>(MContext ctx, ParamBase param) where T : BizDataModel
		{
			List<string> source = Enumerable.ToList<string>((IEnumerable<string>)param.KeyIDSWithNoSingleQuote.Split(','));
			List<CommandInfo> list = new List<CommandInfo>();
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			string empty = string.Empty;
			OperationResult operationResult2 = GLInterfaceRepository.IsDocCanOperate(ctx, Enumerable.ToList<string>((IEnumerable<string>)source));
			if (!operationResult2.Success)
			{
				operationResult.Success = false;
				operationResult.Message = operationResult2.Message;
				operationResult.ErrorMessageDetail = operationResult2.Message;
			}
			list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetDeleteFlagCmd<T>(ctx, Enumerable.ToList<string>((IEnumerable<string>)source)));
			list.AddRange((IEnumerable<CommandInfo>)GLInterfaceRepository.GetDeleteVoucherByDocIDsCmds(ctx, Enumerable.ToList<string>((IEnumerable<string>)source)));
			if (!operationResult.Success)
			{
				return new OperationResult
				{
					Success = false,
					Message = empty
				};
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return operationResult;
		}

		public static OperationResult UpdateBill<T>(MContext ctx, T model, List<CommandInfo> cmdInfo = null) where T : BizDataModel
		{
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetInsertOrUpdateCmd<T>(ctx, (BaseModel)model, (List<string>)null, true));
			OperationResult operationResult2 = GLInterfaceRepository.GenerateVouchersByBill<T>(ctx, model, (T)null);
			if (operationResult2.Success)
			{
				list.AddRange((IEnumerable<CommandInfo>)operationResult2.OperationCommands);
				if (cmdInfo != null)
				{
					list.AddRange((IEnumerable<CommandInfo>)cmdInfo);
				}
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
				return new OperationResult
				{
					Success = true,
					ObjectID = model.MID
				};
			}
			return operationResult2;
		}
	}
}
