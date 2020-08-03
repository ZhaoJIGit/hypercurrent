using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.Common.Mongo.Service
{
	[ServiceContract]
	public interface IMongoMContext
	{
		[OperationContract]
		IList<MContext> GetMContext(string app = "System");

		[OperationContract]
		MContext GetMContextByUserId(string userId, string app = "System");

		[OperationContract]
		string GetMEmailByUserIp(string userId, string app = "System");

		[OperationContract]
		MContext GetMContextByAccessToken(string accessToken, string app = "System");

		[OperationContract]
		MContext GetMContextByEmail(string email, string app = "System");

		[OperationContract]
		void ExpireMContextByUserId(string userId, string app = "System");

		[OperationContract]
		void ExpireMContextByAccessToken(string accessToken, string app = "System");

		[OperationContract]
		void SaveMContext(MContext context, bool refresh = true, string app = "System");

		[OperationContract]
		void RemoveMContextByUserId(string userId, string app = "System");

		[OperationContract]
		void RemoveMContextByAccessToken(string accessToken, string app = "System");

		[OperationContract]
		void RefreshMContextByUserId(string userId, string app = "System");

		[OperationContract]
		void RefreshMContextByAccessToken(string accessToken, string app = "System");

		[OperationContract]
		MContext GetMContextByAccessCode(string accessCode, string app = "System");

		[OperationContract]
		void UpdateMContextByKeyField(string keyName, string keyValue, string fieldName, object fieldValue, bool syn, string app = "System");

		[OperationContract]
		void RemoveMContext(Dictionary<string, string> queryDictionary, string app = "System");
	}
}
