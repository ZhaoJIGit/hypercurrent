using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDBankTypeBusiness : APIBusinessBase<BDBankTypeModel>, IBDBankTypeBusiness
	{
		private readonly BDBankTypeRepository dalBankType = new BDBankTypeRepository();

		protected override DataGridJson<BDBankTypeModel> OnGet(MContext ctx, GetParam param)
		{
			return dalBankType.Get(ctx, param);
		}

		public List<BDBankTypeViewModel> GetBDBankTypeList(MContext ctx)
		{
			return new BDBankTypeRepository().GetBDBankTypeList(ctx, null);
		}

		public OperationResult SaveBankType(MContext ctx, BDBankTypeEditModel banktype)
		{
			return new BDBankTypeRepository().SaveBankType(ctx, banktype);
		}

		public OperationResult DeleteBankType(MContext ctx, BDBankTypeModel banktype)
		{
			return new BDBankTypeRepository().DeleteBankType(ctx, banktype);
		}

		public BDBankTypeEditModel GetBDBankTypeEditModel(MContext ctx, BDBankTypeEditModel banktype)
		{
			return new BDBankTypeRepository().GetBDBankTypeEditModel(ctx, banktype);
		}
	}
}
