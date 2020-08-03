using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASMegiLanguageRepository
	{
		public List<BASMegiLanguageModel> GetByMegiLanguage(SqlWhere filter)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT a.*,b.MLocaleID,b.MName,b.MTooltip FROM T_Bas_AppUI a");
			stringBuilder.Append(" left join T_Bas_AppUI_L b on a.MItemID=b.AppUIID and b.MIsDelete = 0 and b.MIsActive = 1 and a.MIsDelete = 0 and a.MIsActive = 1 ");
			stringBuilder.Append(filter.WhereSqlString);
			return ModelInfoManager.DataTableToList<BASMegiLanguageModel>(DbHelperMySQL.Query(stringBuilder.ToString(), filter.Parameters).Tables[0]);
		}

		public BASMegiLanguageModel GetModelByMegiLanguage(SqlWhere filter)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT a.*,b.MLocaleID,b.MName,b.MTooltip FROM T_Bas_AppUI a");
			stringBuilder.Append(" left join T_Bas_AppUI_L b on a.MItemID=b.AppUIID and b.MIsDelete = 0 and b.MIsActive = 1 ");
			stringBuilder.Append("WHERE a.MIsDelete = 0 and a.MIsActive = 1 ");
			stringBuilder.Append(filter.WhereSqlString);
			stringBuilder.Append(" limit 1 ");
			DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), filter.Parameters);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return ModelInfoManager.DataTableToList<BASMegiLanguageModel>(dataSet.Tables[0])[0];
		}
	}
}
