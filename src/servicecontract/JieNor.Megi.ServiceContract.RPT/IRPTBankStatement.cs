using JieNor.Megi.DataModel.IV;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTBankStatement : IRPTBizReport<RPTBankStatementFilterModel>
	{
	}
}
