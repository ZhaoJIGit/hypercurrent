using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDContactsBusiness : IDataContract<BDContactsModel>
	{
		List<BDContactsInfoModel> GetContactsInfo(MContext ctx, string typeId, string searchFilter);

		DataGridJson<BDContactsInfoModel> GetContactPageList(MContext ctx, BDContactsInfoFilterModel filter);

		List<BDContactsInfoModel> GetContactListForExport(MContext ctx, BDContactsInfoFilterModel filter);

		List<BDContactsTypeLModel> GetTypeListByWhere(MContext ctx, bool isAll = true);

		OperationResult ContactsUpdate(MContext ctx, BDContactsInfoModel info, List<string> fields = null);

		OperationResult ContactsGroupUpdate(MContext ctx, BDContactsGroupModel info);

		List<BDContactItem> GetContactItemList(MContext ctx, BDContactsListFilter filter);

		string GetContactName(MContext ctx, string itemId);

		OperationResult ContactToGroup(MContext ctx, string selIds, string typeId, MultiLanguageFieldList newGroupLangModel, bool isExist, string moveFromTypeId);

		void ContactToArchivedGroup(MContext ctx, string selIds, string typeName = "Archived");

		void ContactMoveOutGroup(MContext ctx, string selIds, string moveFromTypeId);

		OperationResult ArchiveContact(MContext ctx, List<string> contactIds, bool isActive);

		OperationResult DelGroupAndLink(MContext ctx, string typeId);

		BDContactsInfoModel GetContactEditInfo(MContext ctx, BDContactsInfoModel model);

		BDContactsEditModel GetContactsEditModel(MContext ctx, BDContactsEditModel model);

		List<BDContactsTrackLinkModel> GetTrackLinkList(MContext ctx, SqlWhere filter);

		BDContactBasicInfoModel GetContactTrackList(MContext ctx, string contactId);

		BDContactsInfoModel GetStatementContData(MContext ctx, string contactID);

		BDContactsInfoModel GetContactViewData(MContext ctx, string contactID);

		List<BDContactsInfoModel> GetContactViewDataList(MContext ctx, string contactIDs);

		OperationResult IsImportContactNamesExist(MContext ctx, List<BDContactsInfoModel> list);

		OperationResult IsImportContactHaveSameName(MContext ctx, List<BDContactsInfoModel> list);

		OperationResult ImportContactList(MContext ctx, List<BDContactsInfoModel> list);

		ImportTemplateModel GetImportTemplateModel(MContext ctx);

		BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param);

		OperationResult DeleteContact(MContext ctx, ParamBase param);

		List<BDContactsInfoModel> GetContactsListByContactType(MContext ctx, int contactType = 0, string keyWord = null);

		List<BDContactsInfoModel> GetContactsList(MContext ctx, BDContactsListFilter filter);

		OperationResult AddContactNoteLog(MContext ctx, BDContactsModel model);

		List<BDContactsModel> GetContactListByNameOrId(MContext ctx, List<string> nameOrIdList);
	}
}
