using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLCheckTypeBusiness : IGLCheckTypeBusiness, IDataContract<GLCheckTypeModel>
	{
		private readonly GLCheckTypeRepository dal = new GLCheckTypeRepository();

		public GLCheckTypeDataModel GetCheckTypeDataByType(MContext ctx, int type, bool includeDisabled = false)
		{
			return new GLCheckTypeUtility().GetCheckTypeDataByType(ctx, type, includeDisabled);
		}

		public int GetCheckTypeEnumByName(string checkTypeDBFeildName)
		{
			int result = -1;
			switch (checkTypeDBFeildName)
			{
			case "MContactID":
				result = 0;
				break;
			case "MEmployeeID":
				result = 1;
				break;
			case "MMerItemID":
				result = 2;
				break;
			case "MExpItemID":
				result = 3;
				break;
			case "MPaItemID":
				result = 4;
				break;
			case "MTrackItem1":
				result = 5;
				break;
			case "MTrackItem2":
				result = 6;
				break;
			case "MTrackItem3":
				result = 7;
				break;
			case "MTrackItem4":
				result = 8;
				break;
			case "MTrackItem5":
				result = 9;
				break;
			}
			return result;
		}

		public bool IsExistRequireCheckType(BDAccountModel account)
		{
			bool result = false;
			GLCheckGroupModel mCheckGroupModel = account.MCheckGroupModel;
			if (mCheckGroupModel == null)
			{
				return result;
			}
			return mCheckGroupModel.MContactID == CheckTypeStatusEnum.Required || mCheckGroupModel.MEmployeeID == CheckTypeStatusEnum.Required || mCheckGroupModel.MMerItemID == CheckTypeStatusEnum.Required || mCheckGroupModel.MExpItemID == CheckTypeStatusEnum.Required || mCheckGroupModel.MPaItemID == CheckTypeStatusEnum.Required || mCheckGroupModel.MTrackItem1 == CheckTypeStatusEnum.Required || mCheckGroupModel.MTrackItem2 == CheckTypeStatusEnum.Required || mCheckGroupModel.MTrackItem3 == CheckTypeStatusEnum.Required || mCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.Required || mCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.Required;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, GLCheckTypeModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLCheckTypeModel> modelData, string fields = null)
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

		public GLCheckTypeModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public GLCheckTypeModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<GLCheckTypeModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			GLUtility gLUtility = new GLUtility();
			int num = 10;
			List<GLCheckTypeModel> list = new List<GLCheckTypeModel>();
			for (int i = 0; i < num; i++)
			{
				if (ctx.MOrgVersionID == 1 && i == 4)
				{
					continue;
				}
				string checkTypeName = gLUtility.GetCheckTypeName(ctx, i);
				if (string.IsNullOrWhiteSpace(checkTypeName))
				{
					break;
				}
				GLCheckTypeModel gLCheckTypeModel = new GLCheckTypeModel();
				gLCheckTypeModel.MItemID = Convert.ToString(i);
				gLCheckTypeModel.MName = checkTypeName;
				gLCheckTypeModel.MType = i.ToString();
				gLCheckTypeModel.MColumnName = gLUtility.GetCheckTypeColumnName(i);
				list.Add(gLCheckTypeModel);
			}
			return list;
		}

		public DataGridJson<GLCheckTypeModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
