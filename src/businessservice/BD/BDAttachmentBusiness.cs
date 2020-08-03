using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDAttachmentBusiness : IBDAttachmentBusiness
	{
		private BDAttachmentRepository _item = new BDAttachmentRepository();

		public List<BDAttachmentCategoryListModel> GetCategoryList(MContext ctx)
		{
			return _item.GetCategoryList(ctx);
		}

		public OperationResult UpdateCategoryModel(MContext ctx, BDAttachmentCategoryModel model)
		{
			return _item.UpdateCategoryModel(ctx, model);
		}

		public string GetAssociateTargetCategoryId(MContext ctx, string bizObject)
		{
			return _item.GetAssociateTargetCategoryId(ctx, bizObject);
		}

		public DataGridJson<BDAttachmentListModel> GetAttachmentList(MContext ctx, BDAttachmentListFilterModel filter)
		{
			return BDAttachmentRepository.GetAttachmentList(ctx, filter);
		}

		public List<BDAttachmentListModel> GetAttachmentListByIds(MContext ctx, string attachIds)
		{
			return _item.GetAttachmentListByIds(attachIds, ctx);
		}

		public List<BDAttachmentListModel> GetRelatedAttachmentList(MContext ctx, string bizObject, string bizObjectID, string attachIds)
		{
			return _item.GetRelatedAttachmentList(bizObject, bizObjectID, attachIds, ctx);
		}

		public BDAttachmentModel GetAttachmentModel(MContext ctx, string attachId)
		{
			return _item.GetAttachmentModel(attachId, ctx, false);
		}

		public string UpdateAttachmentModel(MContext ctx, BDAttachmentModel model)
		{
			return _item.UpdateAttachmentModel(ctx, model);
		}

		public OperationResult DeleteAttachmentList(MContext ctx, ParamBase param)
		{
			return _item.DeleteAttachmentList(ctx, param);
		}

		public bool MoveAttachmentListTo(MContext ctx, ParamBase param)
		{
			return _item.MoveAttachmentListTo(ctx, param);
		}

		public void CreateAttachmentAssociation(MContext ctx, string bizObject, ParamBase param)
		{
			_item.CreateAttachmentAssociation(ctx, bizObject, param);
		}

		public OperationResult RemoveAttachmentAssociation(MContext ctx, string bizObject, ParamBase param)
		{
			return _item.RemoveAttachmentAssociation(ctx, bizObject, param);
		}

		public BDAttachmentModel GetAttachmentModelById(MContext ctx, string attachId)
		{
			return _item.GetAttachmentModelById(attachId, ctx);
		}
	}
}
