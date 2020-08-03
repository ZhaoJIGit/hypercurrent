using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDEmailTemplate
	{
		[OperationContract]
		MActionResult<List<BDEmailTemplateModel>> GetList(EmailSendTypeEnum sendType, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Copy(BDEmailTemplateModel model, string accessToken = null);

		[OperationContract]
		MActionResult<BDEmailTemplateModel> GetModel(string itemID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Delete(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertOrUpdate(BDEmailTemplateModel modelData, string fields = null, string accessToken = null);
	}
}
