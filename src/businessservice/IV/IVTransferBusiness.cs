using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVTransferBusiness : BusinessServiceBase, IIVTransferBusiness
	{
		public List<IVTransferListModel> GetTransferList(MContext ctx, string filterString)
		{
			return IVTransferRepository.GetTransferList(ctx, filterString);
		}

		public OperationResult UpdateTransfer(MContext ctx, IVTransferModel model)
		{
			OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, model.MBizDate);
			if (!operationResult.Success)
			{
				throw new MActionException
				{
					Codes = new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.MPeriodClosed
					},
					Messages = new List<string>
					{
						operationResult.Message
					}
				};
			}
			if (!string.IsNullOrWhiteSpace(model.MID))
			{
				OperationResult operationResult2 = GLInterfaceRepository.IsDocCanOperate(ctx, new List<string>
				{
					model.MID
				});
				if (!operationResult2.Success)
				{
					throw new MActionException
					{
						Codes = new List<MActionResultCodeEnum>
						{
							MActionResultCodeEnum.MVoucherApproved
						},
						Messages = new List<string>
						{
							operationResult2.Message
						}
					};
				}
			}
			model.MFromReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
			model.MToReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
			if (!string.IsNullOrEmpty(model.MID))
			{
				IVTransferModel transferEditModel = IVTransferRepository.GetTransferEditModel(ctx, model.MID);
				if (!string.IsNullOrEmpty(transferEditModel.MID))
				{
					model.MFromReconcileStatu = transferEditModel.MFromReconcileStatu;
					model.MToReconcileStatu = transferEditModel.MToReconcileStatu;
				}
			}
			return IVTransferRepository.UpdateTransfer(ctx, model);
		}

		public IVTransferModel GetTransferEditModel(MContext ctx, string pkID)
		{
			IVTransferModel transferEditModel = IVTransferRepository.GetTransferEditModel(ctx, pkID);
			transferEditModel.MActionPermission = GetActionPermissionModel(ctx, transferEditModel);
			return transferEditModel;
		}

		private IVTransferPermissionModel GetActionPermissionModel(MContext ctx, IVTransferModel model)
		{
			IVTransferPermissionModel iVTransferPermissionModel = new IVTransferPermissionModel();
			BDBankAccountBusiness bDBankAccountBusiness = new BDBankAccountBusiness();
			BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountBusiness.GetBDBankAccountEditModel(ctx, model.MFromAcctID);
			BDBankAccountEditModel bDBankAccountEditModel2 = bDBankAccountBusiness.GetBDBankAccountEditModel(ctx, model.MToAcctID);
			if (!base.HavePermission(ctx, "Bank", "Change"))
			{
				iVTransferPermissionModel.MHaveAction = false;
				return iVTransferPermissionModel;
			}
			if (string.IsNullOrEmpty(model.MID))
			{
				iVTransferPermissionModel.MHaveAction = false;
				iVTransferPermissionModel.MIsCanEdit = true;
				return iVTransferPermissionModel;
			}
			iVTransferPermissionModel.MHaveAction = true;
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
			if (!base.HavePermission(ctx, "General_Ledger", "View") || !GLInterfaceRepository.IsBillCreatedVoucher(ctx, model.MID))
			{
				iVTransferPermissionModel.MIsCanViewVoucherCreateDetail = false;
			}
			if (!operationResult.Success || Math.Abs(model.MFromReconcileAmtFor) > decimal.Zero || Math.Abs(model.MToReconcileAmtFor) > decimal.Zero)
			{
				iVTransferPermissionModel.MIsCanDelete = false;
			}
			if (!operationResult.Success)
			{
				iVTransferPermissionModel.MIsCanEdit = false;
			}
			IVReconcileStatus mFromReconcileStatu = (IVReconcileStatus)model.MFromReconcileStatu;
			IVReconcileStatus mToReconcileStatu = (IVReconcileStatus)model.MToReconcileStatu;
			if (!bDBankAccountEditModel.MIsNeedReconcile && !bDBankAccountEditModel2.MIsNeedReconcile)
			{
				iVTransferPermissionModel.MIsCanViewReconcile = false;
				iVTransferPermissionModel.MIsCanDeleteReconcile = false;
				iVTransferPermissionModel.MMarkAsReconciled = false;
				iVTransferPermissionModel.MMarkAsUnReconciled = false;
			}
			else if (bDBankAccountEditModel.MIsNeedReconcile && bDBankAccountEditModel2.MIsNeedReconcile)
			{
				iVTransferPermissionModel.MIsCanViewReconcile = false;
				iVTransferPermissionModel.MIsCanDeleteReconcile = false;
				if (mFromReconcileStatu == IVReconcileStatus.Partly || mFromReconcileStatu == IVReconcileStatus.Completely || mToReconcileStatu == IVReconcileStatus.Partly || mToReconcileStatu == IVReconcileStatus.Completely)
				{
					iVTransferPermissionModel.MIsCanViewReconcile = true;
					iVTransferPermissionModel.MIsCanDeleteReconcile = true;
				}
				SetMarkStatus(iVTransferPermissionModel, mFromReconcileStatu, mToReconcileStatu);
			}
			else if (bDBankAccountEditModel.MIsNeedReconcile)
			{
				if (mFromReconcileStatu != IVReconcileStatus.Partly && mFromReconcileStatu != IVReconcileStatus.Completely)
				{
					iVTransferPermissionModel.MIsCanViewReconcile = false;
					iVTransferPermissionModel.MIsCanDeleteReconcile = false;
				}
				SetMarkStatus(iVTransferPermissionModel, mFromReconcileStatu, mToReconcileStatu);
			}
			else
			{
				if (mToReconcileStatu != IVReconcileStatus.Partly && mToReconcileStatu != IVReconcileStatus.Completely)
				{
					iVTransferPermissionModel.MIsCanViewReconcile = false;
					iVTransferPermissionModel.MIsCanDeleteReconcile = false;
				}
				SetMarkStatus(iVTransferPermissionModel, mFromReconcileStatu, mToReconcileStatu);
			}
			if (!base.HavePermission(ctx, "BankAccount", "Change") || mFromReconcileStatu == IVReconcileStatus.Partly || mFromReconcileStatu == IVReconcileStatus.Completely || mToReconcileStatu == IVReconcileStatus.Partly || mToReconcileStatu == IVReconcileStatus.Completely)
			{
				iVTransferPermissionModel.MIsCanDelete = false;
			}
			if (!base.HavePermission(ctx, "Bank_Reconciliation", "Change"))
			{
				iVTransferPermissionModel.MIsCanDeleteReconcile = false;
				iVTransferPermissionModel.MMarkAsReconciled = false;
				iVTransferPermissionModel.MMarkAsUnReconciled = false;
			}
			return iVTransferPermissionModel;
		}

		private void SetMarkStatus(IVTransferPermissionModel actionPerModel, IVReconcileStatus fromRecStatu, IVReconcileStatus toRecStatu)
		{
			if (fromRecStatu == IVReconcileStatus.None && toRecStatu == IVReconcileStatus.None)
			{
				actionPerModel.MMarkAsUnReconciled = false;
			}
			else if (fromRecStatu == IVReconcileStatus.Marked && toRecStatu == IVReconcileStatus.Marked)
			{
				actionPerModel.MMarkAsReconciled = false;
			}
			else
			{
				actionPerModel.MMarkAsReconciled = false;
				actionPerModel.MMarkAsUnReconciled = false;
			}
		}

		public OperationResult UpdateReconcileStatu(MContext ctx, string transferId, IVReconcileStatus statu)
		{
			OperationResult operationResult = new OperationResult();
			IVTransferModel transferEditModel = IVTransferRepository.GetTransferEditModel(ctx, transferId);
			if (string.IsNullOrEmpty(transferEditModel.MID))
			{
				return operationResult;
			}
			IVReconcileStatus mFromReconcileStatu = (IVReconcileStatus)transferEditModel.MFromReconcileStatu;
			IVReconcileStatus mToReconcileStatu = (IVReconcileStatus)transferEditModel.MToReconcileStatu;
			if (mFromReconcileStatu == IVReconcileStatus.Partly || mFromReconcileStatu == IVReconcileStatus.Completely || mToReconcileStatu == IVReconcileStatus.Partly || mToReconcileStatu == IVReconcileStatus.Completely)
			{
				string text2 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！");
				operationResult.Success = false;
				return operationResult;
			}
			return IVTransferRepository.UpdateReconcileStatu(ctx, transferId, statu);
		}

		public OperationResult DeleteTransfer(MContext ctx, IVTransferModel model)
		{
			IVTransferModel transferEditModel = IVTransferRepository.GetTransferEditModel(ctx, model.MID);
			if (transferEditModel == null || transferEditModel.MFromReconcileStatu == Convert.ToInt32(IVReconcileStatus.Partly) || transferEditModel.MFromReconcileStatu == Convert.ToInt32(IVReconcileStatus.Completely) || transferEditModel.MToReconcileStatu == Convert.ToInt32(IVReconcileStatus.Partly) || transferEditModel.MToReconcileStatu == Convert.ToInt32(IVReconcileStatus.Completely))
			{
				return new OperationResult
				{
					Success = false
				};
			}
			return IVTransferRepository.DeleteTransfer(ctx, model);
		}
	}
}
