using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDContacts
	{
		[OperationContract]
		MActionResult<List<BDContactsInfoModel>> GetContactsInfo(string typeId, string searchFilter, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BDContactsInfoModel>> GetContactPageList(BDContactsInfoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDContactsInfoModel>> GetContactListForExport(BDContactsInfoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDContactsTypeLModel>> GetTypeListByWhere(bool isAll = true, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ContactsUpdate(BDContactsInfoModel info, List<string> fields = null, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDContactItem>> GetContactItemList(BDContactsListFilter filter, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetContactName(string itemId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ContactsGroupUpdate(BDContactsGroupModel info, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ContactToGroup(string selIds, string typeId, MultiLanguageFieldList newGroupLangModel, bool isExist, string moveFromTypeId, string accessToken = null);

		[OperationContract]
		MActionResult<string> ContactMoveOutGroup(string selIds, string moveFromTypeId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ArchiveContact(List<string> contactIds, bool isActive, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DelGroupAndLink(string typeId, string accessToken = null);

		[OperationContract]
		MActionResult<BDContactsInfoModel> GetContactEditInfo(BDContactsInfoModel model, string accessToken = null);

		[OperationContract]
		MActionResult<BDContactsEditModel> GetContactsEditModel(BDContactsEditModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDContactsTrackLinkModel>> GetTrackLinkList(SqlWhere filter, string accessToken = null);

		[OperationContract]
		MActionResult<BDContactBasicInfoModel> GetContactTrackList(string contactId, string accessToken = null);

		[OperationContract]
		MActionResult<BDContactsInfoModel> GetStatementContData(string contactID, string accessToken = null);

		[OperationContract]
		MActionResult<BDContactsInfoModel> GetContactViewData(string contactID, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDContactsInfoModel>> GetContactViewDataList(string contactIDs, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> IsImportContactNamesExist(List<BDContactsInfoModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> IsImportContactHaveSameName(List<BDContactsInfoModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportContactList(List<BDContactsInfoModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null);

		[OperationContract]
		MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteContact(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDContactsInfoModel>> GetContactsListByContactType(int contactType = 0, string keyWord = null, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDContactsInfoModel>> GetContactsList(BDContactsListFilter filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> AddContactNoteLog(BDContactsModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDContactsModel>> GetContactListByNameOrId(List<string> nameOrIdList, string accessToken = null);
	}
}
