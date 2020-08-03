using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDEmailTemplateService : ServiceT<BDEmailTemplateModel>, IBDEmailTemplate
	{
		private readonly IBDEmailTemplateBusiness biz = new BDEmailTemplateBusiness();

		public MActionResult<List<BDEmailTemplateModel>> GetList(EmailSendTypeEnum sendType, string accessToken = null)
		{
			IBDEmailTemplateBusiness iBDEmailTemplateBusiness = biz;
			return base.RunFunc(iBDEmailTemplateBusiness.GetList, sendType, accessToken);
		}

		public MActionResult<OperationResult> Copy(BDEmailTemplateModel model, string accessToken = null)
		{
			IBDEmailTemplateBusiness iBDEmailTemplateBusiness = biz;
			return base.RunFunc(iBDEmailTemplateBusiness.Copy, model, accessToken);
		}

		public MActionResult<BDEmailTemplateModel> GetModel(string itemID, string accessToken = null)
		{
			IBDEmailTemplateBusiness iBDEmailTemplateBusiness = biz;
			return base.RunFunc(iBDEmailTemplateBusiness.GetModel, itemID, accessToken);
		}

		public MActionResult<OperationResult> Delete(string pkID, string accessToken = null)
		{
			IBDEmailTemplateBusiness iBDEmailTemplateBusiness = biz;
			return base.Delete(iBDEmailTemplateBusiness.Delete, pkID, accessToken);
		}

		public MActionResult<OperationResult> InsertOrUpdate(BDEmailTemplateModel modelData, string fields = null, string accessToken = null)
		{
			IBDEmailTemplateBusiness iBDEmailTemplateBusiness = biz;
			return base.InsertOrUpdate(iBDEmailTemplateBusiness.InsertOrUpdate, modelData, fields, accessToken);
		}
	}
}
