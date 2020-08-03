using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTGLBaseService : RPTBaseService
	{
		protected List<RPTReportConfigModel> GetConfigList(MContext ctx, RPTGLReportType rptType)
		{
			return RPTReportConfigRepository.GetConfigList(ctx, ctx.MAccountTableID, rptType);
		}

		protected List<RPTReportConfigModel> ConvertAmount(MContext ctx, List<RPTReportConfigModel> configList, List<RPTGLAccountModel> acctList)
		{
			if (configList == null || configList.Count == 0 || acctList == null || acctList.Count == 0)
			{
				return configList;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (RPTReportConfigModel config in configList)
			{
				string mathFormula = GetMathFormula(config, acctList);
				if (!string.IsNullOrEmpty(mathFormula))
				{
					if (num > 0)
					{
						stringBuilder.Append(" UNION ALL ");
					}
					stringBuilder.AppendFormat(" SELECT '{0}' AS MItemID, {1} AS MAmount ", config.MItemID, mathFormula);
					num++;
				}
			}
			List<RPTReportConfigModel> amountList = RPTReportConfigRepository.GetAmountList(ctx, stringBuilder.ToString());
			if (amountList != null && amountList.Count > 0)
			{
				configList = (from p in configList
				join t in amountList on p.MItemID equals t.MItemID into temp
				from tt in temp.DefaultIfEmpty()
				select new RPTReportConfigModel
				{
					MItemID = p.MItemID,
					MParentID = p.MParentID,
					MReportType = p.MReportType,
					MRowType = p.MRowType,
					MAcctTableID = p.MAcctTableID,
					MSequence = p.MSequence,
					MTag = p.MTag,
					MName = p.MName,
					MNumber = p.MNumber,
					MAmount = (tt?.MAmount ?? decimal.Zero)
				} into t
				orderby t.MSequence
				select t).ToList();
			}
			return configList;
		}

		private string GetMathFormula(RPTReportConfigModel config, List<RPTGLAccountModel> acctList)
		{
			if (string.IsNullOrEmpty(config.MFormula))
			{
				return "";
			}
			string text = config.MFormula;
			MatchCollection matchCollection = Regex.Matches(text, "\\[贷\\]\\w*\\d+|\\[末\\]\\w*\\d+|\\[初\\]\\w*\\d+|\\[借\\]\\w*\\d+");
			foreach (Match item in matchCollection)
			{
				string value = item.Value;
				decimal amt = GetAmt(value, acctList);
				Regex regex = new Regex(value.Replace("[", "\\[").Replace("]", "\\]"));
				string replacement = (amt >= decimal.Zero) ? amt.ToString() : $"({amt})";
				text = regex.Replace(text, replacement, 1);
			}
			return text;
		}

		private decimal GetAmt(string str, List<RPTGLAccountModel> acctList)
		{
			string number = "";
			if (str.IndexOf("[借]") == 0)
			{
				number = str.Replace("[借]", "");
				RPTGLAccountModel rPTGLAccountModel = (from t in acctList
				where t.MAccountCode == number
				select t).FirstOrDefault();
				return rPTGLAccountModel?.MDebitAmt ?? decimal.Zero;
			}
			if (str.IndexOf("[贷]") == 0)
			{
				number = str.Replace("[贷]", "");
				RPTGLAccountModel rPTGLAccountModel2 = (from t in acctList
				where t.MAccountCode == number
				select t).FirstOrDefault();
				return rPTGLAccountModel2?.MCreditAmt ?? decimal.Zero;
			}
			if (str.IndexOf("[初]") == 0)
			{
				number = str.Replace("[初]", "");
				RPTGLAccountModel rPTGLAccountModel3 = (from t in acctList
				where t.MAccountCode == number
				select t).FirstOrDefault();
				return rPTGLAccountModel3?.MBeginBalAmt ?? decimal.Zero;
			}
			if (str.IndexOf("[末]") == 0)
			{
				number = str.Replace("[末]", "");
				RPTGLAccountModel rPTGLAccountModel4 = (from t in acctList
				where t.MAccountCode == number
				select t).FirstOrDefault();
				if (rPTGLAccountModel4 == null)
				{
					return decimal.Zero;
				}
				if (rPTGLAccountModel4.MDC == 1)
				{
					return rPTGLAccountModel4.MBeginBalAmt + rPTGLAccountModel4.MDebitAmt - rPTGLAccountModel4.MCreditAmt;
				}
				return rPTGLAccountModel4.MBeginBalAmt + rPTGLAccountModel4.MCreditAmt - rPTGLAccountModel4.MDebitAmt;
			}
			return decimal.Zero;
		}

		protected List<RPTGLAccountModel> ResetData(MContext ctx, List<RPTGLAccountModel> list, List<BDAccountListModel> acctList, DateTime fromDate, DateTime toDate)
		{
			if (list == null || list.Count == 0)
			{
				return list;
			}
			GLPeriodTransferBusiness gLPeriodTransferBusiness = new GLPeriodTransferBusiness();
			List<GLBalanceModel> list2 = gLPeriodTransferBusiness.GLBalanceListFromPeriodTransfer(ctx, fromDate.Year, fromDate.Month, toDate.Year, toDate.Month, 7, 1);
			if (list2 == null || list2.Count == 0)
			{
				return list;
			}
			list2 = ResetBalanceList(ctx, list2, acctList);
			foreach (RPTGLAccountModel item in list)
			{
				List<GLBalanceModel> list3 = (from t in list2
				where t.MAccountID == item.MAccountID
				select t).ToList();
				if (list3 != null && list3.Count > 0)
				{
					item.MCreditAmt -= list3.Sum((GLBalanceModel x) => x.MCredit);
					item.MDebitAmt -= list3.Sum((GLBalanceModel x) => x.MDebit);
				}
			}
			return list;
		}

		private List<GLBalanceModel> ResetBalanceList(MContext ctx, List<GLBalanceModel> glBalList, List<BDAccountListModel> acctList)
		{
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			foreach (GLBalanceModel glBal in glBalList)
			{
				if (!string.IsNullOrEmpty(glBal.MAccountCode) && glBal.MAccountCode.Length > 4)
				{
					ResetBalanceList(ctx, acctList, list, glBal.MCredit, glBal.MDebit, glBal.MAccountCode.Substring(0, glBal.MAccountCode.Length - 2));
				}
			}
			list.AddRange(glBalList);
			var source = (from p in list
			group p by new
			{
				p.MAccountID,
				p.MAccountCode
			} into g
			select new
			{
				Key = g.Key,
				MCredit = g.Sum((GLBalanceModel p) => p.MCredit),
				MDebit = g.Sum((GLBalanceModel p) => p.MDebit)
			}).ToList();
			return (from t in source
			select new GLBalanceModel
			{
				MAccountID = t.Key.MAccountID,
				MAccountCode = t.Key.MAccountCode,
				MDebit = t.MDebit,
				MCredit = t.MCredit
			}).ToList();
		}

		private void ResetBalanceList(MContext ctx, List<BDAccountListModel> acctList, List<GLBalanceModel> list, decimal creditAmt, decimal debitAmt, string acctCode)
		{
			BDAccountListModel accountByCode = BDAccountRepository.GetAccountByCode(acctList, acctCode);
			if (accountByCode != null)
			{
				GLBalanceModel item = new GLBalanceModel
				{
					MAccountID = accountByCode.MItemID,
					MAccountCode = accountByCode.MCode,
					MCredit = creditAmt,
					MDebit = debitAmt
				};
				list.Add(item);
				string acctCode2 = acctCode.Substring(0, acctCode.Length - 2);
				if (acctCode.Length > 4)
				{
					ResetBalanceList(ctx, acctList, list, creditAmt, debitAmt, acctCode2);
				}
			}
		}
	}
}
