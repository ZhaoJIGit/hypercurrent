using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.REG;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class TaxRateController : GoControllerBase
	{
		private IBASOrganisation _org = null;

		private IREGTaxRate _taxRate = null;

		public TaxRateController(IBASOrganisation org, IREGTaxRate taxRate)
		{
			_org = org;
			_taxRate = taxRate;
			base.SetModule("setting");
		}

		[Permission("Setting", "View", "")]
		public ActionResult TaxRateList()
		{
			return base.View();
		}

		[Permission("Setting", "View", "")]
		public ActionResult TaxRateEdit(string id, bool? isSetup)
		{
			ViewData["RateCode"] = id;
			ViewData["IsSetup"] = (isSetup.HasValue ? isSetup.Value.ToString().ToLower() : "false");

			ViewBag.IsEnableGL = ContextHelper.MContext.MRegProgress == 15 && true;
			return base.MView();
		}

		public JsonResult GetTaxRateList()
		{
			MActionResult<List<REGTaxRateModel>> taxRateList = _taxRate.GetTaxRateList(false, null);
			taxRateList.ResultData = (taxRateList.ResultData ?? new List<REGTaxRateModel>());
			return base.Json(taxRateList);
		}

		[Permission("Setting", "View", "")]
		public JsonResult GetTaxRateListByPage(REGTaxTateListFilterModel filter)
		{
			MActionResult<DataGridJson<REGTaxRateModel>> taxRateListByPage = _taxRate.GetTaxRateListByPage(filter, false, null);
			return base.Json(taxRateListByPage);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult IsCanDeleteOrInactive(ParamBase param)
		{
			return base.Json(_taxRate.IsCanDeleteOrInactive(param, null));
		}

		[Permission("Setting", "Change", "")]
		public JsonResult SaveTaxRate(REGTaxRateModel paramModel)
		{
			paramModel.MAppID = base.MContext.MAppID;
			paramModel.MOrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _taxRate.InsertOrUpdate(paramModel, null, null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public JsonResult GetTaxRate(ParamBase param)
		{
			param.OrgID = base.MContext.MOrgID;
			MActionResult<REGTaxRateModel> dataModel = _taxRate.GetDataModel(param.KeyIDs, false, null);
			return base.Json(dataModel);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult DeleteTaxRateList(ParamBase param)
		{
			param.OrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _taxRate.Delete(param.KeyIDs, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult ArchiveTaxRate(ParamBase param, bool isRestore = false)
		{
			MActionResult<OperationResult> data = _taxRate.ArchiveTaxRate(param.KeyIDs, isRestore, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult GetUpdateTaxInfo(int changeTaxType)
		{
			MActionResult<OperationResult> updateTaxInfo = _taxRate.GetUpdateTaxInfo(changeTaxType, null);
			return base.Json(updateTaxInfo);
		}
	}
}
