using JieNor.Megi.Core.Context;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.Core.DataModel
{
	public interface IDataContract<T> where T : BaseModel
	{
		bool Exists(MContext ctx, string pkID, bool includeDelete = false);

		bool ExistsByFilter(MContext ctx, SqlWhere filter);

		OperationResult InsertOrUpdate(MContext ctx, T modelData, string fields = null);

		OperationResult InsertOrUpdateModels(MContext ctx, List<T> modelData, string fields = null);

		OperationResult Delete(MContext ctx, string pkID);

		OperationResult DeleteModels(MContext ctx, List<string> pkID);

		T GetDataModelByFilter(MContext ctx, SqlWhere filter);

		T GetDataModel(MContext ctx, string pkID, bool includeDelete = false);

		List<T> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false);

		DataGridJson<T> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false);
	}
}
