using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.FP
{
	public class FPUtility
	{
		public FPBaseDataModel GetBaseData(MContext ctx)
		{
			FPBaseDataModel fPBaseDataModel = new FPBaseDataModel();
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			GLUtility gLUtility = new GLUtility();
			List<GLTreeModel> trackItemList = gLUtility.GetTrackItemList(ctx, false);
			fPBaseDataModel.MContact = instance.ContactList;
			fPBaseDataModel.MTrackLink = instance.MContactsTrackLink;
			fPBaseDataModel.MFastCode = gLUtility.GetFapiaoModuleList(ctx);
			fPBaseDataModel.MMerItem = instance.MerItemList;
			fPBaseDataModel.MTaxRate = instance.TaxRateList;
			fPBaseDataModel.MAccount = instance.AccountList;
			fPBaseDataModel.MTrackItem1 = gLUtility.GetTrackCheckTypeData(ctx, 5, false);
			fPBaseDataModel.MTrackItem2 = gLUtility.GetTrackCheckTypeData(ctx, 6, false);
			fPBaseDataModel.MTrackItem3 = gLUtility.GetTrackCheckTypeData(ctx, 7, false);
			fPBaseDataModel.MTrackItem4 = gLUtility.GetTrackCheckTypeData(ctx, 8, false);
			fPBaseDataModel.MTrackItem5 = gLUtility.GetTrackCheckTypeData(ctx, 9, false);
			fPBaseDataModel.MSetting = new FPFapiaoRepository().GetCodingSetting(ctx);
			return fPBaseDataModel;
		}

		public List<CommandInfo> GetInsertNewTrackOptionsCmds(MContext ctx, FPBaseDataModel baseData, FCFapiaoModuleModel model)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			List<BDTrackEntryModel> list = new List<BDTrackEntryModel>();
			if (baseData.MTrackItem1 != null && baseData.MTrackItem1.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel = HandleNewTrackOption(ctx, baseData.MTrackItem1, model.MTrackItem1, model.MTrackItem1Name);
				if (bDTrackEntryModel != null)
				{
					model.MTrackItem1 = bDTrackEntryModel.MEntryID;
					list.Add(bDTrackEntryModel);
					model.MNewTrackItem1 = true;
				}
			}
			if (baseData.MTrackItem2 != null && baseData.MTrackItem2.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel2 = HandleNewTrackOption(ctx, baseData.MTrackItem2, model.MTrackItem2, model.MTrackItem2Name);
				if (bDTrackEntryModel2 != null)
				{
					model.MTrackItem2 = bDTrackEntryModel2.MEntryID;
					list.Add(bDTrackEntryModel2);
					model.MNewTrackItem2 = true;
				}
			}
			if (baseData.MTrackItem3 != null && baseData.MTrackItem3.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel3 = HandleNewTrackOption(ctx, baseData.MTrackItem3, model.MTrackItem3, model.MTrackItem3Name);
				if (bDTrackEntryModel3 != null)
				{
					model.MTrackItem3 = bDTrackEntryModel3.MEntryID;
					list.Add(bDTrackEntryModel3);
					model.MNewTrackItem3 = true;
				}
			}
			if (baseData.MTrackItem4 != null && baseData.MTrackItem4.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel4 = HandleNewTrackOption(ctx, baseData.MTrackItem4, model.MTrackItem4, model.MTrackItem4Name);
				if (bDTrackEntryModel4 != null)
				{
					model.MTrackItem4 = bDTrackEntryModel4.MEntryID;
					list.Add(bDTrackEntryModel4);
					model.MNewTrackItem4 = true;
				}
			}
			if (baseData.MTrackItem5 != null && baseData.MTrackItem5.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel5 = HandleNewTrackOption(ctx, baseData.MTrackItem5, model.MTrackItem5, model.MTrackItem5Name);
				if (bDTrackEntryModel5 != null)
				{
					model.MTrackItem5 = bDTrackEntryModel5.MEntryID;
					list.Add(bDTrackEntryModel5);
					model.MNewTrackItem5 = true;
				}
			}
			if (list.Count > 0)
			{
				result = ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, null, true);
			}
			return result;
		}

		public static void ReWriteTrack(MContext ctx, List<BDContactsTrackLinkModel> trackLinkList, List<CommandInfo> comman, bool isIn, string TrackID, string ContactID, string SalTrackId, string PurTrackId)
		{
			if (!string.IsNullOrWhiteSpace(TrackID))
			{
				BDContactsTrackLinkModel bDContactsTrackLinkModel = (trackLinkList != null && trackLinkList.Any()) ? trackLinkList.FirstOrDefault((BDContactsTrackLinkModel x) => x.MTrackID == TrackID && x.MContactID == ContactID) : null;
				if (bDContactsTrackLinkModel == null)
				{
					bDContactsTrackLinkModel = new BDContactsTrackLinkModel();
					bDContactsTrackLinkModel.MTrackID = TrackID;
					bDContactsTrackLinkModel.MContactID = ContactID;
					bDContactsTrackLinkModel.MSalTrackId = SalTrackId;
					bDContactsTrackLinkModel.MPurTrackId = PurTrackId;
					comman.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsTrackLinkModel>(ctx, bDContactsTrackLinkModel, null, true));
				}
				else
				{
					bDContactsTrackLinkModel.MSalTrackId = SalTrackId;
					bDContactsTrackLinkModel.MPurTrackId = PurTrackId;
					comman.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsTrackLinkModel>(ctx, bDContactsTrackLinkModel, new List<string>
					{
						isIn ? "MPurTrackId" : "MSalTrackId"
					}, true));
				}
			}
		}

		public List<CommandInfo> GetInsertNewTrackOptionsCmds(MContext ctx, FPBaseDataModel baseData, FPCodingModel model)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			List<BDTrackEntryModel> list = new List<BDTrackEntryModel>();
			if (baseData.MTrackItem1 != null && baseData.MTrackItem1.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel = HandleNewTrackOption(ctx, baseData.MTrackItem1, model.MTrackItem1, model.MTrackItem1Name);
				if (bDTrackEntryModel != null)
				{
					model.MTrackItem1 = bDTrackEntryModel.MEntryID;
					list.Add(bDTrackEntryModel);
					baseData.MTrackItem1.MDataList.Add(new GLTreeModel
					{
						id = bDTrackEntryModel.MEntryID,
						parentId = bDTrackEntryModel.MItemID,
						text = bDTrackEntryModel.MName
					});
				}
			}
			if (baseData.MTrackItem2 != null && baseData.MTrackItem2.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel2 = HandleNewTrackOption(ctx, baseData.MTrackItem2, model.MTrackItem2, model.MTrackItem2Name);
				if (bDTrackEntryModel2 != null)
				{
					model.MTrackItem2 = bDTrackEntryModel2.MEntryID;
					list.Add(bDTrackEntryModel2);
					baseData.MTrackItem2.MDataList.Add(new GLTreeModel
					{
						id = bDTrackEntryModel2.MEntryID,
						parentId = bDTrackEntryModel2.MItemID,
						text = bDTrackEntryModel2.MName
					});
				}
			}
			if (baseData.MTrackItem3 != null && baseData.MTrackItem3.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel3 = HandleNewTrackOption(ctx, baseData.MTrackItem3, model.MTrackItem3, model.MTrackItem3Name);
				if (bDTrackEntryModel3 != null)
				{
					model.MTrackItem3 = bDTrackEntryModel3.MEntryID;
					list.Add(bDTrackEntryModel3);
					baseData.MTrackItem3.MDataList.Add(new GLTreeModel
					{
						id = bDTrackEntryModel3.MEntryID,
						parentId = bDTrackEntryModel3.MItemID,
						text = bDTrackEntryModel3.MName
					});
				}
			}
			if (baseData.MTrackItem4 != null && baseData.MTrackItem4.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel4 = HandleNewTrackOption(ctx, baseData.MTrackItem4, model.MTrackItem4, model.MTrackItem4Name);
				if (bDTrackEntryModel4 != null)
				{
					model.MTrackItem4 = bDTrackEntryModel4.MEntryID;
					list.Add(bDTrackEntryModel4);
					baseData.MTrackItem4.MDataList.Add(new GLTreeModel
					{
						id = bDTrackEntryModel4.MEntryID,
						parentId = bDTrackEntryModel4.MItemID,
						text = bDTrackEntryModel4.MName
					});
				}
			}
			if (baseData.MTrackItem5 != null && baseData.MTrackItem5.MHasDetail)
			{
				BDTrackEntryModel bDTrackEntryModel5 = HandleNewTrackOption(ctx, baseData.MTrackItem5, model.MTrackItem5, model.MTrackItem5Name);
				if (bDTrackEntryModel5 != null)
				{
					model.MTrackItem5 = bDTrackEntryModel5.MEntryID;
					list.Add(bDTrackEntryModel5);
					baseData.MTrackItem5.MDataList.Add(new GLTreeModel
					{
						id = bDTrackEntryModel5.MEntryID,
						parentId = bDTrackEntryModel5.MItemID,
						text = bDTrackEntryModel5.MName
					});
				}
			}
			if (list.Count > 0)
			{
				result = ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, null, true);
			}
			return result;
		}

		public BDTrackEntryModel HandleNewTrackOption(MContext ctx, GLCheckTypeDataModel src, string id, string name)
		{
			GLTreeModel gLTreeModel = src.MDataList.FirstOrDefault((GLTreeModel x) => x.id == id);
			if (gLTreeModel == null && !string.IsNullOrWhiteSpace(name))
			{
				return new BDTrackEntryModel
				{
					IsNew = true,
					MEntryID = (string.IsNullOrWhiteSpace(id) ? UUIDHelper.GetGuid() : id),
					MItemID = src.MCheckTypeGroupID,
					MName = name,
					MultiLanguage = GetMultiLanguage(ctx, "MName", name)
				};
			}
			return null;
		}

		public List<CommandInfo> GetInsertNewItemCmds(MContext ctx, List<BDItemModel> items, FCFapiaoModuleModel model)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			if (!string.IsNullOrWhiteSpace(model.MMerItemIDName) && !items.Exists((BDItemModel x) => x.MItemID == model.MMerItemID))
			{
				string nextItemNumber = GetNextItemNumber(items);
				BDItemModel bDItemModel = new BDItemModel
				{
					IsNew = true,
					MItemID = (string.IsNullOrWhiteSpace(model.MMerItemID) ? UUIDHelper.GetGuid() : model.MMerItemID),
					MNumber = nextItemNumber,
					MultiLanguage = GetMultiLanguage(ctx, "MDesc", model.MMerItemIDName),
					MIsExpenseItem = false
				};
				model.MNewItem = true;
				model.MMerItemID = bDItemModel.MItemID;
				result = ModelInfoManager.GetInsertOrUpdateCmd<BDItemModel>(ctx, bDItemModel, null, true);
			}
			return result;
		}

		public string GetNextItemNumber(List<BDItemModel> items)
		{
			for (int i = 1; i <= 9999; i++)
			{
				string number = i.ToString().ToFixLengthString(4, "0");
				if (!items.Exists((BDItemModel x) => x.MNumber == number))
				{
					return number;
				}
			}
			return "";
		}

		public List<MultiLanguageFieldList> GetMultiLanguage(MContext ctx, string field, string value)
		{
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList
			{
				MFieldName = field,
				MMultiLanguageField = new List<MultiLanguageField>()
			};
			List<MultiLanguageField> list2 = new List<MultiLanguageField>();
			List<string> mActiveLocaleIDS = ctx.MActiveLocaleIDS;
			foreach (string item2 in mActiveLocaleIDS)
			{
				MultiLanguageField item = new MultiLanguageField
				{
					MLocaleID = item2,
					MValue = value
				};
				multiLanguageFieldList.MMultiLanguageField.Add(item);
			}
			list.Add(multiLanguageFieldList);
			return list;
		}
	}
}
