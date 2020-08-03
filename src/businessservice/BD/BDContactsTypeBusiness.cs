using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDContactsTypeBusiness : APIBusinessBase<BDContactsTypeModel>
	{
		private readonly BDContactsBusiness bizContact = new BDContactsBusiness();

		private readonly BDContactsTypeRepository dal = new BDContactsTypeRepository();

		private List<BDContactsTypeModel> _contactGroupDataPool;

		private List<BDContactsInfoModel> _contactDataPool;

		private string[] defaultGroupIds = new string[4]
		{
			"1",
			"2",
			"4",
			"3"
		};

		protected override void OnGetBefore(MContext ctx, GetParam param)
		{
			param.ModifiedSince = DateTime.MinValue;
			param.IgnoreModifiedSince = true;
		}

		protected override DataGridJson<BDContactsTypeModel> OnGet(MContext ctx, GetParam param)
		{
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_contactGroupDataPool = instance.ContactGroups;
			_contactDataPool = instance.Contacts;
			return dal.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, BDContactsTypeModel model)
		{
			if (!param.IncludeDetail.HasValue)
			{
				model.MContacts = null;
			}
		}
	}
}
