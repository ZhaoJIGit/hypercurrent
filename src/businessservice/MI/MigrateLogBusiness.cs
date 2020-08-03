using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.DataRepository.MI;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.MI
{
	public class MigrateLogBusiness : IBasicBusiness<MigrateLogModel>
	{
		private MigrateConfigRepository dal = new MigrateConfigRepository();

		public MigrateLogModel Delete(MContext ctx, DeleteParam param)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<MigrateLogModel> Get(MContext ctx, GetParam param)
		{
			DataGridJson<MigrateLogModel> dataGridJson = new DataGridJson<MigrateLogModel>();
			BDTrackBusiness bDTrackBusiness = new BDTrackBusiness();
			MigrateTypeEnum type = (MigrateTypeEnum)((!string.IsNullOrWhiteSpace(param.Where)) ? Convert.ToInt32(param.Where) : 0);
			List<MigrateLogModel> migrateLogList = MigrateLogRepository.GetMigrateLogList<MigrateLogModel>(ctx, type);
			if (migrateLogList.Any())
			{
				migrateLogList[0].TrackIdList = bDTrackBusiness.GetTrackIdList(ctx);
			}
			dataGridJson.rows = migrateLogList;
			dataGridJson.total = ((migrateLogList != null) ? migrateLogList.Count : 0);
			return dataGridJson;
		}

		public List<MigrateLogModel> Post(MContext ctx, PostParam<MigrateLogModel> param)
		{
			throw new NotImplementedException();
		}
	}
}
