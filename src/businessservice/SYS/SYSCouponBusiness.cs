using JieNor.Megi.BusinessContract.SYS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.SYS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.SYS
{
	public class SYSCouponBusiness : ISYSCouponBusiness, IDataContract<SYSCouponModel>
	{
		private readonly SYSCouponRepository _coupon = new SYSCouponRepository();

		private readonly SYSCouponUserRepository _couponUser = new SYSCouponUserRepository();

		public OperationResult ApplyCoupon(MContext ctx, string code, SYSCouponType type)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			int historyOrgCount = BASOrganisationRepository.GetHistoryOrgCount(ctx.MUserID);
			if (historyOrgCount > 1)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Org, "CouponInvalid", "The code you enter is invalid!");
				return operationResult;
			}
			SYSCouponModel couponModel = _coupon.GetCouponModel(code);
			if (couponModel == null || couponModel.MTypeID != Convert.ToInt32(type))
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Org, "CouponInvalid", "The code you enter is invalid!");
				return operationResult;
			}
			SYSCouponUserModel couponUserModel = _couponUser.GetCouponUserModel(couponModel.MItemID, ctx.MUserID);
			if (couponUserModel != null)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Org, "CouponInvalid", "The code you enter is invalid!");
				return operationResult;
			}
			SYSCouponModel sYSCouponModel = couponModel;
			DateTime dateTime = couponModel.MToDate;
			dateTime = dateTime.AddDays(1.0);
			sYSCouponModel.MToDate = dateTime.AddSeconds(-1.0);
			dateTime = couponModel.MFromDate;
			if (dateTime.Date > ctx.DateNow || couponModel.MToDate < ctx.DateNow)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Org, "CouponHasExpired", "The code you enter has expired!");
				return operationResult;
			}
			if (type == SYSCouponType.Trial)
			{
				return ApplyTrialCoupon(ctx, couponModel);
			}
			operationResult.Success = true;
			return operationResult;
		}

		private OperationResult ApplyTrialCoupon(MContext ctx, SYSCouponModel model)
		{
			string mCode = model.MCode;
			string mValue = model.MValue;
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Org, "CouponInvalid", "The code you enter is invalid!");
			if (string.IsNullOrEmpty(mValue))
			{
				return operationResult;
			}
			int num = 0;
			if (!int.TryParse(mValue.Trim(), out num) || num <= 0)
			{
				return operationResult;
			}
			bool flag2 = operationResult.Success = _coupon.ApplyCouponForTrial(ctx, model.MItemID, mCode, num);
			BASOrganisationBusiness bASOrganisationBusiness = new BASOrganisationBusiness();
			ctx.IsSys = true;
			BASOrganisationModel dataModel = bASOrganisationBusiness.GetDataModel(ctx, ctx.MOrgID, false);
			if (dataModel == null)
			{
				return operationResult;
			}
			DateTime dateTime = dataModel.MExpiredDate;
			DateTime date = dateTime.Date;
			dateTime = dataModel.MCreateDate;
			int num2 = (int)(date - dateTime.Date).TotalDays + 1;
			if (flag2)
			{
				operationResult.Message = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Org, "ApplyCouponSuccessfull", "{0} days trial extention added! You now have {1} free trial days! ", num, num2);
			}
			else
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Org, "ApplyCouponFailture", "Apply coupon fail!");
			}
			return operationResult;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult InsertOrUpdate(MContext ctx, SYSCouponModel modelData, string fields = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<SYSCouponModel> modelData, string fields = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			throw new NotImplementedException();
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			throw new NotImplementedException();
		}

		public SYSCouponModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			throw new NotImplementedException();
		}

		public SYSCouponModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public List<SYSCouponModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<SYSCouponModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}
	}
}
