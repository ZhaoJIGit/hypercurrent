using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.FC;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FC.Controllers
{
	public class FCCashCodingModuleController : GoControllerBase
	{
		private IFCCashCodingModule _fcCashCoding = null;

		private IBDContacts _bdCont = null;

		public FCCashCodingModuleController(IFCCashCodingModule fcCashCoding, IBDContacts bdContact)
		{
			_fcCashCoding = fcCashCoding;
			_bdCont = bdContact;
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult GetListByCode(string code)
		{
			MActionResult<List<FCCashCodingModuleListModel>> listByCode = _fcCashCoding.GetListByCode(code, null);
			return base.Json(listByCode);
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetCashCodingByPageList(FCCashCodingModuleListFilter filter)
		{
			MActionResult<DataGridJson<FCCashCodingModuleListModel>> cashCodingByPageList = _fcCashCoding.GetCashCodingByPageList(filter, null);
			return base.Json(cashCodingByPageList);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateCashCodingModule(FCCashCodingModuleModel model)
		{
			MActionResult<OperationResult> data = _fcCashCoding.UpdateCashCodingModuleModel(model, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult DeleteCashCodingModule(List<string> pkIDs)
		{
			MActionResult<OperationResult> data = _fcCashCoding.DeleteModels(pkIDs, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public ActionResult FCCashCodingEdit(string mid)
		{
			base.ViewData["MID"] = mid;
			List<BDContactItem> resultData = _bdCont.GetContactItemList(new BDContactsListFilter
			{
				MaxCount = 0
			}, null).ResultData;
			base.ViewData["ContactList"] = resultData;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult GetFCCashCodingModel(string mid)
		{
			MActionResult<FCCashCodingModuleModel> dataModel = _fcCashCoding.GetDataModel(mid, false, null);
			return base.Json(dataModel);
		}
	}
}
