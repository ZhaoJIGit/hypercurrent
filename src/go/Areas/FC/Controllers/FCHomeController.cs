using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.FC;
using JieNor.Megi.ServiceContract.GL;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FC.Controllers
{
	public class FCHomeController : GoControllerBase
	{
		private IGLVoucher _voucher;

		private IFCVoucherModule _voucherModule;

		private IFCCashCodingModule _cashCodingModule;

		private IFCFapiaoModule _fapiaoModule;

		public FCHomeController(IGLVoucher voucher, IFCVoucherModule voucherModule, IFCCashCodingModule cachCodingModule, IFCFapiaoModule fapiaoModule)
		{
			_voucher = voucher;
			_voucherModule = voucherModule;
			_cashCodingModule = cachCodingModule;
			_fapiaoModule = fapiaoModule;
		}

		public ActionResult Index()
		{
			return base.View();
		}

		public ActionResult FCHome()
		{
			return base.View();
		}

		public ActionResult GetModuleInfo()
		{
			int count = _voucherModule.GetVoucherModuleListWithNoEntry(null).ResultData.Count;
			int count2 = _cashCodingModule.GetCashCodingModuleListWithNoEntry(null).ResultData.Count;
			int resultData = _fapiaoModule.GetFapiaoModulePageListCount(null).ResultData;
			return base.Json(new List<int>
			{
				count,
				count2,
				resultData
			});
		}

		public ActionResult GetVoucherModulePageList(GLVoucherListFilterModel filter)
		{
			MActionResult<DataGridJson<FCVoucherModuleModel>> voucherModuleModelPageList = _voucherModule.GetVoucherModuleModelPageList(filter, null);
			return base.Json(voucherModuleModelPageList);
		}

		public ActionResult GetVoucherModuleListDataWithNoEntry()
		{
			MActionResult<List<FCVoucherModuleModel>> voucherModuleListWithNoEntry = _voucherModule.GetVoucherModuleListWithNoEntry(null);
			return base.Json(voucherModuleListWithNoEntry);
		}

		public ActionResult GetVoucherModuleListDataWithEntry(List<string> pkIDs)
		{
			MActionResult<List<FCVoucherModuleModel>> voucherModuleList = _voucherModule.GetVoucherModuleList(pkIDs, null);
			return base.Json(voucherModuleList);
		}

		public ActionResult UpdateVoucherModule(FCVoucherModuleModel model)
		{
			MActionResult<FCVoucherModuleModel> data = _voucherModule.UpdateVoucherModuleModel(model, null);
			return base.Json(data);
		}

		public ActionResult GetVoucherModule(string MItemID)
		{
			MActionResult<FCVoucherModuleModel> voucherModule = _voucherModule.GetVoucherModule(MItemID, null);
			return base.Json(voucherModule);
		}

		public ActionResult DeleteVoucherModules(List<string> pkIDs)
		{
			MActionResult<OperationResult> data = _voucherModule.DeleteModels(pkIDs, null);
			return base.Json(data);
		}

		public ActionResult SaveFapiaoModule(FCFapiaoModuleModel model)
		{
			return base.Json(_fapiaoModule.SaveFapiaoModule(model, null));
		}

		public ActionResult GetFapiaoModuleList(FCFastCodeFilterModel filter)
		{
			return base.Json(_fapiaoModule.GetFapiaoModuleList(filter, null));
		}

		public ActionResult GetFapiaoModulePageList(FCFastCodeFilterModel filter)
		{
			return base.Json(_fapiaoModule.GetFapiaoModulePageList(filter, null));
		}

		public ActionResult DeleteFapiaoModules(List<string> pkIDs)
		{
			return base.Json(_fapiaoModule.DeleteFapiaoModules(pkIDs, null));
		}
	}
}
