using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.REG
{
	[ServiceContract]
	public interface IREGCurrency
	{
		[OperationContract]
		MActionResult<List<REGCurrencyModel>> GetCurrencyList(string accessToken = null);

		[OperationContract]
		MActionResult<List<REGCurrencyViewModel>> GetCurrencyViewList(DateTime? endTime, bool isIncludeBase = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertCurrency(REGCurrencyModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> RemoveCurrency(REGCurrencyModel model, string accessToken = null);

		[OperationContract]
		MActionResult<BASCurrencyViewModel> GetBaseCurrency(string accessToken = null);

		[OperationContract]
		MActionResult<List<GlobalCurrencyModel>> GetOrgCurrencyModel(string accessToken = null);

		[OperationContract]
		MActionResult<List<REGCurrencyViewModel>> GetAllCurrencyList(bool isIncludeBase = false, bool ignoreLocale = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<REGCurrencyViewModel>> GetBillCurrencyViewList(DateTime? endTime, bool isIncludeBase = false, string accessToken = null);
	}
}
