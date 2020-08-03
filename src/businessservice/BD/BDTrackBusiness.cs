using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDTrackBusiness : APIBusinessBase<BDTrackModel>, IBDTrackBusiness, IDataContract<BDTrackModel>, IBasicBusiness<BDTrackModel>
	{
		private List<BDTrackModel> _trackList;

		private int _trackIdx;

		private BDTrackEntryModel _trackEntryModel;

		private bool _isExceedLimitTrackCount = false;

		private List<BDTrackModel> _trackDataPool;

		private readonly BDTrackRepository dal = new BDTrackRepository();

		protected override void OnGetBefore(MContext ctx, GetParam param)
		{
			param.ModifiedSince = DateTime.MinValue;
			param.IgnoreModifiedSince = true;
		}

		protected override DataGridJson<BDTrackModel> OnGet(MContext ctx, GetParam param)
		{
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_trackDataPool = instance.TrackingCategories;
			return dal.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, BDTrackModel model)
		{
			model.MTrackingOptionID = null;
			BDTrackModel bDTrackModel = _trackDataPool.FirstOrDefault((BDTrackModel f) => f.MItemID == model.MItemID);
			bool flag = IsOptionsAction(ctx, param.MGroupName);
			if (bDTrackModel != null)
			{
				model.MName = bDTrackModel.MName;
				if (flag)
				{
					model.MIsOnlyReturnEntry = true;
					if (!string.IsNullOrWhiteSpace(param.MGroupID))
					{
						model.MEntryList = (from f in model.MEntryList
						where f.MEntryID == param.MGroupID
						select f).ToList();
						base.CheckRequestResourceExist(ctx, model.MEntryList.Count == 0);
					}
				}
				else if (!param.IncludeDisabled)
				{
					model.MEntryList = (from f in model.MEntryList
					where f.MIsActive
					select f).ToList();
				}
				foreach (BDTrackEntryModel mEntry in model.MEntryList)
				{
					BDTrackEntryModel bDTrackEntryModel = bDTrackModel.MEntryList.FirstOrDefault((BDTrackEntryModel f) => f.MEntryID == mEntry.MTrackingOptionID);
					if (bDTrackEntryModel != null)
					{
						mEntry.MName = bDTrackEntryModel.MName;
					}
				}
				model.MEntryList = (from a in model.MEntryList
				orderby a.MModifyDate
				select a).ToList();
			}
		}

		protected override void OnPostBefore(MContext ctx, PostParam<BDTrackModel> param, APIDataPool dataPool)
		{
			_trackList = ModelInfoManager.GetDataModelList<BDTrackModel>(ctx, new SqlWhere(), false, true);
			_trackIdx = _trackList.Count;
		}

		protected override void OnPostValidate(MContext ctx, PostParam<BDTrackModel> param, APIDataPool dataPool, BDTrackModel model, bool isPut, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList)
		{
			if (IsOptionsAction(ctx, param.MGroupName))
			{
				_trackEntryModel = new BDTrackEntryModel();
				_trackEntryModel.MItemID = param.ElementID;
				model.MItemID = param.ElementID;
				_trackEntryModel.MEntryID = (param.IsPut ? string.Empty : model.MTrackingOptionID);
				_trackEntryModel.MName = model.MName;
				_trackEntryModel.MultiLanguage = model.MultiLanguage;
				_trackEntryModel.UpdateFieldList = model.UpdateFieldList;
				ValidateTrackEntry(ctx, param.IsPut, ref _trackEntryModel, _trackList, ref validNameList, ref updNameList);
				model.ValidationErrors.AddRange(_trackEntryModel.ValidationErrors);
			}
			else
			{
				ValidateTrack(ctx, param.IsPut, model, _trackList, ref validNameList, ref _trackIdx, ref updNameList);
			}
		}

		protected override List<CommandInfo> OnPostGetCmd(MContext ctx, PostParam<BDTrackModel> param, APIDataPool dataPool, BDTrackModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			bool flag = IsOptionsAction(ctx, param.MGroupName);
			if (flag)
			{
				list = ModelInfoManager.GetInsertOrUpdateCmd<BDTrackEntryModel>(ctx, _trackEntryModel, null, true);
				model.MTrackingOptionID = _trackEntryModel.MEntryID;
			}
			else
			{
				list = dal.GetNewTrackCommandList(model, model.MEntryList, ctx);
			}
			if (!param.IsPut)
			{
				if (flag && _trackEntryModel.IsNew)
				{
					BDTrackModel bDTrackModel = _trackList.FirstOrDefault((BDTrackModel f) => f.MItemID == _trackEntryModel.MItemID);
					bDTrackModel?.MEntryList.Add(_trackEntryModel);
				}
				else if (model.IsNew)
				{
					_trackList.Add(model);
				}
			}
			return list;
		}

		private BDTrackEntryModel ValidateTrackEntry(MContext ctx, bool isPut, ref BDTrackEntryModel option, List<BDTrackModel> trackList, ref Dictionary<string, List<string>> validOptionNameList, ref Dictionary<string, string> updNameList)
		{
			string trackId = option.MItemID;
			string mEntryID = option.MEntryID;
			BDTrackModel bDTrackModel = trackList.FirstOrDefault((BDTrackModel f) => f.MItemID == trackId);
			BDTrackEntryModel bDTrackEntryModel = null;
			base.CheckRequestResourceExist(ctx, bDTrackModel == null);
			bool flag = string.IsNullOrWhiteSpace(option.MEntryID) || !IsEntryIDExist(trackList, option.MEntryID);
			if (option.Validate(ctx, option.UpdateFieldList.Contains("MName") && string.IsNullOrEmpty(option.MName), "FieldEmpty", "“{0}”不能为空。", LangModule.Common, "Name"))
			{
				bDTrackEntryModel = APIValidator.MatchByIdThenName(ctx, isPut, option, bDTrackModel.MEntryList, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackOptionNameUsed", "跟踪项选项名称“{0}”在系统中已经存在。"), ref updNameList, "MName", "MEntryID", true, BasicDataReferenceTypeEnum.NotReference, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "UpdateDisabledTrackOptionError", "禁用状态的跟踪项选项不能被更新。"), true, null);
				APIValidator.ValidateDuplicateName(ctx, option, COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.BD, "TrackOptionNameUsed", "跟踪项选项名称“{0}”在系统中已经存在。", option.MName), ref validOptionNameList, ref updNameList, "MName", "MEntryID");
			}
			if (!string.IsNullOrWhiteSpace(bDTrackEntryModel?.MEntryID))
			{
				APIValidator.SetMatchMultiFieldInfo(option.MultiLanguage, bDTrackEntryModel.MEntryID, bDTrackEntryModel.MultiLanguage);
			}
			option.Validate(ctx, flag && !option.UpdateFieldList.Contains("MName"), "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Name");
			return bDTrackEntryModel;
		}

		private bool IsEntryIDExist(List<BDTrackModel> trackList, string entryId)
		{
			bool flag = false;
			foreach (BDTrackModel track in trackList)
			{
				if (track.MEntryList != null && track.MEntryList.Any())
				{
					foreach (BDTrackEntryModel mEntry in track.MEntryList)
					{
						if (mEntry.MEntryID == entryId)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		private bool ValidateTrack(MContext ctx, bool isPut, BDTrackModel model, List<BDTrackModel> trackList, ref Dictionary<string, List<string>> validNameList, ref int idx, ref Dictionary<string, string> updNameList)
		{
			bool flag = string.IsNullOrWhiteSpace(model.MItemID) || !trackList.Any((BDTrackModel f) => f.MItemID == model.MItemID);
			model.Validate(ctx, model.UpdateFieldList.Contains("MName") && string.IsNullOrEmpty(model.MName), "FieldEmpty", "“{0}”不能为空。", LangModule.Common, "Name");
			model.Validate(ctx, flag && !model.UpdateFieldList.Contains("MName"), "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Name");
			if (!string.IsNullOrWhiteSpace(model.MName))
			{
				model.Validate(ctx, model.MName.Contains('/'), "TrackNameIncludeIllegalChar", "跟踪项名称不能包含字符“/”。", LangModule.BD);
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackNameUsed", "跟踪项类型名称“{0}”在系统中已经存在。");
				APIValidator.MatchByIdThenName(ctx, isPut, model, trackList, text, ref updNameList, "MName", "MItemID", true, BasicDataReferenceTypeEnum.NotReference, null, false, null);
				APIValidator.ValidateDuplicateName(ctx, model, text, ref validNameList, ref updNameList, "MName", "MItemID");
			}
			model.Validate(ctx, model.MEntryList != null && model.MEntryList.Any((BDTrackEntryModel f) => f.UpdateFieldList != null && f.UpdateFieldList.Any()), "TrackOptionCreOrUpdWayError", "提供的内容包括跟踪项类型下的明细跟踪项，请使跟踪项选项子接口做跟踪项选项的创建和更新。", LangModule.BD);
			if (string.IsNullOrWhiteSpace(model.MItemID) && (!model.ValidationErrors.Any() || _isExceedLimitTrackCount))
			{
				if (_isExceedLimitTrackCount)
				{
					model.ValidationErrors.Clear();
				}
				idx++;
				if (!model.Validate(ctx, idx > 5, "TrackCountExceedLimit", "已经达到了最多5个跟踪项类型的限制。", LangModule.BD))
				{
					_isExceedLimitTrackCount = true;
				}
			}
			model.MCreateDate = ctx.DateNow.AddSeconds((double)idx);
			return !model.ValidationErrors.Any();
		}

		protected override void OnDeleteEntryValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, BDTrackModel model, ref bool isExist)
		{
			if (IsOptionsAction(ctx, param.MGroupName) && model != null)
			{
				isExist = model.MEntryList.Exists((BDTrackEntryModel f) => f.MEntryID == param.MGroupID);
			}
		}

		protected override void OnDeleteValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, BDTrackModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (IsOptionsAction(ctx, param.MGroupName))
			{
				if (!string.IsNullOrWhiteSpace(param.MGroupID))
				{
					operationResult = BDRepository.IsCanDelete(ctx, "TrackEntry", param.MGroupID);
					BDTrackEntryModel bDTrackEntryModel = model.MEntryList.FirstOrDefault((BDTrackEntryModel f) => f.MEntryID == param.MGroupID);
					model.Validate(ctx, !operationResult.Success, "TrackOptionUsed", "跟踪项选项“{0}”已被使用，不能被删除。", LangModule.BD, bDTrackEntryModel.MName);
					model.Validate(ctx, !bDTrackEntryModel.MIsActive, "DeleteTrackOptionDisabled", "禁用状态跟踪项选项不能被删除。", LangModule.BD);
				}
			}
			else if (APIBusinessBase<BDTrackModel>.isAPITestMode)
			{
				operationResult = BDRepository.IsCanDelete(ctx, "Track", param.ElementID);
				if (!operationResult.Success)
				{
					model.ValidationErrors.Add(new ValidationError(operationResult.Message));
				}
			}
		}

		public override void CheckRequestMethodImplemented(MContext ctx, string methodName, string groupName)
		{
			if (methodName == "OnDeleteGetCmd" && string.IsNullOrWhiteSpace(groupName) && !APIBusinessBase<BDTrackModel>.isAPITestMode)
			{
				base.ThrowNotImplementedException(ctx);
			}
			base.CheckRequestMethodImplemented(ctx, methodName, groupName);
		}

		protected override List<CommandInfo> OnDeleteGetCmd(MContext ctx, DeleteParam param, APIDataPool dataPool, BDTrackModel model)
		{
			if (IsOptionsAction(ctx, param.MGroupName))
			{
				return ModelInfoManager.GetDeleteCmd<BDTrackEntryModel>(ctx, param.MGroupID);
			}
			if (APIBusinessBase<BDTrackModel>.isAPITestMode)
			{
				return ModelInfoManager.GetDeleteCmd<BDTrackModel>(ctx, param.ElementID);
			}
			return new List<CommandInfo>();
		}

		protected override void OnPostAfter(MContext ctx, PostParam<BDTrackModel> param, APIDataPool dataPool)
		{
			param.IgnoreModifiedSince = true;
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MItemID)
			select t.MItemID).ToList();
			list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MItemID) && t.ValidationErrors.Count == 0
			select t.MItemID).ToList();
			if (list.Count > 0)
			{
				GetParam param2 = new GetParam
				{
					MOrgID = ctx.MOrgID,
					MUserID = ctx.MUserID,
					IncludeDetail = new bool?(true)
				};
				base.SetWhereString(param2, "TrackingCategoryID", list, true);
				IBasicBusiness<BDTrackModel> basicBusiness = new BDTrackBusiness();
				DataGridJson<BDTrackModel> dataGridJson = basicBusiness.Get(ctx, param2);
				if (IsOptionsAction(ctx, param.MGroupName))
				{
					SetReturnOptions(dataGridJson.rows, param);
				}
				else
				{
					List<BDTrackModel> list2 = new List<BDTrackModel>();
					for (int i = 0; i < param.DataList.Count; i++)
					{
						BDTrackModel model = param.DataList[i];
						if (model.ValidationErrors.Count > 0)
						{
							list2.Add(model);
						}
						else
						{
							BDTrackModel item = dataGridJson.rows.FirstOrDefault((BDTrackModel a) => a.MItemID == model.MItemID);
							list2.Add(item);
						}
					}
					param.DataList = list2;
				}
			}
		}

		private bool IsOptionsAction(MContext ctx, string groupName)
		{
			bool flag = false;
			if (!string.IsNullOrEmpty(groupName))
			{
				flag = groupName.Equals("options", StringComparison.CurrentCultureIgnoreCase);
				base.CheckRequestResourceExist(ctx, !flag);
			}
			return flag;
		}

		private void SetReturnOptions(List<BDTrackModel> trackList, PostParam<BDTrackModel> param)
		{
			List<BDTrackModel> dataList = param.DataList;
			param.DataList = trackList;
			param.DataList[0].MIsOnlyReturnEntry = true;
			List<BDTrackEntryModel> list = new List<BDTrackEntryModel>();
			foreach (BDTrackModel item in dataList)
			{
				if (item.ValidationErrors.Any())
				{
					list.Add(new BDTrackEntryModel
					{
						MName = item.MName,
						ValidationErrors = item.ValidationErrors
					});
				}
				else
				{
					list.Add(trackList[0].MEntryList.FirstOrDefault((BDTrackEntryModel f) => f.MEntryID == item.MTrackingOptionID));
				}
			}
			trackList[0].MEntryList = list;
		}

		public List<BDTrackModel> GetList(MContext ctx, string where)
		{
			return dal.GetList(where, ctx, null, null, false, true, "");
		}

		public List<BDTrackModel> GetListByName(MContext ctx, List<string> nameList)
		{
			return dal.GetList(string.Empty, ctx, null, nameList, false, true, "");
		}

		public List<string> GetTrackNameList(MContext ctx)
		{
			List<BDTrackModel> trackNameList = dal.GetTrackNameList(ctx, false);
			return (from f in trackNameList
			select f.MName).ToList();
		}

		public List<string> GetTrackIdList(MContext ctx)
		{
			List<BDTrackModel> trackNameList = dal.GetTrackNameList(ctx, false);
			return (from f in trackNameList
			select f.MItemID).ToList();
		}

		public List<NameValueModel> GetTrackBasicInfo(MContext ctx, string orgIds = null)
		{
			return dal.GetTrackBasicInfo(ctx, orgIds, false, true);
		}

		public OperationResult SaveList(MContext ctx, string[] array)
		{
			OperationResult operationResult = new OperationResult();
			if (GetTrackCount(ctx) < 5)
			{
				operationResult = dal.SaveList(array, ctx);
			}
			else
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackCountThan5", "Tracking is not greater than 5");
			}
			return operationResult;
		}

		public OperationResult SaveTrack(MContext ctx, BDTrackModel trcModel, List<BDTrackEntryModel> optionsModels)
		{
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrEmpty(trcModel.MItemID) && GetTrackCount(ctx) >= 5)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackCountThan5", "Tracking is not greater than 5");
				return operationResult;
			}
			if (string.IsNullOrEmpty(trcModel.MItemID) && ModelInfoManager.IsLangColumnValueExists<BDTrackModel>(ctx, "MName", trcModel.MultiLanguage, trcModel.MItemID, "", "", false))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackNameIsExist", "跟踪项名称已经存在，请使用其他名称！");
				return operationResult;
			}
			if (JudgeTrackNameIncludeIllegalChar(ctx, trcModel))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackNameIncludeIllegalChar", "跟踪项名称不能包含字符/！");
				return operationResult;
			}
			if (optionsModels != null && JudgeTrackEntryNameIsExist(ctx, optionsModels, trcModel.MItemID))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackOptionNameIsExist", "跟踪项子项存在重复，请使用其他名称!");
				return operationResult;
			}
			operationResult = dal.SaveList(trcModel, optionsModels, ctx);
			if (operationResult.Success && optionsModels != null && optionsModels.Count == 1)
			{
				operationResult.ObjectID = optionsModels[0].MEntryID;
			}
			return operationResult;
		}

		private bool JudgeTrackNameIsExist(MContext ctx, BDTrackModel trackModel)
		{
			//bool flag = false;
			List<string> trackNameList = GetTrackNameList(trackModel);
			return dal.IsExistName(ctx, trackModel.MItemID, trackNameList, false, "");
		}

		private bool JudgeTrackNameIncludeIllegalChar(MContext ctx, BDTrackModel track)
		{
			List<string> trackNameList = GetTrackNameList(track);
			return trackNameList.Any((string x) => x.IndexOf('/') >= 0);
		}

		private List<string> GetTrackNameList(BDTrackModel track)
		{
			List<MultiLanguageField> trackMultiNameList = new List<MultiLanguageField>();
			if (track.MultiLanguage != null)
			{
				track.MultiLanguage.ForEach(delegate(MultiLanguageFieldList x)
				{
					if (x.MMultiLanguageField != null)
					{
						trackMultiNameList.AddRange(x.MMultiLanguageField);
					}
				});
			}
			return (from x in trackMultiNameList
			where !string.IsNullOrWhiteSpace(x.MValue)
			select x.MValue).ToList();
		}

		private bool JudgeTrackEntryNameIsExist(MContext ctx, List<BDTrackEntryModel> trackEntryList, string trackId)
		{
			List<string> existedNameList = new List<string>();
			Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
			dic.Add("0x0009", new List<string>());
			dic.Add("0x7804", new List<string>());
			dic.Add("0x7C04", new List<string>());
			foreach (BDTrackEntryModel trackEntry in trackEntryList)
			{
				if (trackEntry.MultiLanguage != null)
				{
					trackEntry.MultiLanguage.ForEach(delegate(MultiLanguageFieldList x)
					{
						if (x.MMultiLanguageField != null)
						{
							x.MMultiLanguageField.ForEach(delegate(MultiLanguageField y)
							{
								string mLocaleID = y.MLocaleID;
								string name = y.MValue;
								if (!string.IsNullOrWhiteSpace(name) && dic[mLocaleID].Exists((string a) => a == name))
								{
									existedNameList.Add(name);
								}
								else if (!string.IsNullOrWhiteSpace(name))
								{
									dic[mLocaleID].Add(name);
								}
							});
						}
					});
				}
			}
			if (existedNameList.Any())
			{
				return true;
			}
			if (string.IsNullOrWhiteSpace(trackId))
			{
				return false;
			}
			return (from model in trackEntryList
			select ModelInfoManager.IsLangColumnValueExists<BDTrackEntryModel>(ctx, "MName", model.MultiLanguage, model.MEntryID, "MItemID", model.MItemID, false)).Any((bool isExistsName) => isExistsName) || existedNameList.Any();
		}

		public List<CommandInfo> GetNewTrackCommandList(MContext ctx, List<BDTrackModel> trackList, ref string errMsg)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			int trackCount = GetTrackCount(ctx);
			if (trackList.Count() + trackCount > 5)
			{
				errMsg = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ImportTrackError", "当前组织已经有{0}个跟踪项，您最多只能再新增{1}个跟踪项！（正在新增的跟踪项：{2}）"), trackCount, 5 - trackCount, string.Join(",", from f in trackList
				select f.MName));
				return list;
			}
			foreach (BDTrackModel track in trackList)
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDTrackModel>(ctx, track, null, true));
			}
			return list;
		}

		public OperationResult updateEdit(MContext ctx, BDTrackModel info)
		{
			OperationResult operationResult = new OperationResult();
			info.MOrgID = ctx.MOrgID;
			if (JudgeTrackNameIncludeIllegalChar(ctx, info))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackNameIncludeIllegalChar", "跟踪项名称不能包含字符/！");
				return operationResult;
			}
			if (ModelInfoManager.IsLangColumnValueExists<BDTrackModel>(ctx, "MName", info.MultiLanguage, info.MItemID, "", "", false))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackNameIsExist", "跟踪项名称已经存在，请使用其他名称!");
			}
			else
			{
				operationResult.Success = true;
				dal.updateEdit(info, ctx);
			}
			return operationResult;
		}

		public OperationResult updateOptEdit(MContext ctx, BDTrackModel info, List<BDTrackEntryModel> entryList)
		{
			OperationResult operationResult = new OperationResult();
			if (JudgeTrackEntryNameIsExist(ctx, entryList, info.MItemID))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackNameIsExist", "跟踪项名称已经存在，请使用其他名称！");
			}
			else
			{
				operationResult.Success = true;
				dal.updateOptEdit(info, ctx);
			}
			return operationResult;
		}

		public OperationResult IsCanDeleteOrInactive(MContext ctx, BDTrackModel info)
		{
			return BDRepository.IsCanDelete(ctx, "TrackEntry", info.MEntryID);
		}

		public OperationResult trackDel(MContext ctx, BDTrackModel info)
		{
			List<string> list = new List<string>();
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Track", info.MItemID);
			if (operationResult.Success)
			{
				operationResult = dal.trackDel(info, ctx);
			}
			return operationResult;
		}

		public OperationResult trackOptDel(MContext ctx, BDTrackModel info)
		{
			return dal.trackOptDel(info, ctx);
		}

		public int GetTrackCount(MContext ctx)
		{
			//int num = 0;
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MIsDelete", 0);
			sqlWhere.Equal(" MIsActive ", 1);
			List<BDTrackModel> modelList = dal.GetModelList(ctx, sqlWhere, false);
			return (modelList != null) ? modelList.Count : 0;
		}

		public OperationResult ArchiveTrackEntry(MContext ctx, string entryId, int status)
		{
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrWhiteSpace(entryId))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "CanNotSelectTrackEntry", "没有选择需要操作的跟踪项");
				return operationResult;
			}
			operationResult = dal.ArchiveTrackEntry(ctx, entryId, status);
			if (!operationResult.Success)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ArchiveTrackEntryFail", "跟踪项子项归档失败！");
			}
			return operationResult;
		}

		public BDTrackModel GetTrackModelIncludeEntry(MContext ctx, string trackId)
		{
			BDTrackModel result = new BDTrackModel();
			if (string.IsNullOrWhiteSpace(trackId))
			{
				return result;
			}
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MItemID", trackId);
			return ModelInfoManager.GetDataModelList<BDTrackModel>(ctx, sqlWhere, false, true).FirstOrDefault();
		}

		public List<string> TrimTrackPrefix(string[] importTrackNameList)
		{
			List<string> list = new List<string>();
			Dictionary<string, string> allText = COMMultiLangRepository.GetAllText(ServerHelper.MegiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.BD, "Tracking", "Tracking", "\\d?[：:]?")
			});
			string pattern = string.Join("|", allText.Values);
			foreach (string text in importTrackNameList)
			{
				list.Add((text == null) ? string.Empty : Regex.Replace(text, pattern, "").Trim());
			}
			return list;
		}

		public void CheckTrackExist<T>(MContext ctx, string[] importTrackNameList, List<T> entryList, List<IOValidationResultModel> validationResult)
		{
			if (importTrackNameList != null && importTrackNameList != null)
			{
				importTrackNameList = TrimTrackPrefix(importTrackNameList).ToArray();
				List<BDTrackModel> list = (from a in GetListByName(ctx, (from f in importTrackNameList
				where !string.IsNullOrWhiteSpace(f)
				select f).ToList())
				where a.MLocaleID == ctx.MLCID
				select a into p
				group p by new
				{
					p.MName,
					p.MEntryName
				} into g
				select Enumerable.First<BDTrackModel>((IEnumerable<BDTrackModel>)g)).ToList();
				List<BDTrackModel> list2 = (from p in list
				group p by p.MName into g
				select Enumerable.First<BDTrackModel>((IEnumerable<BDTrackModel>)g)).ToList();
				int num = 0;
				foreach (T entry in entryList)
				{
					string text = ModelHelper.TryGetModelValue(entry, "MRowIndex");
					if (!string.IsNullOrWhiteSpace(text) && text != "0")
					{
						int.TryParse(text, out num);
					}
					for (int i = 0; i < importTrackNameList.Length; i++)
					{
						string trackCategory = importTrackNameList[i];
						if (!string.IsNullOrWhiteSpace(trackCategory))
						{
							string propName = "MTrackItem" + (i + 1);
							string entryName = ModelHelper.GetModelValue(entry, propName);
							if (!string.IsNullOrWhiteSpace(entryName))
							{
								BDTrackModel matchedTrack = list?.FirstOrDefault((BDTrackModel f) => !string.IsNullOrWhiteSpace(f.MName) && HttpUtility.HtmlDecode(f.MName).ToUpper().Trim() == trackCategory.ToUpper().Trim());
								string empty = string.Empty;
								if (matchedTrack != null && list2.Count > i && list2[i].MName.EqualsIgnoreCase(trackCategory))
								{
									string text2 = string.Empty;
									BDTrackModel bDTrackModel = list.FirstOrDefault((BDTrackModel f) => f.MItemID == matchedTrack.MItemID && !string.IsNullOrWhiteSpace(f.MEntryName) && HttpUtility.HtmlDecode(f.MEntryName).ToUpper().Trim() == entryName.ToUpper().Trim());
									if (bDTrackModel != null)
									{
										if (!bDTrackModel.MIsActive)
										{
											text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackOptionHasDisabled", "跟踪项选项：{0}已禁用！");
										}
										else
										{
											empty = bDTrackModel.MEntryID;
											ModelHelper.SetModelValue(entry, propName, bDTrackModel.MEntryID, null);
										}
									}
									else
									{
										text2 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TrackingCategoryOptionNotFound", "The Tracking Category options:{0} can't be found!(Tracking Category:{1})"), "{0}", trackCategory);
									}
									if (!string.IsNullOrWhiteSpace(text2))
									{
										validationResult.Add(new IOValidationResultModel
										{
											FieldType = IOValidationTypeEnum.TrackOption,
											FieldValue = entryName,
											Message = text2,
											RowIndex = num
										});
									}
								}
								else
								{
									validationResult.Add(new IOValidationResultModel
									{
										FieldType = IOValidationTypeEnum.TrackCategory,
										FieldValue = entryName,
										Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TrackingCategoryNotFound", "The Tracking Category:{0} can't be found!"),
										RowIndex = num
									});
								}
							}
						}
					}
					num++;
				}
			}
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDTrackModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDTrackModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public BDTrackModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BDTrackModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BDTrackModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDTrackModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public List<BDTrackModel> Get(MContext ctx, string filterTrackId, bool ignoreLocale, GetParam param = null)
		{
			List<BDTrackModel> list = new List<BDTrackModel>();
			string strWhere = string.Empty;
			if (param != null && param.ModifiedSince > DateTime.MinValue)
			{
				strWhere = string.Format(" t1.MModifyDate>'{0}' ", param.ModifiedSince.ToString("yyyy-MM-dd"));
			}
			List<BDTrackModel> list2 = dal.GetList(strWhere, ctx, null, null, ignoreLocale, true, filterTrackId);
			if (list2 == null || list2.Count == 0)
			{
				return list;
			}
			List<string> list3 = (from t in list2
			select t.MItemID).Distinct().ToList();
			List<string> list4 = (from t in list2
			select t.MLocaleID).Distinct().ToList();
			foreach (string item in list3)
			{
				BDTrackModel bDTrackModel = list2.FirstOrDefault((BDTrackModel t) => t.MItemID == item && t.MLocaleID == ctx.MLCID);
				if (bDTrackModel == null)
				{
					bDTrackModel = list2.FirstOrDefault((BDTrackModel t) => t.MItemID == item);
				}
				if (bDTrackModel != null)
				{
					foreach (string item2 in list4)
					{
						BDTrackModel bDTrackModel2 = list2.FirstOrDefault((BDTrackModel t) => t.MItemID == item && t.MLocaleID == item2);
						if (bDTrackModel2 != null)
						{
							bDTrackModel.AddLang("MName", item2, bDTrackModel2.MName);
						}
					}
					List<BDTrackModel> list5 = (from t in list2
					where t.MItemID == item && t.MLocaleID == ctx.MLCID
					orderby t.MName
					select t).ToList();
					bDTrackModel.MEntryList = new List<BDTrackEntryModel>();
					if (list5 != null && list5.Count > 0)
					{
						foreach (BDTrackModel item3 in list5)
						{
							BDTrackEntryModel bDTrackEntryModel = new BDTrackEntryModel
							{
								MEntryID = item3.MEntryID,
								MName = item3.MEntryName,
								MItemID = item
							};
							foreach (string item4 in list4)
							{
								BDTrackModel bDTrackModel3 = list2.FirstOrDefault((BDTrackModel t) => t.MItemID == item && t.MEntryID == item3.MEntryID && t.MLocaleID == item4);
								if (bDTrackModel3 != null)
								{
									bDTrackEntryModel.AddLang("MName", item4, bDTrackModel3.MEntryName);
								}
							}
							bDTrackModel.MEntryList.Add(bDTrackEntryModel);
						}
					}
					list.Add(bDTrackModel);
				}
			}
			return list;
		}

		private List<BDTrackModel> Post(MContext ctx, List<BDTrackModel> list)
		{
			List<BDTrackModel> list2 = Get(ctx, "", true, null);
			int count = list2.Count;
			int num = list.Count((BDTrackModel t) => string.IsNullOrEmpty(t.MItemID));
			if (count + num > 5)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackCountThan5", "Tracking is not greater than 5");
				foreach (BDTrackModel item in list)
				{
					item.ValidationErrors.Add(new ValidationError(text));
				}
				return list;
			}
			List<BDTrackModel> list3 = new List<BDTrackModel>();
			List<BDTrackModel> list4 = new List<BDTrackModel>();
			for (int i = 0; i < list.Count; i++)
			{
				bool flag = true;
				BDTrackModel bDTrackModel = list[i];
				MultiLanguageFieldList multiLanguageFieldList = (from t in bDTrackModel.MultiLanguage
				where t.MFieldName == "MName"
				select t).FirstOrDefault();
				if (multiLanguageFieldList == null || multiLanguageFieldList.MMultiLanguageField == null || multiLanguageFieldList.MMultiLanguageField.Count == 0 || multiLanguageFieldList.MMultiLanguageField.Any((MultiLanguageField m) => string.IsNullOrWhiteSpace(m.MValue)))
				{
					flag = false;
					bDTrackModel.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NameCannotEmpty", "The name cannot empty.")));
				}
				bool flag2 = false;
				for (int j = 0; j < list.Count; j++)
				{
					if (i != j)
					{
						flag2 = IsNameExists(ctx, bDTrackModel, multiLanguageFieldList, list[j]);
					}
				}
				if (flag2)
				{
					flag = false;
				}
				foreach (BDTrackModel item2 in list2)
				{
					if ((string.IsNullOrEmpty(bDTrackModel.MItemID) || bDTrackModel.MItemID != item2.MItemID) && IsNameExists(ctx, bDTrackModel, multiLanguageFieldList, item2))
					{
						flag = false;
					}
				}
				if (!flag)
				{
					list4.Add(bDTrackModel);
				}
				else
				{
					list3.Add(bDTrackModel);
				}
			}
			if (list3.Any())
			{
				dal.Post(ctx, list3);
			}
			list3.AddRange(list4);
			return list3;
		}

		private bool IsNameExists(MContext ctx, BDTrackModel model, MultiLanguageFieldList nameLang, BDTrackModel checkModel)
		{
			MultiLanguageFieldList multiLanguageFieldList = (from t in checkModel.MultiLanguage
			where t.MFieldName == "MName"
			select t).FirstOrDefault();
			if (multiLanguageFieldList == null || multiLanguageFieldList.MMultiLanguageField == null || multiLanguageFieldList.MMultiLanguageField.Count == 0)
			{
				return false;
			}
			bool result = false;
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NameExists", "The name exists.");
			for (int i = 0; i < nameLang.MMultiLanguageField.Count; i++)
			{
				MultiLanguageField field = nameLang.MMultiLanguageField[i];
				MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.FirstOrDefault((MultiLanguageField t) => t.MValue == field.MValue && t.MLocaleID == field.MLocaleID);
				if (multiLanguageField != null)
				{
					model.ValidationErrors.Add(new ValidationError(text));
					result = true;
				}
			}
			return result;
		}

		public BDTrackEntryModel GetInsertTrackEntryModel(MContext ctx, string trackId, string entryName, List<BASLangModel> languageList)
		{
			BDTrackEntryModel bDTrackEntryModel = new BDTrackEntryModel();
			bDTrackEntryModel.MEntryID = UUIDHelper.GetGuid();
			bDTrackEntryModel.IsNew = true;
			bDTrackEntryModel.MIsActive = true;
			bDTrackEntryModel.MItemID = trackId;
			bDTrackEntryModel.MName = entryName;
			if (languageList == null)
			{
				languageList = new BASLangBusiness().GetOrgLangList(ctx);
			}
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
			multiLanguageFieldList.MMultiLanguageField = new List<MultiLanguageField>();
			multiLanguageFieldList.MFieldName = "MName";
			foreach (BASLangModel language in languageList)
			{
				MultiLanguageField multiLanguageField = new MultiLanguageField();
				multiLanguageField.MLocaleID = language.LangID;
				multiLanguageField.MOrgID = ctx.MOrgID;
				multiLanguageField.MValue = entryName;
				multiLanguageFieldList.MMultiLanguageField.Add(multiLanguageField);
			}
			bDTrackEntryModel.MultiLanguage = new List<MultiLanguageFieldList>();
			bDTrackEntryModel.MultiLanguage.Add(multiLanguageFieldList);
			return bDTrackEntryModel;
		}
	}
}
