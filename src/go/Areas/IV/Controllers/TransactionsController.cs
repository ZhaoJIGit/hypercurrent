using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Param;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.IV;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class TransactionsController : GoControllerBase
	{
		private IIVTransactions _transaction = null;

		public TransactionsController(IIVTransactions transaction)
		{
			_transaction = transaction;
		}

		public JsonResult GetBankStatementsList(IVTransactionQueryParamModel param)
		{
			MActionResult<List<IVBankStatementsModel>> bankStatementsList = _transaction.GetBankStatementsList(param, null);
			return base.Json(bankStatementsList);
		}

		public JsonResult GetBankStatementModel(IVBankStatementsModel model)
		{
			MActionResult<IVBankStatementsModel> bankStatementModel = _transaction.GetBankStatementModel(model.MID, null);
			return base.Json(bankStatementModel);
		}

		public JsonResult GetStatementDetails(string id)
		{
			MActionResult<List<IVBankStatementViewModel>> bankStatementView = _transaction.GetBankStatementView(id, null);
			return base.Json(bankStatementView);
		}
	}
}
