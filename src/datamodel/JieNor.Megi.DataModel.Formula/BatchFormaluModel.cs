using JieNor.Megi.Core;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.Formula
{
	[DataContract]
	public class BatchFormaluModel
	{
		[DataMember]
		public string FormulaId
		{
			get;
			set;
		}

		[DataMember]
		public int FormulaType
		{
			get;
			set;
		}

		[DataMember]
		public string FormulaString
		{
			get;
			set;
		}

		[DataMember]
		public object FormulaParam
		{
			get;
			set;
		}

		[DataMember]
		public int FormulaDataType
		{
			get;
			set;
		}

		[DataMember]
		public decimal FormulaResult
		{
			get;
			set;
		}

		[DataMember]
		public string ApiModule
		{
			get;
			set;
		}

		public T GetFilterByFormula<T>()
		{
			T result = default(T);
			if (FormulaType == 0)
			{
				object obj = AnalysisMegiAcct();
				return (T)obj;
			}
			if (FormulaType == 1)
			{
				object obj2 = AnalysisMegiInvoice();
				return (T)obj2;
			}
			return result;
		}

		public GLBalanceListFilterModel AnalysisMegiAcct()
		{
			GLBalanceListFilterModel gLBalanceListFilterModel = new GLBalanceListFilterModel();
			gLBalanceListFilterModel.AccountIDS = ConvertParma<List<string>>(FormulaParam, "Account", 2);
			gLBalanceListFilterModel.AccountIDS = ((gLBalanceListFilterModel.AccountIDS != null) ? (from x in gLBalanceListFilterModel.AccountIDS
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList() : gLBalanceListFilterModel.AccountIDS);
			string text = ConvertParma<string>(FormulaParam, "StartDate", 1);
			gLBalanceListFilterModel.StartYear = ((!string.IsNullOrWhiteSpace(text)) ? Convert.ToInt32(text.Substring(0, 4)) : 0);
			gLBalanceListFilterModel.StartPeriod = ((!string.IsNullOrWhiteSpace(text)) ? Convert.ToInt32(text.Substring(4, 2)) : 0);
			gLBalanceListFilterModel.MCurrencyID = ConvertParma<string>(FormulaParam, "CurrencyID", 1);
			string text2 = ConvertParma<string>(FormulaParam, "EndDate", 1);
			gLBalanceListFilterModel.EndYear = ((!string.IsNullOrWhiteSpace(text)) ? Convert.ToInt32(text2.Substring(0, 4)) : 0);
			gLBalanceListFilterModel.EndPeriod = ((!string.IsNullOrWhiteSpace(text)) ? Convert.ToInt32(text2.Substring(4, 2)) : 0);
			string text3 = ConvertParma<string>(FormulaParam, "CheckTypeValueList", 1);
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
			FormulaDataType = ConvertParma<int>(FormulaParam, "FormulaDataType", 3);
			gLBalanceListFilterModel.FormaluDataType = FormulaDataType;
			gLBalanceListFilterModel.ExcludeCLPVoucher = (FormulaDataType == 7 || FormulaDataType == 8);
			gLBalanceListFilterModel.PageIndex = 1;
			gLBalanceListFilterModel.PageSize = 2147483647;
			gLBalanceListFilterModel.Status = "1";
			return gLBalanceListFilterModel;
		}

		public IVInvoiceListFilterModel AnalysisMegiInvoice()
		{
			IVInvoiceListFilterModel iVInvoiceListFilterModel = new IVInvoiceListFilterModel();
			iVInvoiceListFilterModel.MStartDate = Convert.ToDateTime(ConvertParma<DateTime>(FormulaParam, "StartDate", 1));
			iVInvoiceListFilterModel.MEndDate = ConvertParma<DateTime>(FormulaParam, "EndDate", 1);
			List<string> list = ConvertParma<List<string>>(FormulaParam, "Track", 2);
			if (list != null && list.Count() == 5)
			{
				iVInvoiceListFilterModel.MTrackItem1 = list[0];
				iVInvoiceListFilterModel.MTrackItem2 = list[1];
				iVInvoiceListFilterModel.MTrackItem3 = list[2];
				iVInvoiceListFilterModel.MTrackItem4 = list[3];
				iVInvoiceListFilterModel.MTrackItem5 = list[4];
			}
			iVInvoiceListFilterModel.MContactID = ConvertParma<string>(FormulaParam, "Customer", 1);
			FormulaDataType = ConvertParma<int>(FormulaParam, "FormulaDataType", 3);
			iVInvoiceListFilterModel.MType = "Invoice_Sales";
			int formulaDataType = FormulaDataType;
			if ((uint)formulaDataType <= 2u)
			{
				iVInvoiceListFilterModel.MStatus = 0;
			}
			else
			{
				iVInvoiceListFilterModel.MStatus = FormulaDataType;
			}
			iVInvoiceListFilterModel.PageIndex = 1;
			iVInvoiceListFilterModel.PageSize = 2147483647;
			iVInvoiceListFilterModel.MSearchWithin = (IVInvoiceSearchWithinEnum)ConvertParma<int>(FormulaParam, "SearchWithIn", 3);
			return iVInvoiceListFilterModel;
		}

		private T ConvertParma<T>(object param, string property, int classType = 1)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(param);
			PropertyDescriptor propertyDescriptor = properties.Find(property, true);
			object obj = propertyDescriptor.GetValue(param);
			if (obj != null)
			{
				obj = obj.ToString();
			}
			//T val = default(T);
			if (classType == 2 && obj != null)
			{
				string text = obj.ToString();
				return (T)Convert.ChangeType(text.Split(',').ToList(), typeof(T));
			}
			return (T)Convert.ChangeType(obj, typeof(T));
		}
	}
}
