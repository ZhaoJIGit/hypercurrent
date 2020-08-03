using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Log;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;

namespace JieNor.Megi.BusinessService.BAS
{
	public class OptLogListBusiness : IOptLogListBusiness
	{
		public DataGridJson<OptLogListModel> GetOptLogList(MContext ctx, OptLogListFilter filter)
		{
			DataGridJson<OptLogListModel> logList = OptLog.GetLogList(filter);
			if (logList.rows.Count <= 0)
			{
				return logList;
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "SystemGenerated", "系统生成");
			foreach (OptLogListModel row in logList.rows)
			{
				if (!string.IsNullOrEmpty(row.MCreateBy))
				{
					row.MNote = "(" + text + ")" + row.MNote;
				}
			}
			return logList;
		}
	}
}
