using JieNor.Megi.EntityModel.MResource;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.Common.Mongo.IProvider
{
	[ServiceContract]
	public interface IMResourceProvider
	{
		[OperationContract]
		List<JieNor.Megi.EntityModel.MResource.MResource> Get(string accessToken = null, string userId = null, string orgId = null);

		[OperationContract]
		void Save(List<JieNor.Megi.EntityModel.MResource.MResource> resources);

		[OperationContract]
		void Remove(string accessToken = null, string userId = null, string orgId = null);

		[OperationContract]
		void RemoveWithIDs(IEnumerable<string> ids);
	}
}
