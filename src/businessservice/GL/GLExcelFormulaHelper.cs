using JieNor.Megi.Core;
using JieNor.Megi.DataModel.Formula;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLExcelFormulaHelper
	{
		public T GetFilterByFormula<T>(BatchFormaluModel formula)
		{
			T result = default(T);
			if (formula.FormulaType == 0)
			{
				object obj = AnalysisMegiAcct(formula);
				return (T)obj;
			}
			if (formula.FormulaType == 1)
			{
				object obj2 = AnalysisMegiInvoice(formula);
				return (T)obj2;
			}
			return result;
		}

		public GLBalanceListFilterModel AnalysisMegiAcct(BatchFormaluModel formula)
		{
			GLBalanceListFilterModel gLBalanceListFilterModel = new GLBalanceListFilterModel();
			gLBalanceListFilterModel.AccountIDS = ConvertParma<List<string>>(formula.FormulaParam, "Account", 2);
			string text = ConvertParma<string>(formula.FormulaParam, "StartDate", 1);
			gLBalanceListFilterModel.StartYear = ((!string.IsNullOrWhiteSpace(text)) ? Convert.ToInt32(text.Substring(0, 4)) : 0);
			gLBalanceListFilterModel.StartPeriod = ((!string.IsNullOrWhiteSpace(text)) ? Convert.ToInt32(text.Substring(4, 2)) : 0);
			gLBalanceListFilterModel.MCurrencyID = ConvertParma<string>(formula.FormulaParam, "CurrencyID", 1);
			string text2 = ConvertParma<string>(formula.FormulaParam, "EndDate", 1);
			gLBalanceListFilterModel.EndYear = ((!string.IsNullOrWhiteSpace(text)) ? Convert.ToInt32(text2.Substring(0, 4)) : 0);
			gLBalanceListFilterModel.EndPeriod = ((!string.IsNullOrWhiteSpace(text)) ? Convert.ToInt32(text2.Substring(4, 2)) : 0);
			string text3 = ConvertParma<string>(formula.FormulaParam, "CheckTypeValueList", 1);
			List<NameValueModel> list = new List<NameValueModel>();
			if (!string.IsNullOrWhiteSpace(text3))
			{
				List<string> list2 = text3.Split('|').ToList();
				if (list2.Count() > 0)
				{
					foreach (string item in list2)
					{
						string[] array = item.Split(':');
						if (array.Length >= 2)
						{
							int num = -1;
							if (int.TryParse(array[0], out num))
							{
								string[] array2 = array[1].Split(',');
								string[] array3 = array2;
								foreach (string mValue in array3)
								{
									NameValueModel nameValueModel = new NameValueModel();
									nameValueModel.MName = num.ToString();
									nameValueModel.MValue = mValue;
									list.Add(nameValueModel);
								}
							}
						}
					}
				}
			}
			gLBalanceListFilterModel.CheckTypeValueList = ((list.Count() > 0) ? list : null);
			gLBalanceListFilterModel.IncludeCheckType = (gLBalanceListFilterModel.CheckTypeValueList != null);
			formula.FormulaDataType = ConvertParma<int>(formula.FormulaParam, "FormulaDataType", 3);
			gLBalanceListFilterModel.FormaluDataType = formula.FormulaDataType;
			gLBalanceListFilterModel.ExcludeCLPVoucher = (formula.FormulaDataType == 7 || formula.FormulaDataType == 8);
			gLBalanceListFilterModel.PageIndex = 1;
			gLBalanceListFilterModel.PageSize = 2147483647;
			gLBalanceListFilterModel.Status = "1";
			return gLBalanceListFilterModel;
		}

		public IVInvoiceListFilterModel AnalysisMegiInvoice(BatchFormaluModel formula)
		{
			IVInvoiceListFilterModel iVInvoiceListFilterModel = new IVInvoiceListFilterModel();
			iVInvoiceListFilterModel.MStartDate = Convert.ToDateTime(ConvertParma<DateTime>(formula.FormulaParam, "StartDate", 1));
			iVInvoiceListFilterModel.MEndDate = ConvertParma<DateTime>(formula.FormulaParam, "EndDate", 1);
			List<string> list = ConvertParma<List<string>>(formula.FormulaParam, "Track", 2);
			if (list != null && list.Count() == 5)
			{
				iVInvoiceListFilterModel.MTrackItem1 = list[0];
				iVInvoiceListFilterModel.MTrackItem2 = list[1];
				iVInvoiceListFilterModel.MTrackItem3 = list[2];
				iVInvoiceListFilterModel.MTrackItem4 = list[3];
				iVInvoiceListFilterModel.MTrackItem5 = list[4];
			}
			iVInvoiceListFilterModel.MContactID = ConvertParma<string>(formula.FormulaParam, "Customer", 1);
			formula.FormulaDataType = ConvertParma<int>(formula.FormulaParam, "FormulaDataType", 3);
			iVInvoiceListFilterModel.MType = "Invoice_Sales";
			int formulaDataType = formula.FormulaDataType;
			if ((uint)formulaDataType <= 2u)
			{
				iVInvoiceListFilterModel.MStatus = 0;
			}
			else
			{
				iVInvoiceListFilterModel.MStatus = formula.FormulaDataType;
			}
			iVInvoiceListFilterModel.PageIndex = 1;
			iVInvoiceListFilterModel.PageSize = 2147483647;
			iVInvoiceListFilterModel.MSearchWithin = (IVInvoiceSearchWithinEnum)ConvertParma<int>(formula.FormulaParam, "SearchWithIn", 3);
			return iVInvoiceListFilterModel;
		}

		private T ConvertParma<T>(object param, string property, int classType = 1)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(param);
			PropertyDescriptor propertyDescriptor = properties.Find(property, true);
			object value = propertyDescriptor.GetValue(param);
			value = ((value != null) ? value.ToString() : "");
			//T val = default(T);
			if (classType == 2 && value != null)
			{
				string text = value.ToString();
				return (T)Convert.ChangeType(text.Split(',').ToList(), typeof(T));
			}
			return (T)Convert.ChangeType(value, typeof(T));
		}
	}
}
