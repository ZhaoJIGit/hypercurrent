using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace JieNor.Megi.DataRepository.COM
{
	public class FPLogRepository : DataServiceT<FPLogModel>
	{
		public List<FPLogModel> Get(MContext ctx, GetParam param)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT * FROM t_fp_log WHERE MOrgID=@MOrgID AND MIsDelete=0 ");
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return ModelInfoManager.GetDataModelBySql<FPLogModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		public DataGridJson<FPLogModel> GetPageList(MContext ctx, FPFapiaoFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select * from t_fp_log where MOrgID=@MOrgID and MType=@MType and MIsDelete=0");
			if (filter.MStartDate != DateTime.MinValue)
			{
				stringBuilder.AppendFormat(" and MDate>=@MStartDate");
			}
			if (filter.MEndDate != DateTime.MinValue)
			{
				stringBuilder.AppendFormat(" and MDate<=@MEndDate");
			}
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MType", filter.MType),
				new MySqlParameter("@MStartDate", filter.MStartDate),
				new MySqlParameter("@MEndDate", filter.MEndDate.ToString("yyyy/MM/dd 23:59:59"))
			};
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				filter.OrderBy($"{filter.Sort} {filter.Order} ");
			}
			else
			{
				filter.AddOrderBy("MDate", SqlOrderDir.Desc);
			}
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			DataGridJson<FPLogModel> pageDataModelListBySql = ModelInfoManager.GetPageDataModelListBySql<FPLogModel>(ctx, sqlQuery);
			List<FPImportTypeConfigModel> dataModelList = ModelInfoManager.GetDataModelList<FPImportTypeConfigModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MType", filter.MType), false, false);
			pageDataModelListBySql.obj = ((dataModelList.Count > 0) ? dataModelList[0].MLastUploadDate : DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
			return pageDataModelListBySql;
		}
	}
}
