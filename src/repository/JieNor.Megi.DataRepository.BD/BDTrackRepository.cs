using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDTrackRepository : DataServiceT<BDTrackModel>
	{
		private string multLangFieldSql = "\r\n            ,t2.MName\n            ,t4.MName as MEntryList_MName ";

		public string CommonSelect = "SELECT \n                t1.MItemID,\n                t1.MCreateDate,\n                t1.MItemID AS MTrackingCategoryID,\n                t1.MModifyDate,\n                t3.MEntryID as MEntryList_MTrackingOptionID,\n                t3.MIsActive as MEntryList_MStatus,\n                t2.MName,\n                t4.MName as MEntryList_MName,\n                t1.MIsActive,\n                t3.MModifyDate as MEntryList_MModifyDate\n                #_#lang_field0#_# \n            FROM\n                T_BD_Track t1\n                    INNER JOIN\n                @_@t_bd_track_l@_@ t2 ON t1.MOrgID = t2.MOrgID\n                    AND t1.MItemID = t2.MParentID\n                    AND t2.MIsDelete = 0\n                    LEFT JOIN\n                t_bd_trackentry t3 ON t3.MOrgID = t1.MOrgID\n                    and t3.MItemID = t1.MItemID\n                    and t3.MIsDelete = 0\n                    left join\n                @_@t_bd_trackentry_l@_@ t4 ON t4.MOrgID = t1.MOrgID\n                    and t4.MParentID = t3.MEntryID\n                    and t4.MIsDelete = 0\n            WHERE\n                t1.MOrgID = @MOrgID\n                    AND t1.MIsDelete = 0";

		private string GetOrderBy(GetParam param)
		{
			return string.IsNullOrWhiteSpace(param.OrderBy) ? "MCreateDate, \n\t            IF(TRIM(convert( MEntryList_MName using gbk)) RLIKE '^[0-9]', 1, 2), \n\t            TRIM(convert( MEntryList_MName using gbk))" : string.Empty;
		}

		public DataGridJson<BDTrackModel> Get(MContext ctx, GetParam param)
		{
			bool multiLang = ctx.MultiLang;
			ctx.MultiLang = false;
			param.IncludeDetail = true;
			DataGridJson<BDTrackModel> result = new APIDataRepository().Get<BDTrackModel>(ctx, param, CommonSelect, multLangFieldSql, false, true, GetOrderBy(param));
			ctx.MultiLang = multiLang;
			return result;
		}

		public OperationResult Post(MContext ctx, List<BDTrackModel> trackList)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			int num = 1;
			foreach (BDTrackModel track in trackList)
			{
				BDTrackModel trcModel = new BDTrackModel
				{
					MItemID = track.MItemID,
					MultiLanguage = track.MultiLanguage.ToList(),
					MCreateDate = DateTime.Now.AddSeconds((double)num)
				};
				num++;
				list.AddRange(GetNewTrackCommandList(trcModel, track.MEntryList, ctx));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num2 = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num2 > 0)
			{
				operationResult.Success = true;
			}
			else
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackCountThan5", "Tracking is not greater than 5");
			}
			return operationResult;
		}

		public List<BDTrackModel> GetList(string strWhere, MContext ctx, string orgIds = null, List<string> nameList = null, bool ignoreLocale = false, bool showDisableOption = true, string itemId = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MItemID", itemId),
				new MySqlParameter("@MLCID", ctx.MLCID)
			};
			if (string.IsNullOrWhiteSpace(orgIds))
			{
				orgIds = ctx.MOrgID;
			}
			string whereInSql = base.GetWhereInSql(orgIds, ref cmdParms, null);
			string arg = string.Empty;
			bool flag = nameList == null || nameList.Count() == 0;
			if (!flag)
			{
				arg = base.GetWhereInSql(nameList, ref cmdParms, null);
			}
			stringBuilder.Append("SELECT t3.MCreateDate,t1.MItemID,t2.MPKID,t2.MName,t3.MEntryID,t4.MPKID MEntryPKID,t4.MName MEntryName , t3.MIsActive, t2.MLocaleID");
			stringBuilder.AppendLine(" FROM T_BD_Track t1");
			stringBuilder.AppendFormat(" JOIN T_BD_Track_L t2 ON t1.MOrgID = t2.MOrgID and t2.MIsDelete = 0  and  t1.MItemID=t2.MParentID and t1.MIsDelete = 0 and t1.MIsActive = 1  AND t1.MOrgID in ({0}) ", whereInSql);
			if (flag)
			{
				if (!ignoreLocale)
				{
					stringBuilder.Append(" And t2.MLocaleID=@MLCID");
				}
			}
			else
			{
				stringBuilder.AppendFormat(" and TRIM(t2.MName) in ({0})", arg);
			}
			if (!string.IsNullOrEmpty(itemId))
			{
				stringBuilder.AppendLine(" And t1.MItemID=@MItemID ");
			}
			stringBuilder.AppendLine(" LEFT JOIN T_BD_TrackEntry t3 ON t3.MOrgID = t1.MOrgID and t3.MIsDelete = 0 and  t1.MItemID=t3.MItemID And t3.MIsDelete = 0");
			if (!showDisableOption)
			{
				stringBuilder.Append(" and t3.MIsActive=1");
			}
			stringBuilder.AppendLine(" LEFT JOIN T_BD_TrackEntry_L t4 ON t3.MOrgID = t1.MOrgID and t4.MIsDelete = 0 and  t3.MEntryID=t4.MParentID ");
			if (!showDisableOption)
			{
				stringBuilder.Append(" and t4.MIsActive=1");
			}
			if (flag && !ignoreLocale)
			{
				stringBuilder.Append(" And t4.MLocaleID=@MLCID");
			}
			if (strWhere.Trim() != "")
			{
				stringBuilder.AppendLine(" WHERE " + strWhere);
			}
			stringBuilder.AppendLine(" ORDER BY t1.MCreateDate, IF(TRIM(convert(t4.MName using gbk)) RLIKE '^[0-9]', 1, 2), TRIM(convert(t4.MName using gbk)) ");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDTrackModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms).Tables[0]);
		}

		public List<BDTrackModel> GetTrackNameList(MContext ctx, bool ignoreLocale = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT t2.MName,t2.MLocaleID,t1.MItemID ");
			stringBuilder.AppendLine(" FROM T_BD_Track t1");
			stringBuilder.AppendFormat(" JOIN T_BD_Track_L t2 ON t1.MItemID=t2.MParentID and t2.MOrgID = t1.MOrgID and t2.MIsDelete = 0 {0} ", ignoreLocale ? string.Empty : "and t2.MLocaleID=@MLocaleID");
			stringBuilder.AppendLine(" WHERE t1.MIsDelete = 0 AND t1.MOrgID=@MOrgID and t1.MIsActive = 1");
			stringBuilder.AppendLine(" ORDER BY t1.MCreateDate");
			return ModelInfoManager.GetDataModelBySql<BDTrackModel>(ctx, stringBuilder.ToString(), ctx.GetParameters((MySqlParameter)null));
		}

		public List<NameValueModel> GetTrackBasicInfo(MContext ctx, string orgIds = null, bool ignoreLocale = false, bool showDisableOption = true)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			List<BDTrackModel> list2 = GetList("", ctx, orgIds, null, ignoreLocale, showDisableOption, "");
			if (list2 == null || list2.Count == 0)
			{
				return list;
			}
			List<string> list3 = (from t in list2
			select t.MItemID).Distinct().ToList();
			foreach (string item in list3)
			{
				List<NameValueModel> trackEntryBasicInfo = GetTrackEntryBasicInfo(item, list2, ctx);
				if (trackEntryBasicInfo.Count > 0)
				{
					string mName = (from t in list2
					where t.MItemID == item
					select t).FirstOrDefault().MName;
					NameValueModel nameValueModel = new NameValueModel();
					nameValueModel.MName = mName;
					nameValueModel.MValue = item;
					nameValueModel.MChildren = trackEntryBasicInfo;
					list.Add(nameValueModel);
				}
			}
			return list;
		}

		public string GetBillTrackItemID(List<NameValueModel> trackList, string trackItemId, int index)
		{
			if (trackList == null || trackList.Count == 0 || string.IsNullOrEmpty(trackItemId))
			{
				return "";
			}
			if (trackList.Count < index)
			{
				return "";
			}
			List<NameValueModel> mChildren = trackList[index - 1].MChildren;
			if (mChildren == null || mChildren.Count == 0)
			{
				return "";
			}
			if (mChildren.Count((NameValueModel t) => t.MValue == trackItemId) > 0)
			{
				return trackItemId;
			}
			return "";
		}

		private List<NameValueModel> GetTrackEntryBasicInfo(string trackId, List<BDTrackModel> trackList, MContext ctx)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			List<BDTrackModel> list2 = (from t in trackList
			where t.MItemID == trackId
			orderby t.MEntryName
			select t).ToList();
			if (list2 == null || list2.Count == 0)
			{
				return list;
			}
			foreach (BDTrackModel item in list2)
			{
				NameValueModel nameValueModel = new NameValueModel();
				nameValueModel.MName = item.MEntryName;
				nameValueModel.MValue = item.MEntryID;
				nameValueModel.MValue1 = (item.MIsActive ? "1" : "0");
				list.Add(nameValueModel);
			}
			return list;
		}

		public OperationResult SaveList(string[] array, MContext ctx)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (array.Length > 1 && array[0] == array[1] + "_Megi")
			{
				for (int i = 2; i < array.Length; i++)
				{
					if (!string.IsNullOrWhiteSpace(array[i]))
					{
						BDTrackEntryModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<BDTrackEntryModel>(ctx);
						emptyDataEditModel.MItemID = array[1];
						setMultiField(emptyDataEditModel, "MName", array[i]);
						setMultiField(emptyDataEditModel, "MDesc", array[i]);
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDTrackEntryModel>(ctx, emptyDataEditModel, null, true));
					}
				}
			}
			else if (!string.IsNullOrWhiteSpace(array[0]))
			{
				BDTrackModel emptyDataEditModel2 = ModelInfoManager.GetEmptyDataEditModel<BDTrackModel>(ctx);
				emptyDataEditModel2.MOrgID = ctx.MOrgID;
				setMultiField(emptyDataEditModel2, "MName", array[0]);
				setMultiField(emptyDataEditModel2, "MDesc", array[0]);
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDTrackModel>(ctx, emptyDataEditModel2, null, true));
				for (int j = 1; j < array.Length; j++)
				{
					if (!string.IsNullOrWhiteSpace(array[j]))
					{
						BDTrackEntryModel emptyDataEditModel3 = ModelInfoManager.GetEmptyDataEditModel<BDTrackEntryModel>(ctx);
						emptyDataEditModel3.MItemID = emptyDataEditModel2.MItemID;
						setMultiField(emptyDataEditModel3, "MName", array[j]);
						setMultiField(emptyDataEditModel3, "MDesc", array[j]);
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDTrackEntryModel>(ctx, emptyDataEditModel3, null, true));
					}
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			OperationResult operationResult = new OperationResult();
			if (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0)
			{
				operationResult.Success = true;
			}
			else
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackCountThan5", "Tracking is not greater than 5");
			}
			return operationResult;
		}

		public List<CommandInfo> GetNewTrackCommandList(BDTrackModel trcModel, List<BDTrackEntryModel> optionsModels, MContext ctx)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			trcModel.MOrgID = ctx.MOrgID;
			if (trcModel.UpdateFieldList != null && trcModel.UpdateFieldList.Any())
			{
				trcModel.UpdateFieldList.Add("MModifyDate");
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDTrackModel>(ctx, trcModel, trcModel.UpdateFieldList, true));
			if (optionsModels != null)
			{
				int num = 1;
				foreach (BDTrackEntryModel optionsModel in optionsModels)
				{
					optionsModel.MItemID = trcModel.MItemID;
					optionsModel.MCreateDate = DateTime.Now.AddSeconds((double)num);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDTrackEntryModel>(ctx, optionsModel, optionsModel.UpdateFieldList, true));
					num++;
				}
			}
			return list;
		}

		public OperationResult SaveList(BDTrackModel trcModel, List<BDTrackEntryModel> optionsModels, MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> newTrackCommandList = GetNewTrackCommandList(trcModel, optionsModels, ctx);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(newTrackCommandList);
			if (num > 0)
			{
				operationResult.Success = true;
			}
			else
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TrackCountThan5", "Tracking is not greater than 5");
			}
			return operationResult;
		}

		private void setMultiField<T>(T trcModel, string multiKey, string multiValue) where T : BaseModel
		{
			MultiLanguageFieldList multiLanguageFieldList = trcModel.MultiLanguage.Find((Predicate<MultiLanguageFieldList>)((MultiLanguageFieldList p) => p.MFieldName.Equals(multiKey)));
			if (multiLanguageFieldList != null)
			{
				if (multiLanguageFieldList.MMultiLanguageField.Find((Predicate<MultiLanguageField>)((MultiLanguageField p) => p.MLocaleID.Equals("0x0009", StringComparison.OrdinalIgnoreCase))) == null)
				{
					multiLanguageFieldList.MMultiLanguageField.Add(new MultiLanguageField
					{
						MLocaleID = "0x0009",
						MValue = multiValue
					});
				}
				else
				{
					multiLanguageFieldList.MMultiLanguageField.Find((Predicate<MultiLanguageField>)((MultiLanguageField p) => p.MLocaleID.Equals("0x0009", StringComparison.OrdinalIgnoreCase))).MValue = multiValue;
				}
			}
		}

		public void updateEdit(BDTrackModel info, MContext ctx)
		{
			BDTrackModel dataEditModel = ModelInfoManager.GetDataEditModel<BDTrackModel>(ctx, info.MItemID, false, true);
			if (info.MultiLanguage != null)
			{
				dataEditModel.MOrgID = ctx.MOrgID;
				dataEditModel.MultiLanguage = info.MultiLanguage;
			}
			ModelInfoManager.InsertOrUpdate<BDTrackModel>(ctx, info, null);
		}

		public void updateOptEdit(BDTrackModel info, MContext ctx)
		{
			BDTrackEntryModel bDTrackEntryModel = ModelInfoManager.GetDataEditModel<BDTrackEntryModel>(ctx, info.MEntryID, false, true);
			if (bDTrackEntryModel != null)
			{
				if (info.MultiLanguage != null)
				{
					bDTrackEntryModel.MultiLanguage = info.MultiLanguage;
				}
			}
			else
			{
				bDTrackEntryModel = new BDTrackEntryModel();
				bDTrackEntryModel.MultiLanguage = info.MultiLanguage;
				bDTrackEntryModel.MItemID = info.MItemID;
				bDTrackEntryModel.IsNew = true;
			}
			ModelInfoManager.InsertOrUpdate<BDTrackEntryModel>(ctx, bDTrackEntryModel, null);
		}

		public OperationResult trackDel(BDTrackModel info, MContext ctx)
		{
			return ModelInfoManager.DeleteFlag<BDTrackModel>(ctx, info.MItemID);
		}

		public OperationResult trackOptDel(BDTrackModel info, MContext ctx)
		{
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "TrackEntry", info.MEntryID);
			if (operationResult.Success)
			{
				operationResult = ModelInfoManager.DeleteFlag<BDTrackEntryModel>(ctx, info.MEntryID);
			}
			return operationResult;
		}

		public bool IsExistName(MContext ctx, string mItemId, List<string> strList, bool isOption = false, string entryId = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = (mItemId == null) ? (strList.Count + 1) : (strList.Count + 2);
			num = ((!string.IsNullOrWhiteSpace(entryId)) ? (num + 1) : num);
			MySqlParameter[] array = new MySqlParameter[num];
			if (!isOption)
			{
				stringBuilder.Append(" SELECT * FROM t_bd_track t1 INNER JOIN t_bd_track_l t2 on t2.MOrgID = t1.MOrgID and t2.MIsDelete = 0  and  t1.MItemID= t2.MParentID and t1.MOrgID = @MOrgID and t1.MIsDelete = 0 ");
				stringBuilder.Append(" WHERE t1.MOrgId=@MOrgId ");
				if (mItemId != null)
				{
					stringBuilder.Append(" AND MItemID <> @MItemID ");
					array[num - 1] = new MySqlParameter("@MItemID", mItemId);
				}
			}
			else
			{
				stringBuilder.Append("SELECT t1.* , t3.MName FROM t_bd_trackentry t1 INNER JOIN t_bd_track t2 on t1.MOrgID = t2.MOrgID and t2.MIsDelete = 0  and  t2.MItemID = t1.MItemID INNER JOIN t_bd_trackentry_l t3 on t3.MParentID= t1.MEntryID and t3.MOrgID = t1.MOrgID  and t3.MIsDelete = 0 and t1.MOrgID = @MOrgID and t1.MIsDelete = 0 ");
				stringBuilder.Append(" WHERE t1.MOrgId=@MOrgId ");
				if (mItemId != null)
				{
					stringBuilder.Append(" AND t2.MItemID = @MItemID ");
					array[num - 1] = new MySqlParameter("@MItemID", mItemId);
				}
				if (!string.IsNullOrWhiteSpace(entryId))
				{
					stringBuilder.Append(" AND t1.MEntryID <> @MEntryID ");
					array[num - 2] = new MySqlParameter("@MEntryID", entryId);
				}
			}
			array[0] = new MySqlParameter("@MOrgID", ctx.MOrgID);
			for (int i = 0; i < strList.Count; i++)
			{
				if (i == 0)
				{
					stringBuilder.Append(" AND (( MName=@MName ) ");
					array[i + 1] = new MySqlParameter("@MName", strList[i]);
				}
				else
				{
					stringBuilder.Append("  OR  ( MName=@MName" + i + " ) ");
					array[i + 1] = new MySqlParameter(("@MName" + i) ?? "", strList[i]);
				}
				if (i == strList.Count() - 1)
				{
					stringBuilder.Append(")");
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0;
		}

		public OperationResult ArchiveTrackEntry(MContext ctx, string entryId, int status)
		{
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@IsActive", status),
				new MySqlParameter("@EntryId", entryId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo obj = new CommandInfo
			{
				CommandText = "update t_bd_trackentry_L set MIsActive = @IsActive where MParentID=@EntryId and MOrgID = @MOrgID and MIsDelete = 0"
			};
			DbParameter[] array = obj.Parameters = parameters;
			list.Add(obj);
			List<CommandInfo> list2 = list;
			List<CommandInfo> archiveFlagCmd = ModelInfoManager.GetArchiveFlagCmd<BDTrackEntryModel>(ctx, entryId, status == 1);
			list2.AddRange(archiveFlagCmd);
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list2) > 0)
			};
		}

		public int GetTrackIndex(MContext ctx, string trackId)
		{
			List<BDTrackModel> trackList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).TrackList;
			int num = trackList.FindIndex((BDTrackModel x) => x.MItemID == trackId);
			return num + 1;
		}
	}
}
