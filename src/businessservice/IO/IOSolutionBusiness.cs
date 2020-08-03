using JieNor.Megi.BusinessContract.IO;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IO;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.BusinessService.IO
{
	public class IOSolutionBusiness : IIOSolutionBusiness
	{
		private readonly IOSolutionRepository _repSolution = new IOSolutionRepository();

		protected virtual int DefaultHeaderRowIndex => 1;

		public ImportResult ImportData(MContext ctx, ImportTypeEnum type, IOImportDataModel data)
		{
			if (data == null || data.EffectiveData == null || data.EffectiveData.Rows.Count == 0)
			{
				return new ImportResult
				{
					Success = false,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NoDataToImport", "That's no data to import!")
				};
			}
			IImport import = ImportFactory.CreateInstance(type, data);
			return import.ImportData(ctx, data);
		}

		public List<IOSolutionModel> GetSolutionList(MContext ctx, ImportTypeEnum importType)
		{
			return _repSolution.GetSolutionList(ctx, importType);
		}

		public OperationResult UpdateSolution(MContext ctx, IOSolutionModel model)
		{
			OperationResult operationResult = _repSolution.CheckNameExist(ctx, model.MTypeID, model.MItemID, model.MName);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			if (model.MHeaderRowIndex == 0)
			{
				model.MHeaderRowIndex = 1;
			}
			if (model.MDataRowIndex == 0)
			{
				model.MDataRowIndex = model.MHeaderRowIndex + 1;
			}
			List<IOSolutionConfigModel> list = _repSolution.GetSolutionConfigList(ctx, model.MItemID, model.MTypeID);
			if (model.MConfig != null)
			{
				IEnumerable<IOSolutionConfigModel> source = from p in list
				join t in model.MConfig on p.MConfigID equals t.MConfigID
				select new IOSolutionConfigModel
				{
					MItemID = p.MItemID,
					MConfigID = p.MConfigID,
					MOrgID = ctx.MOrgID,
					MTypeID = model.MTypeID,
					MColumnName = t.MColumnName
				};
				list = source.ToList();
			}
			model.MConfig = list;
			return _repSolution.UpdateSolution(ctx, model);
		}

		public IOSolutionModel GetSolutionModel(MContext ctx, string solutionId)
		{
			return _repSolution.GetSolutionModel(ctx, solutionId);
		}

		public List<IOConfigModel> GetConfigList(MContext ctx, int importType)
		{
			return _repSolution.GetConfigList(ctx, importType);
		}

		public List<IOSolutionConfigModel> GetSolutionConfigList(MContext ctx, ImportTypeEnum importType, string solutionId)
		{
			return _repSolution.GetSolutionConfigList(ctx, solutionId, Convert.ToInt32(importType));
		}

		public OperationResult SaveSolution(MContext ctx, IOImportDataModel model, bool isFromUpload = false)
		{
			IOSolutionModel iOSolutionModel = new IOSolutionModel();
			if (isFromUpload && !string.IsNullOrWhiteSpace(model.SolutionID))
			{
				iOSolutionModel = GetSolutionModel(ctx, model.SolutionID);
			}
			else
			{
				iOSolutionModel.MTypeID = Convert.ToInt32(model.TemplateType);
				iOSolutionModel.MItemID = model.SolutionID;
				iOSolutionModel.MHeaderRowIndex = model.MHeaderRowIndex;
				iOSolutionModel.MDataRowIndex = model.MDataRowIndex;
			}
			iOSolutionModel.MName = model.MSolutionName;
			iOSolutionModel.MConfig = (string.IsNullOrEmpty(model.SolutionID) ? GetSolutionConfigList(ctx, model) : model.MConfig);
			return UpdateSolution(ctx, iOSolutionModel);
		}

		private List<IOSolutionConfigModel> GetSolutionConfigList(MContext ctx, IOImportDataModel data)
		{
			List<IOSolutionConfigModel> list = new List<IOSolutionConfigModel>();
			List<IOConfigModel> configList = GetConfigList(ctx, (int)data.TemplateType);
			if (configList == null || configList.Count == 0)
			{
				return list;
			}
			DataRow dr = null;
			if (DefaultHeaderRowIndex > 0)
			{
				DataTable sourceData = data.SourceData;
				if (sourceData != null && sourceData.Rows.Count >= DefaultHeaderRowIndex)
				{
					dr = sourceData.Rows[DefaultHeaderRowIndex - 1];
				}
			}
			foreach (IOConfigModel item in configList)
			{
				IOSolutionConfigModel iOSolutionConfigModel = new IOSolutionConfigModel();
				iOSolutionConfigModel.MConfigID = item.MItemID;
				iOSolutionConfigModel.MTypeID = Convert.ToInt32((int)data.TemplateType);
				iOSolutionConfigModel.MColumnName = GetConfigColumnList(item, dr);
				list.Add(iOSolutionConfigModel);
			}
			return list;
		}

		private string GetConfigColumnList(IOConfigModel item, DataRow dr)
		{
			if (dr == null)
			{
				return string.Empty;
			}
			MultiLanguageFieldList multiLanguageFieldList = item.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
			if (multiLanguageFieldList?.MMultiLanguageField == null || multiLanguageFieldList.MMultiLanguageField.Count == 0)
			{
				return string.Empty;
			}
			foreach (string item2 in from col in dr.ItemArray
			where col != null && col != DBNull.Value
			select col.ToString() into str
			where !string.IsNullOrEmpty(str)
			select str)
			{
				if (!string.IsNullOrEmpty(item.MSimilarName))
				{
					string[] source = item.MSimilarName.Split(';');
					if (source.Count((string t) => t.ToLower() == item2.ToLower()) > 0)
					{
						return item2;
					}
				}
				int num = multiLanguageFieldList.MMultiLanguageField.Count((MultiLanguageField t) => !string.IsNullOrEmpty(t.MValue) && t.MValue.ToLower() == item2.ToLower());
				if (num > 0)
				{
					return item2;
				}
			}
			return string.Empty;
		}
	}
}
