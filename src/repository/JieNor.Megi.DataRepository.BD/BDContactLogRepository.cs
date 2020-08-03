using JieNor.Megi.Core.Log;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDContactLogRepository
	{
		public static void AddContactNoteLog(MContext ctx, BDContactsModel model)
		{
			OptLogTemplate template = OptLogTemplate.Contact_Note;
			OptLog.AddLog(template, ctx, model.MItemID, model.MDesc);
		}
	}
}
