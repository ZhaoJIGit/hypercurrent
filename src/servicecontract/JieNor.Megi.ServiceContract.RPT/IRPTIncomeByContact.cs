using JieNor.Megi.DataModel.RPT;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTIncomeByContact : IRPTBizReport<RPTIncomeByContactFilterModel>
	{
	}
}
