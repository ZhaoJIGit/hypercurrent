using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FC
{
	[ServiceContract]
	public interface IFCVoucherModule
	{
		[OperationContract]
		MActionResult<List<FCVoucherModuleModel>> GetVoucherModuleListWithNoEntry(string accessToken = null);

		[OperationContract]
		MActionResult<FCVoucherModuleModel> GetVoucherModelWithEntry(string PKID, string accessToken = null);

		[OperationContract]
		MActionResult<List<FCVoucherModuleModel>> GetVoucherModuleList(List<string> pkIDS, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FCVoucherModuleModel>> GetVoucherModuleModelPageList(GLVoucherListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<FCVoucherModuleModel> UpdateVoucherModuleModel(FCVoucherModuleModel model, string accessToken = null);

		[OperationContract]
		MActionResult<FCVoucherModuleModel> GetVoucherModule(string pkID = null, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null);
	}
}
