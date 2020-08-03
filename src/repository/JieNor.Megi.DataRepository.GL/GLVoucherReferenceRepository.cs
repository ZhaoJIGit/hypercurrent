using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLVoucherReferenceRepository : DataServiceT<GLVoucherReferenceModel>
	{
		private GLUtility utility = new GLUtility();

		public OperationResult InsertReference(MContext ctx, GLVoucherReferenceModel model)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			ValidateVoucherReference(ctx, model);
			return InsertOrUpdate(ctx, model, null);
		}

		public void ValidateVoucherReference(MContext ctx, GLVoucherReferenceModel model)
		{
			ValidateQueryModel validteVoucherReferenceSql = utility.GetValidteVoucherReferenceSql(ctx, model);
			utility.QueryValidateSql(ctx, true, validteVoucherReferenceSql);
		}

		public List<GLVoucherReferenceModel> GetReferenceList(MContext ctx, int size)
		{
			string sql = "\n                        SELECT\r\n                            MItemID, MContent\n                        FROM\n                            t_gl_voucherreference\n                        WHERE\n                            MOrgID = @MOrgID\n                                AND MLocaleID = @MLocaleID\n                                AND MIsDelete = 0\n                        ORDER BY MModifyDate DESC\n                        LIMIT 0 ," + ((size == 0) ? 200 : size);
			return ModelInfoManager.GetDataModelBySql<GLVoucherReferenceModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
		}

		public List<CommandInfo> GetInsertReferenceCmds(MContext ctx, List<string> explanations)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			explanations = (explanations ?? new List<string>()).Distinct().ToList();
			List<string> list = (from x in GetModelList(ctx, new SqlWhere().AddFilter("MLocaleID", SqlOperators.Equal, ctx.MLCID), false) ?? new List<GLVoucherReferenceModel>()
			select x.MContent).Distinct().ToList();
			List<GLVoucherReferenceModel> list2 = new List<GLVoucherReferenceModel>();
			for (int i = 0; i < explanations.Count; i++)
			{
				if (!list.Contains(explanations[i].Trim()))
				{
					list2.Add(new GLVoucherReferenceModel
					{
						MContent = explanations[i].Trim(),
						MOrgID = ctx.MOrgID,
						MLocaleID = ctx.MLCID
					});
				}
			}
			if (list2.Count > 0)
			{
				result = ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, null, true);
			}
			return result;
		}
	}
}
