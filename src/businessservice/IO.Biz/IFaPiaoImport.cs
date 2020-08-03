using JieNor.Megi.DataModel.IO;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.BusinessService.IO.Biz
{
	public interface IFaPiaoImport
	{
		DataTable GetEffiectiveData(IOImportDataModel model, List<IOSolutionConfigModel> soluConfig, DataTable sourceData, out List<string> errorMsgList);
	}
}
