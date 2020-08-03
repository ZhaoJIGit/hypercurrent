using JieNor.Megi.Core;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.BusinessService.IO.Biz
{
	public class InPutFaPiaoImport : FaPiaoImport
	{
		public override void CheckColumnDataIsEffective(MContext ctx, List<KeyValuePair<int, IOSolutionConfigModel>> columnMath, DataRow dr, KeyValuePair<int, IOSolutionConfigModel> kv, string str, List<string> errorMsgList, int rowIndex, bool isMainFaPiao, out bool isRightDataRow, out bool isRightRow)
		{
			isRightRow = true;
			isRightDataRow = true;
			if (isMainFaPiao)
			{
				if (kv.Value.MIsDataRequired && string.IsNullOrEmpty(str))
				{
					isRightRow = false;
					isRightDataRow = false;
					string mConfigStandardName = kv.Value.MConfigStandardName;
					if (mConfigStandardName == "MSContactName")
					{
						errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "SaleContactNameIsRequired", "第{0}行销方企业名称不能为空！"), rowIndex + 1));
					}
				}
				KeyValuePair<int, IOSolutionConfigModel> keyValuePair = columnMath.Find((KeyValuePair<int, IOSolutionConfigModel> a) => a.Value.MConfigStandardName == "MType");
				KeyValuePair<int, IOSolutionConfigModel> keyValuePair2 = columnMath.Find((KeyValuePair<int, IOSolutionConfigModel> a) => a.Value.MConfigStandardName == "MVerifyType");
				KeyValuePair<int, IOSolutionConfigModel> keyValuePair3 = columnMath.Find((KeyValuePair<int, IOSolutionConfigModel> a) => a.Value.MConfigStandardName == "MVerifyDate");
				KeyValuePair<int, IOSolutionConfigModel> keyValuePair4 = columnMath.Find((KeyValuePair<int, IOSolutionConfigModel> a) => a.Value.MConfigStandardName == "MBizDate");
				string fapiaoType = dr[keyValuePair.Key].ToString();
				string a2 = dr[keyValuePair2.Key].ToString();
				bool flag = a2 == "扫描认证" || a2 == "勾选认证";
				if ((base.IsSpecilaFapiao(fapiaoType) & flag) && keyValuePair3.Value == null)
				{
					isRightRow = false;
					isRightDataRow = false;
					errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "VerifyDateIsRequired", "第{0}行增值税专用发票认证方式信息是扫描认证、勾选认证的情况下,确认/认证日期必须填写,请重新填写后进行再次导入"), rowIndex + 1));
				}
				else
				{
					if (kv.Value.MDataType == "datetime" && kv.Value.MConfigStandardName == "MVerifyDate")
					{
						bool flag2 = string.IsNullOrEmpty(str);
						if (base.IsSpecilaFapiao(fapiaoType) & flag & flag2)
						{
							isRightRow = false;
							isRightDataRow = false;
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "VerifyDateIsRequired", "第{0}行增值税专用发票认证方式信息是扫描认证、勾选认证的情况下,确认/认证日期必须填写,请重新填写后进行再次导入"), rowIndex + 1));
							return;
						}
						if (!flag2)
						{
							DateTime t = Convert.ToDateTime(str);
							string value = dr[keyValuePair4.Key].ToString();
							if (!string.IsNullOrEmpty(value))
							{
								DateTime t2 = Convert.ToDateTime(value);
								if (t2 > t)
								{
									isRightRow = false;
									isRightDataRow = false;
									errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "VerifyDateIsError", "第{0}行认证日期必须大于或等于发票开票日期!"), rowIndex + 1));
									return;
								}
							}
						}
					}
					if (base.IsSpecilaFapiao(fapiaoType))
					{
						if (keyValuePair2.Value == null)
						{
							isRightRow = false;
							isRightDataRow = false;
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "VerifyTypeIsRequired", "第{0}行增值税专用发票,认证方式为必填!"), rowIndex + 1));
						}
						else if (kv.Value.MConfigStandardName == "MVerifyType")
						{
							List<string> list = new List<string>
							{
								"扫描认证",
								"勾选认证",
								"未认证"
							};
							if (!list.Exists((string a) => a == str))
							{
								isRightRow = false;
								isRightDataRow = false;
								errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "VerifyTypeIsError", "第{0}行增值税专用发票认证信息只能填写扫描认证、勾选认证、未认证,请重新填写后进行再次导入!"), rowIndex + 1));
							}
						}
					}
				}
			}
		}

		public override void SetFaPiaoModel(DataRow dr, FPFapiaoModel model)
		{
			if (model.MType == 1)
			{
				if (!string.IsNullOrEmpty(dr["MVerifyType"]?.ToString()))
				{
					model.MVerifyType = int.Parse(dr["MVerifyType"].ToString());
				}
				if (!string.IsNullOrEmpty(dr["MVerifyDate"]?.ToString()) && model.MVerifyType != 0)
				{
					model.MVerifyDate = Convert.ToDateTime(dr["MVerifyDate"]);
				}
			}
			else
			{
				model.MVerifyType = 3;
			}
			model.MSContactName = dr["MSContactName"].ToString().Trim();
			model.MSContactBankInfo = dr["MSContactBankInfo"].ToString().Trim();
			model.MSContactTaxCode = dr["MSContactTaxCode"].ToString().Trim();
			model.MSContactAddressPhone = dr["MSContactAddressPhone"].ToString().Trim();
			model.MInvoiceType = 1;
		}

		public override void CheckFapiaoModelListIsEqual(MContext ctx, List<FPFapiaoModel> importFapiaoList, List<FPFapiaoModel> sysFapiaoList, List<string> errorMsgList)
		{
			foreach (FPFapiaoModel importFapiao in importFapiaoList)
			{
				FPFapiaoModel fPFapiaoModel = sysFapiaoList.FirstOrDefault((FPFapiaoModel a) => a.MCode == importFapiao.MCode && a.MNumber == importFapiao.MNumber);
				if (fPFapiaoModel != null && !base.CheckFapiaoModelIsEqual(importFapiao, fPFapiaoModel))
				{
					errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "InputFapiaoIsRepeat", "发票号码{0}在系统中已经存在，并且除发票状态、认证方式和确认/认证日期之外的其他信息不相同，请删除对应的发票后再次进行导入"), importFapiao.MNumber));
				}
			}
		}

		public override void SetFapiaoListDetail(MContext ctx, FPFapiaoModel model)
		{
			model.MPContactTaxCode = ctx.MTaxCode;
			model.MPContactName = ctx.MLegalTradingName;
			model.MInvoiceType = 1;
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
			if (!base.CompareStringField(importFapiao.MSContactTaxCode, sysFapiao.MSContactTaxCode))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MSContactName, sysFapiao.MSContactName))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MSContactAddressPhone, sysFapiao.MSContactAddressPhone))
			{
				return false;
			}
			if (!base.CompareStringField(importFapiao.MSContactBankInfo, sysFapiao.MSContactBankInfo))
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

		public override bool IsSetLocalUpload(MContext ctx, int type)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MType", 1);
			sqlWhere.Equal("MFPType", type);
			List<FPImportTypeConfigModel> modelList = new FPSettingRepository().GetModelList(ctx, sqlWhere, false);
			return modelList != null && modelList.Count > 0 && modelList[0].MImportType == 1;
		}
	}
}
