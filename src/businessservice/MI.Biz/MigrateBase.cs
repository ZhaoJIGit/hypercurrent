using JieNor.Megi.Core;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace JieNor.Megi.BusinessService.MI.Biz
{
	public class MigrateBase
	{
		public List<MigrateLogBaseModel> GetLogList(MContext ctx, string migrationId)
		{
			return ModelInfoManager.GetDataModelList<MigrateLogBaseModel>(ctx, new SqlWhere().Equal("MMigrationID", migrationId), false, false);
		}

		public string SerializeObject(object obj)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(obj);
		}

		public T DeSerializeObject<T>(string str)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Deserialize<T>(str);
		}
	}
}
