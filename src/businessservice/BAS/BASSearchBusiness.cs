using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASSearchBusiness : IBASSearchBusiness
	{
		private readonly BASSearchRepository dal = new BASSearchRepository();

		public DataGridJson<BASSearchModel> GetSearchResult(MContext ctx, BASSearchFilterModel filter)
		{
			if (string.IsNullOrWhiteSpace(filter.OrderBySqlString))
			{
				filter.AddOrderBy("MType", SqlOrderDir.Desc).AddOrderBy("MBizDate", SqlOrderDir.Desc);
			}
			DataGridJson<BASSearchModel> dataGridJson = new DataGridJson<BASSearchModel>();
			dataGridJson.total = dal.GetTotalCount(ctx, filter);
			dataGridJson.rows = dal.GetDataPageList(ctx, filter);
			return dataGridJson;
		}
	}
}
