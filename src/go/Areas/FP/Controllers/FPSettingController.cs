using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.FP;
using JieNor.Megi.ServiceContract.IO;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FP.Controllers
{
	public class FPSettingController : GoControllerBase
	{
		private IIOSolution _solution;

		private IFPSetting _fpSetting;

		public FPSettingController(IIOSolution solution, IFPSetting fpSetting)
		{
			_solution = solution;
			_fpSetting = fpSetting;
		}

		public ActionResult FPSetting()
		{
			return base.View();
		}

		public ActionResult FPSetAutoParams(string id)
		{
			MActionResult<FPImportTypeConfigModel> dataModel = _fpSetting.GetDataModel(id, false, null);
			ViewBag.DataModel = dataModel.ResultData;
			return base.View();
		}

		public JsonResult SaveSetAutoParams(FPImportTypeConfigModel model)
		{
			MActionResult<OperationResult> data = _fpSetting.SaveImportTypeConfig(model, null);
			return base.Json(data);
		}

		public JsonResult GetConfigList(int fpType)
		{
			MActionResult<List<IOConfigModel>> configList = _solution.GetConfigList(fpType, null);
			return base.Json(configList);
		}

		public JsonResult GetImportTypeConfig(int type)
		{
			MActionResult<List<FPImportTypeConfigModel>> modelList = _fpSetting.GetModelList(new SqlWhere(), false, null);
			return base.Json(modelList);
		}

		public JsonResult SaveFaPiaoSetting(FPConfigSettingSaveModel model)
		{
			MActionResult<OperationResult> data = _fpSetting.SaveFaPiaoSetting(model, null);
			return base.Json(data);
		}

		public JsonResult GetImportTypeConfigModel(int type, int fpType)
		{
			SqlWhere sqlWhere = new SqlWhere();
			if (type == 1)
			{
				List<string> list = new List<string>();
				int num = 0;
				list.Add(num.ToString());
				num = 1;
				list.Add(num.ToString());
				List<string> values = list;
				sqlWhere.Equal("MType", type);
				sqlWhere.In("MFPType", values);
				MActionResult<List<FPImportTypeConfigModel>> modelList = _fpSetting.GetModelList(sqlWhere, false, null);
				return base.Json(modelList);
			}
			sqlWhere.Equal("MType", type);
			sqlWhere.Equal("MFPType", fpType);
			MActionResult<List<FPImportTypeConfigModel>> modelList2 = _fpSetting.GetModelList(sqlWhere, false, null);
			return base.Json(modelList2);
		}
	}
}
