using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDAttachment
	{
		[OperationContract]
		MActionResult<List<BDAttachmentCategoryListModel>> GetCategoryList(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateCategoryModel(BDAttachmentCategoryModel model, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetAssociateTargetCategoryId(string bizObject, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BDAttachmentListModel>> GetAttachmentList(BDAttachmentListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAttachmentListModel>> GetAttachmentListByIds(string attachIds, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAttachmentListModel>> GetRelatedAttachmentList(string bizObject, string bizObjectID, string attachIds, string accessToken = null);

		[OperationContract]
		MActionResult<BDAttachmentModel> GetAttachmentModel(string attachId, string accessToken = null);

		[OperationContract]
		MActionResult<BDAttachmentModel> GetAttachmentModelById(string attchId, string accessToken = null);

		[OperationContract]
		MActionResult<string> UpdateAttachmentModel(BDAttachmentModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteAttachmentList(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<bool> MoveAttachmentListTo(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<string> CreateAttachmentAssociation(string bizObject, ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> RemoveAttachmentAssociation(string bizObject, ParamBase param, string accessToken = null);
	}
}
