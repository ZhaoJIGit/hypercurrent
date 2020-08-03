using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.BusinessService.IO.Biz
{
	public class HangTianFaPiaoImport : OutPutFaPiaoImport
	{
		public override DataTable GetEffiectiveData(MContext ctx, IOImportDataModel model, List<IOSolutionConfigModel> soluConfig, DataTable sourceData, out List<string> errorMsgList)
		{
			errorMsgList = new List<string>();
			if (!FaPiaoImport.CheckSourceDataIsEmpty(sourceData, errorMsgList))
			{
				return null;
			}
			int count = sourceData.Rows.Count;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				if (sourceData.Rows[i][0].ToString().Contains("发票类别") && sourceData.Rows[i][0].ToString().Contains("专用发票"))
				{
					num = i + 1;
				}
				else if (sourceData.Rows[i][0].ToString().Contains("发票类别") && sourceData.Rows[i][0].ToString().Contains("普通发票"))
				{
					num2 = i + 1;
				}
			}
			if (num != 0 && num2 != 0 && num2 <= num)
			{
				errorMsgList.Add(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "GeneralFaPiaoMustLargeSpecialFaPiao", "航天发票文件。专用发票的数据必须在普通发票之前!"));
				return null;
			}
			base.GeneralTitleBegin = num2;
			int num3 = (num != 0) ? num : num2;
			return base.GetNewFaPiaoDataTable(ctx, soluConfig, sourceData, num3, num3 + 1, out errorMsgList);
		}

		public override bool CheckFapiaoBodyIsEqual(FPFapiaoModel importFapiao, FPFapiaoModel sysFapiao)
		{
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

		public override void SetInvoiceType(int currentDataIndex, int commonBeginIndex, DataRow dr)
		{
			if (commonBeginIndex != 0 && commonBeginIndex < currentDataIndex)
			{
				dr["MType"] = "增值税普通发票";
			}
			else
			{
				dr["MType"] = "增值税专用发票";
			}
		}
	}
}
