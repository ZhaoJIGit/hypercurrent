using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDEmailTemplateBusiness : IDataContract<BDEmailTemplateModel>
	{
		List<BDEmailTemplateModel> GetList(MContext ctx, EmailSendTypeEnum sendType);

		OperationResult Copy(MContext ctx, BDEmailTemplateModel model);

		BDEmailTemplateModel GetModel(MContext ctx, string itemID);
	}
}
