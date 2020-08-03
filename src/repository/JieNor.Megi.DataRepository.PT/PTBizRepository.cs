using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.PT
{
	public class PTBizRepository : DataServiceT<BDPrintSettingModel>
	{
		public List<BDPrintSettingModel> GetList(MContext ctx)
		{
			string sql = string.Format("select a.MItemID,a.MOrgID,concat(MTopMargin, ' ', MMeasureIn) as MTopMarginWithUnit,concat(MBottomMargin, ' ', MMeasureIn) as MBottomMarginWithUnit,\r\n                concat(MAddressPadding, ' ', MMeasureIn) as MAddressPaddingWithUnit,MShowTaxNumber,MShowHeading,MShowUnitPriceAndQuantity,MShowTaxColumn,MShowRegAddress,MMeasureIn,MTopMargin,MBottomMargin,MAddressPadding,\r\n                MShowLogo,MShowTracking,MHideDiscount,MShowTaxSubTotalWay,MShowCurrencyConversionWay,concat_ws('{1}',d.MName, ifnull(e.MName, ''), convert(AES_DECRYPT(c.MBankNo ,'{0}') using utf8)) as MPayService,\r\n                MTermsAndPayAdvice,MLogoID,MLogoAlignment,MShowTaxType,MContactDetails,MSeq,\r\n                l.MName,MDraftInvoiceTitle,MApprovedInvoiceTitle,MOverdueInvoiceTitle,MCreditNoteTitle,MStatementTitle,MPaymentTitle,MReceiptTitle,\r\n                concat_ws(', ',MDraftInvoiceTitle,MApprovedInvoiceTitle,MOverdueInvoiceTitle,MCreditNoteTitle,MStatementTitle) as Headings\r\n                from t_bd_printsetting a\r\n                left join t_bd_printsetting_l l\r\n                on a.MItemID=l.MParentID and l.MLocaleID=@MLocaleID  and l.MOrgID = a.MOrgID and l.MIsDelete = 0 \n\t\t\t\tleft join T_BD_BankAccount c on a.MPayService=c.MItemID and c.MOrgID = a.MOrgID and c.MIsDelete = 0                           \r\n                left join T_BD_BankAccount_l d on a.MPayService=d.MParentID and d.MLocaleID=@MLocaleID and d.MOrgID = a.MOrgID and d.MIsDelete = 0 \n\t\t\t\tleft join T_BD_BankType_l e on c.MBankTypeID=e.MParentID and e.MLocaleID=@MLocaleID and (e.MOrgID = a .MOrgID or ifnull(e.MOrgID, '')='') and e.MIsDelete = 0 \r\n                where a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                order by MSeq,a.MCreateDate", "JieNor-001", '┇');
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			return ModelInfoManager.GetDataModelBySql<BDPrintSettingModel>(ctx, sql, array);
		}

		public BDPrintSettingModel GetPrintSetting(MContext ctx, string itemID)
		{
			string sql = string.Format("select a.MItemID,a.MOrgID,MMeasureIn,MTopMargin,MBottomMargin,MAddressPadding,MShowTaxNumber,MShowHeading,MShowUnitPriceAndQuantity,\n                MShowTaxColumn,MShowRegAddress,MShowLogo,MShowTracking,MHideDiscount,MShowTaxSubTotalWay,MShowCurrencyConversionWay,concat_ws(',',d.MName, ifnull(e.MName, ''), \n                convert(AES_DECRYPT(c.MBankNo ,'{0}') using utf8)) as MPayService,MTermsAndPayAdvice,MLogoID,MLogoAlignment,MShowTaxType,MContactDetails,MSeq,\n                l.MName,MDraftInvoiceTitle,MApprovedInvoiceTitle,MOverdueInvoiceTitle,MCreditNoteTitle,MStatementTitle,MPaymentTitle,MReceiptTitle\n                from t_bd_printsetting a\n\t\t\t\tleft join T_BD_BankAccount c on a.MPayService=c.MItemID  and c.MOrgID = a .MOrgID and c.MIsDelete = 0 \n                left join T_BD_BankAccount_l d on a.MPayService=d.MParentID and d.MLocaleID=@MLocaleID  and d.MOrgID = a .MOrgID and d.MIsDelete = 0 \n                left join t_bd_printsetting_l l on a.MItemID=l.MParentID and l.MLocaleID=@MLocaleID  and l.MOrgID = a .MOrgID and l.MIsDelete = 0 \n\t\t\t\tleft join T_BD_BankType_l e on c.MBankTypeID=e.MParentID and e.MLocaleID=@MLocaleID  and (e.MOrgID = a .MOrgID or ifnull(e.MOrgID, '')='') and e.MIsDelete = 0                            \n                where a.MOrgID=@MOrgID and a.MItemID=@MItemID", "JieNor-001");
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MItemID", itemID),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			List<BDPrintSettingModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDPrintSettingModel>(ctx, sql, cmdParms);
			if (dataModelBySql.Count > 0)
			{
				return dataModelBySql[0];
			}
			return null;
		}

		public OperationResult Sort(MContext ctx, string ids)
		{
			OperationResult operationResult = new OperationResult();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<CommandInfo> list = new List<CommandInfo>();
			int num = 0;
			string commandText = "update t_bd_printsetting set MSeq=@MSeq where MItemID=@MItemID and MOrgID = @MOrgID and MIsDelete = 0 ";
			try
			{
				string[] array = ids.Split(',');
				foreach (string value in array)
				{
					MySqlParameter[] parameters = new MySqlParameter[3]
					{
						new MySqlParameter("@MSeq", num),
						new MySqlParameter("@MItemID", value),
						new MySqlParameter("@MOrgID", ctx.MOrgID)
					};
					List<CommandInfo> list2 = list;
					CommandInfo obj = new CommandInfo
					{
						CommandText = commandText
					};
					DbParameter[] array2 = obj.Parameters = parameters;
					list2.Add(obj);
					num++;
				}
				dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		public override OperationResult Delete(MContext ctx, string pkID)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDPrintSettingModel>(ctx, pkID));
				list.AddRange(RPTReportRepository.GetDelReportLayoutCmds(ctx, new List<string>
				{
					pkID
				}));
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		public OperationResult CheckNameExist(MContext ctx, string itemId, string name)
		{
			OperationResult operationResult = new OperationResult();
			string text = "select Count(*) from t_bd_printsetting a \r\n                            left join t_bd_printsetting_l b \r\n                            on a.MItemID=b.MParentID and MLocaleID=@MLocaleID and a.MOrgID = b.MOrgID and b.MIsDelete = 0 \r\n                            WHERE a.MIsDelete=0 and a.MOrgID=@MOrgID AND b.MName=@MName ";
			if (!string.IsNullOrEmpty(itemId))
			{
				text += " AND a.MItemID <> @MItemID ";
			}
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MName", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			array[2].Value = name;
			array[3].Value = itemId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(text, array);
			if (Convert.ToInt32(single) > 0)
			{
				operationResult.Success = false;
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PrintSettingExist", "A Print Setting with the same name already exists.");
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text2
				});
			}
			return operationResult;
		}

		public OperationResult DeletePrintSetting(MContext ctx, List<string> idList)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDPrintSettingModel>(ctx, idList));
				list.AddRange(RPTReportRepository.GetDelReportLayoutCmds(ctx, idList));
				int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		public OperationResult CopySettingWithTemplate(MContext ctx, BDPrintSettingModel copyModel, string srcModelId)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDPrintSettingModel>(ctx, copyModel, null, true));
				List<RPTReportLayoutModel> reportLayoutList = RPTReportRepository.GetReportLayoutList(ctx, srcModelId);
				if (reportLayoutList.Any())
				{
					foreach (RPTReportLayoutModel item in reportLayoutList)
					{
						item.MID = string.Empty;
						item.MPrintSettingID = copyModel.MItemID;
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<RPTReportLayoutModel>(ctx, item, null, true));
					}
				}
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}
	}
}
