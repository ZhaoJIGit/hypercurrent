using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.DataRepository.MI;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.MI
{
	public class MigrateConfigBusiness : IBasicBusiness<MigrateConfigModel>
	{
		private MigrateConfigRepository dal = new MigrateConfigRepository();

		public MigrateConfigModel Delete(MContext ctx, DeleteParam param)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<MigrateConfigModel> Get(MContext ctx, GetParam param)
		{
			DataGridJson<MigrateConfigModel> dataGridJson = new DataGridJson<MigrateConfigModel>();
			List<MigrateConfigModel> list = dal.GetList(ctx, param.ElementID);
			if (!string.IsNullOrWhiteSpace(param.ElementID))
			{
				ctx.IsSys = true;
				MigrateConfigModel migrateConfigModel = list[0];
				migrateConfigModel.MModifyDate = ctx.DateNow;
				ModelInfoManager.InsertOrUpdate<MigrateConfigModel>(ctx, migrateConfigModel, new List<string>
				{
					"MModifyDate"
				});
			}
			dataGridJson.rows = list;
			dataGridJson.total = ((list != null) ? list.Count : 0);
			return dataGridJson;
		}

		public List<MigrateConfigModel> Post(MContext ctx, PostParam<MigrateConfigModel> param)
		{
			List<MigrateConfigModel> dataList = param.DataList;
			if (!dataList.Any())
			{
				return new List<MigrateConfigModel>();
			}
			bool flag = false;
			bool flag2 = dataList[0].MConfirmedType == dataList[0].MType;
			List<ExistDataModel> source = new List<ExistDataModel>();
			if (!flag2)
			{
				source = MigrateConfigRepository.GetExistDataList(ctx, (from f in dataList
				select f.MOrgID).ToList());
			}
			foreach (MigrateConfigModel item in dataList)
			{
				if (!string.IsNullOrWhiteSpace(item.MItemID))
				{
					item.IsUpdate = true;
				}
				if (source.Any())
				{
					List<string> list = new List<string>();
					ExistDataModel existDataModel = source.FirstOrDefault((ExistDataModel f) => f.MOrgID == item.MOrgID);
					if (existDataModel.Contact > 0 || existDataModel.Employee > 0 || existDataModel.Item > 0 || existDataModel.Track > 0 || existDataModel.Currency > 0)
					{
						list.Add("基础资料");
					}
					if (existDataModel.InitBalance > 0)
					{
						list.Add("期初数据");
					}
					if (existDataModel.Voucher > 0)
					{
						list.Add("凭证数据");
					}
					if (list.Any())
					{
						flag = true;
						item.ValidationErrors.Add(new ValidationError
						{
							Message = string.Format("当前系统已有{0}，无法迁移凭证！", string.Join("、", list))
						});
					}
				}
			}
			if (flag)
			{
				return dataList;
			}
			List<string> list2 = new List<string>();
			if (dataList[0].MConfirmedType == dataList[0].MType)
			{
				list2.Add("MType");
				list2.Add("MConfirmedType");
				list2.Add("MStatus");
			}
			OperationResult operationResult = dal.Save(ctx, dataList, list2);
			if (!operationResult.Success)
			{
				throw new Exception(operationResult.Message);
			}
			return dataList;
		}
	}
}
