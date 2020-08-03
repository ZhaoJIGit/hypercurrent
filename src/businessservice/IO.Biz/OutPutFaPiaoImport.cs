using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.BusinessService.IO.Biz
{
	public class OutPutFaPiaoImport : FaPiaoImport
	{
		public override void CheckColumnDataIsEffective(MContext ctx, List<KeyValuePair<int, IOSolutionConfigModel>> columnMath, DataRow dr, KeyValuePair<int, IOSolutionConfigModel> kv, string str, List<string> errorMsgList, int rowIndex, bool isMainFaPiao, out bool isRightDataRow, out bool isRightRow)
		{
			isRightRow = true;
			isRightDataRow = true;
			if (kv.Value.MConfigStandardName == "MItemName" && str.Contains("详见销货清单"))
			{
				isRightRow = false;
				isRightDataRow = false;
				errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "FaPiaoIncludeDetailList", "第{0}行数据包含销货清单，对应发票请使用开票软件导出销货清单后，进行再次导入！"), rowIndex + 1));
			}
			else if (kv.Value.MConfigStandardName == "MTaxRate" && string.IsNullOrEmpty(str))
			{
				isRightRow = false;
				isRightDataRow = false;
				errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TaxRateIsRequired", "第{0}行税率不能为空！"), rowIndex + 1));
			}
			else if (isMainFaPiao && kv.Value.MIsDataRequired && string.IsNullOrEmpty(str))
			{
				isRightRow = false;
				isRightDataRow = false;
				string mConfigStandardName = kv.Value.MConfigStandardName;
				if (mConfigStandardName == "MSContactName")
				{
					errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "ContactNameIsRequired", "第{0}行购方企业名称不能为空！"), rowIndex + 1));
				}
			}
		}

		public override void SetFaPiaoModel(DataRow dr, FPFapiaoModel model)
		{
			model.MPContactName = dr["MSContactName"].ToString().Trim();
			model.MPContactBankInfo = dr["MSContactBankInfo"].ToString().Trim();
			model.MPContactTaxCode = dr["MSContactTaxCode"].ToString().Trim();
			model.MPContactAddressPhone = dr["MSContactAddressPhone"].ToString().Trim();
			model.MInvoiceType = 0;
			model.MVerifyType = 3;
		}

		public override void CheckFapiaoModelListIsEqual(MContext ctx, List<FPFapiaoModel> importFapiaoList, List<FPFapiaoModel> sysFapiaoList, List<string> errorMsgList)
		{
			foreach (FPFapiaoModel importFapiao in importFapiaoList)
			{
				FPFapiaoModel fPFapiaoModel = sysFapiaoList.FirstOrDefault((FPFapiaoModel a) => a.MCode == importFapiao.MCode && a.MNumber == importFapiao.MNumber);
				if (fPFapiaoModel != null && !base.CheckFapiaoModelIsEqual(importFapiao, fPFapiaoModel))
				{
					errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "OutputFapiaoIsRepeat", "发票号码{0}在系统中已经存在，并且除发票状态之外的其他信息不相同，请删除对应的发票后再次进行导入！"), importFapiao.MNumber));
					break;
				}
			}
		}

		public override void SetFapiaoListDetail(MContext ctx, FPFapiaoModel model)
		{
			model.MSContactTaxCode = ctx.MTaxCode;
			model.MSContactName = ctx.MLegalTradingName;
			model.MSContactBankInfo = "";
			model.MSContactAddressPhone = "";
			model.MInvoiceType = 0;
		}

		public override bool CheckFapiaoBodyIsEqual(FPFapiaoModel importFapiao, FPFapiaoModel sysFapiao)
		{
			if (!importFapiao.MType.Equals(sysFapiao.MType))
			{
				return false;
			}
			if (!importFapiao.MBizDate.Equals(sysFapiao.MBizDate))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MReceiver, sysFapiao.MReceiver))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MRemark, sysFapiao.MRemark))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MPContactTaxCode, sysFapiao.MPContactTaxCode))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MPContactName, sysFapiao.MPContactName))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MPContactBankInfo, sysFapiao.MPContactBankInfo))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MPContactAddressPhone, sysFapiao.MPContactAddressPhone))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MReaduitor, sysFapiao.MReaduitor))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MDrawer, sysFapiao.MDrawer))
			{
				return false;
			}
			return true;
		}
	}
}
