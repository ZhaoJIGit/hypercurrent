using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDAttachmentBusiness
	{
		List<BDAttachmentCategoryListModel> GetCategoryList(MContext ctx);

		OperationResult UpdateCategoryModel(MContext ctx, BDAttachmentCategoryModel model);

		string GetAssociateTargetCategoryId(MContext ctx, string bizObject);

		DataGridJson<BDAttachmentListModel> GetAttachmentList(MContext ctx, BDAttachmentListFilterModel filter);

		List<BDAttachmentListModel> GetAttachmentListByIds(MContext ctx, string attachIds);

		List<BDAttachmentListModel> GetRelatedAttachmentList(MContext ctx, string bizObject, string bizObjectID, string attachIds);

		BDAttachmentModel GetAttachmentModel(MContext ctx, string attachId);

		BDAttachmentModel GetAttachmentModelById(MContext ctx, string attachId);

		string UpdateAttachmentModel(MContext ctx, BDAttachmentModel model);

		OperationResult DeleteAttachmentList(MContext ctx, ParamBase param);

		bool MoveAttachmentListTo(MContext ctx, ParamBase param);

		void CreateAttachmentAssociation(MContext ctx, string bizObject, ParamBase param);

		OperationResult RemoveAttachmentAssociation(MContext ctx, string bizObject, ParamBase param);
	}
}
