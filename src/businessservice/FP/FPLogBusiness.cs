using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.FP
{
	public class FPLogBusiness : IBasicBusiness<FPLogModel>
	{
		private readonly FPLogRepository dal = new FPLogRepository();

		public FPLogModel Delete(MContext ctx, DeleteParam param)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<FPLogModel> Get(MContext ctx, GetParam param)
		{
			throw new NotImplementedException();
		}

		public List<FPLogModel> Post(MContext ctx, PostParam<FPLogModel> param)
		{
			List<FPLogModel> dataList = param.DataList;
			if (!dataList.Any())
			{
				return new List<FPLogModel>();
			}
			foreach (FPLogModel item in dataList)
			{
				item.MDate = DateTime.Now;
			}
			FPLogModel fPLogModel = (from f in dataList
			orderby f.MDate descending
			select f).ToList()[0];
			FPLogModel fPLogModel2 = null;
			List<FPLogModel> dataModelList = ModelInfoManager.GetDataModelList<FPLogModel>(ctx, new SqlWhere().Equal("MType", fPLogModel.MType), false, false);
			if (dataModelList.Any())
			{
				dataModelList = (from f in dataModelList
				orderby f.MDate descending
				select f).ToList();
				fPLogModel2 = dataModelList.FirstOrDefault();
			}
			if (fPLogModel2 != null && fPLogModel2.MStatus == fPLogModel.MStatus && fPLogModel2.MMessage == fPLogModel.MMessage && fPLogModel.MStatus == 2 && !string.IsNullOrWhiteSpace(fPLogModel.MMessage))
			{
				fPLogModel.MItemID = fPLogModel2.MItemID;
				fPLogModel.IsUpdate = true;
			}
			List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, dataList, null, true);
			FPSettingBusiness fPSettingBusiness = new FPSettingBusiness();
			insertOrUpdateCmds.AddRange(fPSettingBusiness.GetFpLastUploadTimeUpdateCmds(ctx, fPLogModel.MType, fPLogModel.MDate));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(insertOrUpdateCmds) > 0);
			if (!operationResult.Success)
			{
				throw new Exception(operationResult.Message);
			}
			return dataList;
		}

		public DataGridJson<FPLogModel> GetFapiaoLogList(MContext ctx, FPFapiaoFilterModel filter)
		{
			return dal.GetPageList(ctx, filter);
		}
	}
}
