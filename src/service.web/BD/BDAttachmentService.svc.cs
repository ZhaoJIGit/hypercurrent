using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDAttachmentService : ServiceT<BDAttachmentModel>, IBDAttachment
	{
		private IBDAttachmentBusiness biz = new BDAttachmentBusiness();

		public MActionResult<List<BDAttachmentCategoryListModel>> GetCategoryList(string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.GetCategoryList, accessToken);
		}

		public MActionResult<OperationResult> UpdateCategoryModel(BDAttachmentCategoryModel model, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.UpdateCategoryModel, model, accessToken);
		}

		public MActionResult<string> GetAssociateTargetCategoryId(string bizObject, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.GetAssociateTargetCategoryId, bizObject, accessToken);
		}

		public MActionResult<DataGridJson<BDAttachmentListModel>> GetAttachmentList(BDAttachmentListFilterModel filter, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.GetAttachmentList, filter, accessToken);
		}

		public MActionResult<List<BDAttachmentListModel>> GetAttachmentListByIds(string attachIds, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.GetAttachmentListByIds, attachIds, accessToken);
		}

		public MActionResult<List<BDAttachmentListModel>> GetRelatedAttachmentList(string bizObject, string bizObjectID, string attachIds, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.GetRelatedAttachmentList, bizObject, bizObjectID, attachIds, accessToken);
		}

		public MActionResult<BDAttachmentModel> GetAttachmentModel(string attachId, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.GetAttachmentModel, attachId, accessToken);
		}

		public MActionResult<string> UpdateAttachmentModel(BDAttachmentModel model, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.UpdateAttachmentModel, model, accessToken);
		}

		public MActionResult<OperationResult> DeleteAttachmentList(ParamBase param, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.DeleteAttachmentList, param, accessToken);
		}

		public MActionResult<bool> MoveAttachmentListTo(ParamBase param, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.MoveAttachmentListTo, param, accessToken);
		}

		public MActionResult<string> CreateAttachmentAssociation(string bizObject, ParamBase param, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunAction<string, string, ParamBase>(iBDAttachmentBusiness.CreateAttachmentAssociation, bizObject, param, accessToken);
		}

		public MActionResult<OperationResult> RemoveAttachmentAssociation(string bizObject, ParamBase param, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.RemoveAttachmentAssociation, bizObject, param, accessToken);
		}

		public MActionResult<BDAttachmentModel> GetAttachmentModelById(string attchId, string accessToken = null)
		{
			IBDAttachmentBusiness iBDAttachmentBusiness = biz;
			return base.RunFunc(iBDAttachmentBusiness.GetAttachmentModelById, attchId, accessToken);
		}
	}
}
