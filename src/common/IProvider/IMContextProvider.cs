using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.Common.Mongo.IProvider
{
	[ServiceContract]
	public interface IMContextProvider
	{
		[OperationContract]
		IList<MContext> GetMContext();

		[OperationContract]
		MContext GetMContextByUserId(string userId);

		[OperationContract]
		MContext GetMContextByAccessToken(string accessToken);

		[OperationContract]
		MContext GetMContextByEmail(string accessToken);

		[OperationContract]
		void ExpireMContextByUserId(string userId);

		[OperationContract]
		string GetMEmailByUserIp(string userIp);

		[OperationContract]
		void ExpireMContextByAccessToken(string accessToken);

		[OperationContract]
		void SaveMContext(MContext context, bool refresh = true);

		[OperationContract]
		void RemoveMContextByUserId(string userId);

		[OperationContract]
		void RemoveMContextByAccessToken(string accessToken);

		[OperationContract]
		void RefreshMContextByUserId(string userId);

		[OperationContract]
		void UpdateMContextByKeyField(string keyName, string keyValue, string fieldName, object fieldValue, bool syn);

		[OperationContract]
		void RefreshMContextByAccessToken(string userId);

		MContext GetMContextByAccessCode(string accessCode);

		void RemoveMContext(Dictionary<string, string> queryDictionary);
	}
}
