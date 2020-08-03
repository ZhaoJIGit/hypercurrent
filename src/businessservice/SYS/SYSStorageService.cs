using JieNor.Megi.BusinessContract.SYS;
using JieNor.Megi.DataRepository.SYS;

namespace JieNor.Megi.BusinessService.SYS
{
	public class SYSStorageService : ISYSStorageBusiness
	{
		private readonly SYSStorageRepository dal = new SYSStorageRepository();

		public void CreateBasDB()
		{
			dal.CreateBasDB();
		}
	}
}
