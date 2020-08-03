using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLCheckGroupBusiness : IGLCheckGroupBusiness, IDataContract<GLCheckGroupModel>
	{
		private readonly GLCheckGroupRepository dal = new GLCheckGroupRepository();

		public SqlWhere GetCheckGroupQueryFilter(MContext ctx, GLCheckGroupModel checkGroupModel)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MContactID", checkGroupModel.MContactID);
			sqlWhere.Equal("MEmployeeID", checkGroupModel.MEmployeeID);
			sqlWhere.Equal("MMerItemID", checkGroupModel.MMerItemID);
			sqlWhere.Equal("MExpItemID", checkGroupModel.MExpItemID);
			sqlWhere.Equal("MPaItemID", checkGroupModel.MPaItemID);
			sqlWhere.Equal("MTrackItem1", checkGroupModel.MTrackItem1);
			sqlWhere.Equal("MTrackItem2", checkGroupModel.MTrackItem2);
			sqlWhere.Equal("MTrackItem3", checkGroupModel.MTrackItem3);
			sqlWhere.Equal("MTrackItem4", checkGroupModel.MTrackItem4);
			sqlWhere.Equal("MTrackItem5", checkGroupModel.MTrackItem5);
			sqlWhere.Equal("MIsDelete", 0);
			return sqlWhere;
		}

		public GLCheckGroupModel GetModelByFilter(MContext ctx, GLCheckGroupModel checkGroupModel)
		{
			SqlWhere checkGroupQueryFilter = GetCheckGroupQueryFilter(ctx, checkGroupModel);
			return GetDataModelByFilter(ctx, checkGroupQueryFilter);
		}

		public List<CommandInfo> GetInsertOrUpdateCmd(MContext ctx, GLCheckGroupModel checkGroupModel, List<string> fieldList = null)
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<GLCheckGroupModel>(ctx, checkGroupModel, fieldList, true);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, GLCheckGroupModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLCheckGroupModel> modelData, string fields = null)
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

		public GLCheckGroupModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public GLCheckGroupModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<GLCheckGroupModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<GLCheckGroupModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
