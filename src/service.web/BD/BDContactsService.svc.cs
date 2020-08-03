using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDContactsService : ServiceT<BDContactsModel>, IBDContacts
	{
		private readonly IBDContactsBusiness biz = new BDContactsBusiness();

		public MActionResult<DataGridJson<BDContactsInfoModel>> GetContactPageList(BDContactsInfoFilterModel filter, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactPageList, filter, accessToken);
		}

		public MActionResult<List<BDContactsInfoModel>> GetContactListForExport(BDContactsInfoFilterModel filter, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactListForExport, filter, accessToken);
		}

		public MActionResult<BDContactsEditModel> GetContactsEditModel(BDContactsEditModel model, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactsEditModel, model, accessToken);
		}

		public MActionResult<OperationResult> IsImportContactNamesExist(List<BDContactsInfoModel> list, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.IsImportContactNamesExist, list, accessToken);
		}

		public MActionResult<OperationResult> IsImportContactHaveSameName(List<BDContactsInfoModel> list, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.IsImportContactHaveSameName, list, accessToken);
		}

		public MActionResult<OperationResult> ImportContactList(List<BDContactsInfoModel> list, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.ImportContactList, list, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetImportTemplateModel, accessToken);
		}

		public MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.IsCanDeleteOrInactive, param, accessToken);
		}

		public MActionResult<List<BDContactsInfoModel>> GetContactsInfo(string typeId, string searchFilter, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactsInfo, typeId, searchFilter, accessToken);
		}

		public MActionResult<List<BDContactsTypeLModel>> GetTypeListByWhere(bool isAll = true, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetTypeListByWhere, isAll, accessToken);
		}

		public MActionResult<OperationResult> ContactsUpdate(BDContactsInfoModel info, List<string> fields = null, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.ContactsUpdate, info, fields, accessToken);
		}

		public MActionResult<List<BDContactItem>> GetContactItemList(BDContactsListFilter filter, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactItemList, filter, accessToken);
		}

		public MActionResult<string> GetContactName(string itemId, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactName, itemId, accessToken);
		}

		public MActionResult<OperationResult> ContactsGroupUpdate(BDContactsGroupModel info, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.ContactsGroupUpdate, info, accessToken);
		}

		public MActionResult<OperationResult> ContactToGroup(string selIds, string typeId, MultiLanguageFieldList newGroupLangModel, bool isExist, string moveFromTypeId, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.ContactToGroup, selIds, typeId, newGroupLangModel, isExist, moveFromTypeId, accessToken);
		}

		public MActionResult<string> ContactMoveOutGroup(string selIds, string moveFromTypeId, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunAction<string, string, string>(iBDContactsBusiness.ContactMoveOutGroup, selIds, moveFromTypeId, accessToken);
		}

		public MActionResult<OperationResult> ArchiveContact(List<string> contactIds, bool isActive, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.ArchiveContact, contactIds, isActive, accessToken);
		}

		public MActionResult<OperationResult> DelGroupAndLink(string typeId, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.DelGroupAndLink, typeId, accessToken);
		}

		public MActionResult<BDContactsInfoModel> GetContactEditInfo(BDContactsInfoModel model, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactEditInfo, model, accessToken);
		}

		public MActionResult<List<BDContactsTrackLinkModel>> GetTrackLinkList(SqlWhere filter, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetTrackLinkList, filter, accessToken);
		}

		public MActionResult<BDContactBasicInfoModel> GetContactTrackList(string contactId, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactTrackList, contactId, accessToken);
		}

		public MActionResult<BDContactsInfoModel> GetStatementContData(string contactID, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetStatementContData, contactID, accessToken);
		}

		public MActionResult<BDContactsInfoModel> GetContactViewData(string contactID, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactViewData, contactID, accessToken);
		}

		public MActionResult<List<BDContactsInfoModel>> GetContactViewDataList(string contactIDs, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactViewDataList, contactIDs, accessToken);
		}

		public MActionResult<OperationResult> DeleteContact(ParamBase param, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.DeleteContact, param, accessToken);
		}

		public MActionResult<List<BDContactsInfoModel>> GetContactsListByContactType(int contactType = 0, string keyWord = null, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactsListByContactType, contactType, keyWord, accessToken);
		}

		public MActionResult<List<BDContactsInfoModel>> GetContactsList(BDContactsListFilter filter, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactsList, filter, accessToken);
		}

		public MActionResult<OperationResult> AddContactNoteLog(BDContactsModel model, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.AddContactNoteLog, model, accessToken);
		}

		public MActionResult<List<BDContactsModel>> GetContactListByNameOrId(List<string> nameOrIdList, string accessToken = null)
		{
			IBDContactsBusiness iBDContactsBusiness = biz;
			return base.RunFunc(iBDContactsBusiness.GetContactListByNameOrId, nameOrIdList, accessToken);
		}
	}
}
