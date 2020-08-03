using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Formula;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.GL
{
	[ServiceContract]
	public interface IGLExcel
	{
		[OperationContract]
		MActionResult<List<GLVoucherModel>> GetVoucherListByFilter(GLBalanceListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLBalanceModel>> GetBalanceListByFilter(GLBalanceListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLBalanceModel>> GetBalanceListWithTrackByFilter(GLBalanceListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLVoucherModel>> GetTransferVoucherList(GLBalanceListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLInitBalanceModel>> GetInitBalanceList(GLBalanceListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountModel>> GetAccountList(string accessToken = null);

		[OperationContract]
		MActionResult<DateTime> GetGLBeginDate(string accessToken = null);

		[OperationContract]
		MActionResult<List<BatchFormaluModel>> RefreshFormula(List<BatchFormaluModel> formaluList, string accessToken = null);
	}
}
