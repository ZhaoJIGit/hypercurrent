using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log.TableLog;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.ServiceContract.BAS;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.FP
{
    public class FPTableRepository : DataServiceT<FPTableModel>
    {
        private readonly FPFapiaoRepository fapiapDal = new FPFapiaoRepository();

        public DataGridJson<FPTableViewModel> GetTableViewModelGrid(MContext ctx, FPTableViewFilterModel filter)
        {
            int tableModelListCount = GetTableModelListCount(ctx, filter);
            List<FPTableViewModel> tableModelList = GetTableModelList(ctx, filter);
            return new DataGridJson<FPTableViewModel>
            {
                total = tableModelListCount,
                rows = tableModelList
            };
        }

        public DataGridJson<FPTableViewModel> GetTableViewModelPageList(MContext ctx, FPTableViewFilterModel filter)
        {
            List<FPTableViewModel> tables = GetTableModelList(ctx, filter);
            List<string> tableIds = (from x in tables
                                     select x.MItemID).ToList();
            List<FPFapiaoModel> fapiaoListDataByTableIds = new FPFapiaoRepository().GetFapiaoListDataByTableIds(ctx, tableIds);
            List<IVInvoiceModel> invoiceListByTableIds = GetInvoiceListByTableIds(ctx, tableIds);
            int i;
            for (i = 0; i < tables.Count; i++)
            {
                tables[i].fapiaoList = (from x in fapiaoListDataByTableIds
                                        where x.MTableID == tables[i].MItemID
                                        select x).ToList();
                tables[i].invoiceList = (from x in invoiceListByTableIds
                                         where x.MTableID == tables[i].MItemID
                                         select x).ToList();
            }
            tables = (from x in tables
                      orderby x.MBizDate descending
                      orderby int.Parse(x.MNumber) descending
                      select x).ToList();
            return new DataGridJson<FPTableViewModel>
            {
                rows = tables,
                total = GetTableModelListCount(ctx, filter)
            };
        }

        public bool IsTableNumberDuplicated(MContext ctx, string number, DateTime date, string tableId, int invoiceType)
        {
            SqlWhere sqlWhere = new SqlWhere();
            sqlWhere.AddFilter("MNumber", SqlOperators.Equal, number);
            if (!string.IsNullOrWhiteSpace(tableId))
            {
                sqlWhere.AddFilter("MItemID", SqlOperators.NotEqual, tableId);
            }
            sqlWhere.AddFilter("MInvoiceType", SqlOperators.Equal, invoiceType);
            sqlWhere.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
            sqlWhere.AddFilter("MIsDelete", SqlOperators.Equal, 0);
            return ExistsByFilter(ctx, sqlWhere);
        }

        public int GetNextTableNumber(MContext ctx, string tableId, int invoiceType)
        {
            FPTableModel fPTableModel = string.IsNullOrWhiteSpace(tableId) ? null : GetDataModel(ctx, tableId, false);
            if (fPTableModel != null)
            {
                return int.Parse(fPTableModel.MNumber);
            }
            return GetNextTableNumber(ctx, invoiceType);
        }

        public List<FPTableViewModel> GetTableViewModelList(MContext ctx, FPTableViewFilterModel filter)
        {
            filter = (filter ?? new FPTableViewFilterModel());
            return GetTableModelList(ctx, filter);
        }

        public List<NameValueModel> GetTableHomeData(MContext ctx, int invoiceType, DateTime date)
        {
            string text = "SELECT \r\n                                SUM(CASE MIssueStatus\r\n                                    WHEN 0 THEN 1\r\n                                    ELSE 0\r\n                                END) AS NotIssued,\r\n                                SUM(CASE MIssueStatus\r\n                                    WHEN 1 THEN 1\r\n                                    ELSE 0\r\n                                END) AS PartlyIssued,\r\n                                SUM(CASE MIssueStatus\r\n                                    WHEN 2 THEN 1\r\n                                    ELSE 0\r\n                                END) AS FullIssued\r\n                            FROM\r\n                                t_fp_Table\r\n                            WHERE\r\n                                morgid = @MOrgID\r\n                                    AND MIsDelete = 0\r\n                                    AND MInvoiceType = @MInvoiceType ";
            if (date > new DateTime(1900, 1, 1))
            {
                text = text + " and (year(MBizDate) * 12 + month(MBizDate))  =  " + (date.Year * 12 + date.Month);
            }
            DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(text, ctx.GetParameters("@MInvoiceType", invoiceType));
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
            {
                return new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        MName = "0",
                        MValue = "0"
                    },
                    new NameValueModel
                    {
                        MName = "1",
                        MValue = "0"
                    },
                    new NameValueModel
                    {
                        MName = "2",
                        MValue = "0"
                    }
                };
            }
            List<NameValueModel> list = new List<NameValueModel>();
            NameValueModel obj = new NameValueModel
            {
                MName = "0"
            };
            decimal num = dataSet.Tables[0].Rows[0].MField<decimal>("NotIssued");
            obj.MValue = num.ToString();
            list.Add(obj);
            NameValueModel obj2 = new NameValueModel
            {
                MName = "1"
            };
            num = dataSet.Tables[0].Rows[0].MField<decimal>("PartlyIssued");
            obj2.MValue = num.ToString();
            list.Add(obj2);
            NameValueModel obj3 = new NameValueModel
            {
                MName = "2"
            };
            num = dataSet.Tables[0].Rows[0].MField<decimal>("FullIssued");
            obj3.MValue = num.ToString();
            list.Add(obj3);
            return list;
        }

        public int GetNextTableNumber(MContext ctx, int invoiceType)
        {
            List<FPTableViewModel> tableViewModelList = GetTableViewModelList(ctx, new FPTableViewFilterModel
            {
                MInvoiceType = invoiceType.ToString(),
                rows = 0
            });
            tableViewModelList = (from x in tableViewModelList
                                  orderby int.Parse(x.MNumber)
                                  select x).ToList();
            for (int i = 1; i < 99999999; i++)
            {
                if (tableViewModelList.Count < i || tableViewModelList[i - 1] == null || (tableViewModelList[i - 1] != null && int.Parse(tableViewModelList[i - 1].MNumber) > i))
                {
                    return i;
                }
            }
            return 0;
        }

        public string GetTabelFullNumber(int invoiceType, int number)
        {
            return ((invoiceType == 0) ? "SFP" : "PFP") + "0000".Substring(number.ToString().Length) + number.ToString();
        }

        public OperationResult SaveTable(MContext ctx, FPTableViewModel table)
        {
            table.IsNew = string.IsNullOrWhiteSpace(table.MItemID);
            OperationResult operationResult = new OperationResult
            {
                Success = true
            };
            if (table.MBizDate.Year == 1900)
            {
                table.MBizDate = ctx.DateNow;
            }
            table.fapiaoList = (table.fapiaoList ?? new List<FPFapiaoModel>());
            if (IsTableNumberDuplicated(ctx, table.MNumber, table.MBizDate, table.MItemID, table.MInvoiceType))
            {
                throw new MActionException
                {
                    Codes = new List<MActionResultCodeEnum>
                    {
                        MActionResultCodeEnum.MNumberInvalid
                    },
                    Messages = new List<string>
                    {
                        COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TableNumberOfThisMonthDuplicatedYouCanUse", "本月的开票单号重复了,你可以使用") + GetTabelFullNumber(table.MInvoiceType, GetNextTableNumber(ctx, table.MItemID, table.MInvoiceType))
                    }
                };
            }
            //List<BDContactsModel> contactList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).ContactList;
            //BDContactsModel contact = contactList.FirstOrDefault((BDContactsModel x) => x.MItemID == table.MContactID) ?? new BDContactsModel();

            ////BDContactsInfoModel contactViewData = BDContactManager.GetContactViewData(contactID);
            //contact = (BDContactsModel)MText.JsonEncode(contact);

            BASOrganisationRepository bASOrganisation = new BASOrganisationRepository();
            var org= bASOrganisation.GetDataModel(ctx, ctx.MOrgID);

        BDContactsRepository bDContacts = new BDContactsRepository();
            var contact=  bDContacts.GetContactModelByKey(table.MContactID,ctx );

            BDItemRepository bDItem = new BDItemRepository();
            BDBankAccountRepository bDBankAccount = new BDBankAccountRepository();

            table.fapiaoList.ForEach(delegate (FPFapiaoModel x)
            {
                x.MInvoiceType = table.MInvoiceType;

                //x.MPContactTaxCode = ((x.MInvoiceType == 0) ? contact.MTaxNo : ctx.MTaxCode);
                //x.MSContactTaxCode = ((x.MInvoiceType == 1) ? contact.MTaxNo : ctx.MTaxCode);

                x.MPContactTaxCode = ((x.MInvoiceType == 0) ? table.MContactTaxCode : ctx.MTaxCode);
                x.MSContactTaxCode = ((x.MInvoiceType == 1) ? contact.MTaxNo : ctx.MTaxCode);


                x.MPContactName = ((x.MInvoiceType == 0) ? contact.MName : ctx.MOrgName);
                x.MSContactName = ((x.MInvoiceType == 1) ? contact.MName : ctx.MOrgName);

               //var s= Encoding.UTF8.GetString(contact.MBankAcctNo as byte[])
                string arg2 = string.IsNullOrWhiteSpace(contact.MBankAcctNo) ? "" : contact.MBankAcctNo;
                string arg3 = string.IsNullOrWhiteSpace(contact.MBankAccName) ? "" : contact.MBankAccName;

                //x.MSContactAddressPhone = ((x.MInvoiceType == 1) ? contact.MPhone : ctx.MMobilePhone);
                //x.MPContactAddressPhone = ((x.MInvoiceType == 0) ? contact.MPhone : ctx.MMobilePhone);

                //****银行信息
                var bank = "";
                List<BDBankAccountEditModel> bankinfo = BDBankAccountRepository.GetBankAccountList(ctx, new string[] { table.MBankId });
                if (bankinfo!=null|| bankinfo.Count>0) {
                    bank = bankinfo[0].MOpeningBank + " " + bankinfo[0].MBankNo;
                }

                x.MPContactBankInfo = ((x.MInvoiceType == 0) ? arg3+" "+ arg2 : bank);
                x.MSContactBankInfo = ((x.MInvoiceType == 1) ? arg3 +" "+ arg2 : bank);

                x.MReceiver =  ((x.MInvoiceType == 1) ? contact.MName : ctx.MUserName);
                x.MDrawer = ((x.MInvoiceType == 1) ? contact.MName : ctx.MUserName);
                x.MReaduitor =  ((x.MInvoiceType == 1) ? contact.MName : ctx.MUserName);
                //x.MTitle = "MTitle";

                var item = bDItem.GetBDModelByKey(x.MItemID, ctx);

                x.MItemName = item.MText;


            });
            List<FPFapiaoModel> list = (from x in table.fapiaoList
                                        where string.IsNullOrWhiteSpace(x.MID)
                                        select x).ToList();
            List<FPFapiaoModel> list2 = (from x in table.fapiaoList
                                         where !string.IsNullOrWhiteSpace(x.MID) && x.MHasDetail
                                         select x).ToList();

            BASCountryRepository bASCountryRepository = new BASCountryRepository();
            List<BASCountryModel> countryList = bASCountryRepository. GetCountryList(ctx);

            var provinceList = bASCountryRepository.GetProvinceList(ctx,org.MCountryID);
            //List<BASProvinceModel> provinceList = GetProvinceList(org.MCountryID);
            var country = countryList.FirstOrDefault(i=>i.MItemID==org.MCountryID);
            var province = provinceList.FirstOrDefault(i=>i.MItemID==org.MStateID);

            list.ForEach(delegate (FPFapiaoModel x)
            {
                x.MType = table.MType;
                x.MOrgID = ctx.MOrgID;
                x.IsNew = true;
                x.MVerifyDate = x.MDeductionDate;
                x.MSource = 0;
                x.MContactID = table.MContactID;
                x.MReconcileStatus = 1;
                x.MAmount = x.MTotalAmount - x.MTaxAmount;
                x.MStatus = ((!(x.MTotalAmount < decimal.Zero)) ? 1 : 4);
                //contact.MRealCountryName+
                var address = contact.MRealRegion + contact.MRealCityID + contact.MRealStreet+" "+contact.MPhone;



                //country?.MCountryName+
                var saddress = province?.MName + org.MCityID + org.MStreet;// +" "+ctx.MMobilePhone;
                x.MPContactAddressPhone = ((x.MInvoiceType == 0) ? address : saddress);
                x.MSContactAddressPhone = ((x.MInvoiceType == 1) ? address : saddress);

                if (x.MInvoiceType == 0)
                {
                    x.MVerifyType = 3;
                }
                FPFapiaoEntryModel item = new FPFapiaoEntryModel
                {
                    MItemID = x.MItemID,
                    MAmount = x.MTotalAmount - x.MTaxAmount,
                    MTaxAmount = x.MTaxAmount,
                    MPrice = x.MTotalAmount - x.MTaxAmount,
                    MTotalAmount = x.MTotalAmount,
                    MItemName = x.MItemName,
                    MQuantity = decimal.One,
                    MTaxPercent = x.MTaxAmount * 100m / (x.MTotalAmount - x.MTaxAmount),

                };
                x.MFapiaoEntrys = new List<FPFapiaoEntryModel>
                {
                    item
                };
            });
            List<FPFapiaoModel> list3 = (from x in table.fapiaoList
                                         where !x.MHasDetail && !string.IsNullOrWhiteSpace(x.MID)
                                         select x).ToList();
            list3.ForEach(delegate (FPFapiaoModel x)
            {
                x.MOrgID = ctx.MOrgID;
                if (x.MFapiaoEntrys != null && x.MFapiaoEntrys.Any())
                {
                    x.MFapiaoEntrys[0].MItemID = x.MItemID;
                    x.MFapiaoEntrys[0].MItemName = x.MItemName;
                    x.MFapiaoEntrys[0].MAmount = x.MTotalAmount - x.MTaxAmount;
                    x.MFapiaoEntrys[0].MTaxAmount = x.MTaxAmount;
                    x.MFapiaoEntrys[0].MPrice = x.MTotalAmount - x.MTaxAmount;
                    x.MFapiaoEntrys[0].MQuantity = decimal.One;
                    x.MFapiaoEntrys[0].MTaxPercent = x.MTaxAmount * 100m / (x.MTotalAmount - x.MTaxAmount);
                }
            });
            list.AddRange(list3);
            FPTableModel tableModel = CastTableView2Table(table);
            tableModel.MOrgID = ctx.MOrgID;
            List<CommandInfo> cmds = new List<CommandInfo>();
            List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<FPTableModel>(ctx, tableModel, null, true);
            cmds.AddRange(insertOrUpdateCmd);
            if (list.Count > 0)
            {
                list.ForEach(delegate (FPFapiaoModel x)
                {
                    x.MTableID = tableModel.MItemID;
                    x.MType = table.MType;
                    fapiapDal.OperateVerifyType(x);
                });
                OperationResult updateFapiaoList = fapiapDal.GetUpdateFapiaoList(ctx, list);
                if (updateFapiaoList.Success)
                {
                    cmds.AddRange(updateFapiaoList.OperationCommands);
                    goto IL_0348;
                }
                return updateFapiaoList;
            }
            goto IL_0348;
            IL_0348:
            if (list2.Count > 0)
            {
                list2.ForEach(delegate (FPFapiaoModel x)
                {
                    x.MTableID = tableModel.MItemID;
                    x.MType = table.MType;
                    fapiapDal.OperateVerifyType(x);
                    cmds.AddRange(fapiapDal.GetSaveFapiaoTable(ctx, x));
                });
            }
            if (table.invoiceIdList != null && table.invoiceIdList.Count > 0)
            {
                List<IVInvoiceModel> invoices = new List<IVInvoiceModel>();
                table.invoiceIdList.ForEach(delegate (string x)
                {
                    invoices.Add(new IVInvoiceModel
                    {
                        MID = x
                    });
                    table.MItemID = tableModel.MItemID;
                });
                cmds.AddRange(GetSaveInvoiceTable(ctx, table, invoices));
            }
            cmds.AddRange(FPTableLogHelper.SaveCmdLog(ctx, table));
            if (cmds.Count > 0)
            {
                operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(cmds) > 0);
                operationResult.ObjectID = tableModel.MItemID;
            }
            return operationResult;
        }
        //public   List<BASCountryModel> GetCountryList()
        //{
        //    IBASCountry sysService = ServiceHostManager.GetSysService<IBASCountry>();
        //    using (sysService as IDisposable)
        //    {
        //        return sysService.GetCountryList(null).ResultData;
        //    }
        //}
        //public static List<BASProvinceModel> GetProvinceList(string countryId)
        //{
        //    IBASCountry sysService = ServiceHostManager.GetSysService<IBASCountry>();
        //    using (sysService as IDisposable)
        //    {
        //        return sysService.GetProvinceList(countryId, null).ResultData;
        //    }
        //}
        private List<CommandInfo> GetSaveInvoiceTable(MContext ctx, FPTableModel table, List<IVInvoiceModel> invoices)
        {
            List<CommandInfo> list = new List<CommandInfo>();
            List<FPInvoiceTableModel> list2 = new List<FPInvoiceTableModel>();
            for (int i = 0; i < invoices.Count; i++)
            {
                SqlWhere sqlWhere = new SqlWhere();
                sqlWhere.AddFilter("MInvoiceId", SqlOperators.Equal, invoices[i].MID).AddFilter("MTableID", SqlOperators.Equal, table.MItemID).AddFilter("MInvoiceType", SqlOperators.Equal, table.MInvoiceType);
                if (!ModelInfoManager.ExistsByFilter<FPInvoiceTableModel>(ctx, sqlWhere))
                {
                    list2.Add(new FPInvoiceTableModel
                    {
                        MTableID = table.MItemID,
                        MInvoiceID = invoices[i].MID,
                        MInvoiceType = table.MInvoiceType,
                        MOrgID = ctx.MOrgID
                    });
                }
            }
            if (list2.Count > 0)
            {
                List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, null, true);
                list.AddRange(insertOrUpdateCmds);
            }
            return list;
        }

        private List<IVInvoiceModel> GetInvoiceListByTableIds(MContext ctx, List<string> tableIds)
        {
            string sql = " select t1.MInvoiceID as MID, t1.MTableID from t_fp_invoice_table t1 where t1.MOrgID = @MOrgID and t1.MTableID in ('" + string.Join("','", tableIds.ToArray()) + "') and t1.MIsDelete = 0 ";
            MySqlParameter[] cmdParms = new MySqlParameter[1]
            {
                new MySqlParameter
                {
                    ParameterName = "@MOrgID",
                    Value = ctx.MOrgID
                }
            };
            return ModelInfoManager.GetDataModelBySql<IVInvoiceModel>(ctx, sql, cmdParms);
        }

        private List<IVInvoiceModel> GetInvoiceListWithAmountByTableIds(MContext ctx, List<string> tableIds)
        {
            string sql = "SELECT \n                        t1.MInvoiceID AS MID, t1.MTableID, t2.MTaxAmt, t2.MTaxTotalAmt, MTotalAmt\n                    FROM\n                        t_fp_invoice_table t1\n                            INNER JOIN\n                        t_iv_invoice t2 ON t1.MInvoiceID = t2.MID\n                            AND t1.MOrgID = t2.MOrgID\n                            AND t1.MIsDelete = t2.MIsDelete\n                    WHERE\n                        t1.MOrgID = @MOrgID\n                            AND t1.MTableID IN ('" + string.Join("' , '", tableIds.ToArray()) + "')\n                            AND t1.MIsDelete = 0";
            MySqlParameter[] cmdParms = new MySqlParameter[1]
            {
                new MySqlParameter
                {
                    ParameterName = "@MOrgID",
                    Value = ctx.MOrgID
                }
            };
            return ModelInfoManager.GetDataModelBySql<IVInvoiceModel>(ctx, sql, cmdParms);
        }

        private FPTableModel CastTableView2Table(FPTableViewModel view)
        {
            FPTableModel fPTableModel = new FPTableModel
            {
                MItemID = view.MItemID,
                IsNew = (view.MItemID == null && true),
                MAjustAmount = view.MAjustAmount,
                MAmount = view.MAmount,
                MTaxAmount = view.MTaxAmount,
                MBizDate = view.MBizDate,
                MContactID = view.MContactID,
                MContactTaxCode = view.MContactTaxCode,
                MType = view.MType,
                MExplanation = view.MExplanation,
                MTotalAmount = view.MTotalAmount,
                MNumber = view.MNumber,
                MBankId = view.MBankId,
                MOrgID = view.MOrgID,
                MRTotalAmount = view.MRTotalAmount,
                MRTaxAmount = view.MRTaxAmount,
                MRAmount = view.MRAmount,
                MInvoiceType = view.MInvoiceType
            };
            bool flag = fPTableModel.MTotalAmount >= decimal.Zero;
            view.fapiaoList = (view.fapiaoList ?? new List<FPFapiaoModel>());
            view.invoiceIdList = ((view.invoiceIdList == null || (from x in view.invoiceIdList
                                                                  where !string.IsNullOrWhiteSpace(x)
                                                                  select x).Count() == 0) ? new List<string>() : view.invoiceIdList);
            view.fapiaoIdList = ((view.fapiaoIdList == null || (from x in view.fapiaoIdList
                                                                where !string.IsNullOrWhiteSpace(x)
                                                                select x).Count() == 0) ? new List<string>() : view.fapiaoIdList);
            UpdateTableIssueStatus(fPTableModel, view.fapiaoList);
            return fPTableModel;
        }

        public void UpdateTableIssueStatus(FPTableModel table, List<FPFapiaoModel> fapiaos)
        {
            bool flag = table.MTotalAmount >= decimal.Zero;
            fapiaos = (fapiaos ?? new List<FPFapiaoModel>());
            decimal num = fapiaos.Sum((FPFapiaoModel x) => x.MTotalAmount);
            decimal mRTaxAmount = fapiaos.Sum((FPFapiaoModel x) => x.MTaxAmount);
            decimal mRAmount = fapiaos.Sum((FPFapiaoModel x) => x.MAmount);
            decimal num2 = table.MTotalAmount - table.MAjustAmount;
            table.MRAmount = mRAmount;
            table.MRTaxAmount = mRTaxAmount;
            table.MRTotalAmount = num;
            if (num == decimal.Zero && num2 == decimal.Zero)
            {
                goto IL_00f3;
            }
            if (flag && num >= num2)
            {
                goto IL_00f3;
            }
            int num3 = (!flag && num <= num2) ? 1 : 0;
            goto IL_00f4;
            IL_00f3:
            num3 = 1;
            goto IL_00f4;
            IL_00f4:
            if (num3 != 0)
            {
                table.MIssueStatus = 2;
            }
            else if (num == decimal.Zero && (num2 != decimal.Zero || table.MTaxAmount != decimal.Zero))
            {
                table.MIssueStatus = 0;
            }
            else if ((flag && num < num2) || (!flag && num > num2))
            {
                table.MIssueStatus = 1;
            }
        }

        private string GetTableModelListQuerySql(MContext ctx, FPTableViewFilterModel filter, out List<MySqlParameter> parameters, bool getCount = false)
        {
            parameters = new List<MySqlParameter>
            {
                new MySqlParameter
                {
                    ParameterName = "@MOrgID",
                    Value = ctx.MOrgID
                },
                new MySqlParameter
                {
                    ParameterName = "@MLocaleID",
                    Value = ctx.MLCID
                }
            };
            string str = "SELECT \r\n                            " + (getCount ? " count(t1.MItemID) as MCount " : " t1.*, CONVERT(AES_DECRYPT(t2.MName, '{0}') USING UTF8) AS MContactName ") + "\r\n                        FROM\r\n                            t_fp_table t1\r\n                                left JOIN\r\n                            t_bd_contacts_l t2 ON t1.MContactID = t2.MParentID\r\n                                AND t2.MLocaleID = @MLocaleID\r\n                                AND t1.MOrgID = t2.MOrgID\r\n                                AND t2.MIsDelete = t1.MIsDelete\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 ";
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                str += " and ( t1.MNumber like concat('%',@Keyword,'%' ) or \r\n                                t1.MTotalAmount like concat('%',@Keyword,'%' ) or  \r\n                                t1.MAjustAmount like concat('%',@Keyword,'%' ) or  \r\n                                t1.MTaxAmount like concat('%',@Keyword,'%' ) or \r\n                                CONVERT( AES_DECRYPT(t2.MName, '{0}') USING UTF8) like concat('%',@Keyword,'%' ) or\r\n                                t1.MExplanation like concat('%',@Keyword,'%')) ";
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Keyword",
                    Value = filter.Keyword
                });
            }
            if (!string.IsNullOrWhiteSpace(filter.MItemID))
            {
                str += " and t1.MItemID = @MItemID ";
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@MItemID",
                    Value = filter.MItemID
                });
            }
            if (filter.MTableDate > new DateTime(1900, 1, 1))
            {
                str += " and t1.MBizDate between @BStartDate and @BEndDate ";
                DateTime dateTime = filter.MTableDate;
                int year = dateTime.Year;
                dateTime = filter.MTableDate;
                filter.MTableDate = new DateTime(year, dateTime.Month, 1);
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@BStartDate",
                    Value = (object)filter.MTableDate
                });
                List<MySqlParameter> obj = parameters;
                MySqlParameter obj2 = new MySqlParameter
                {
                    ParameterName = "@BEndDate"
                };
                dateTime = filter.MTableDate;
                dateTime = dateTime.AddMonths(1);
                obj2.Value = dateTime.AddDays(-1.0);
                obj.Add(obj2);
            }
            if (filter.MStartDate > new DateTime(1900, 1, 1))
            {
                str += " and t1.MBizDate >= @StartDate ";
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@StartDate",
                    Value = (object)filter.MStartDate
                });
            }
            if (filter.MEndDate > new DateTime(1900, 1, 1))
            {
                str += " and t1.MBizDate <= @EndDate ";
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@EndDate",
                    Value = (object)filter.MEndDate
                });
            }
            if (!string.IsNullOrWhiteSpace(filter.NumberAmount))
            {
                str += " and (t1.MTotalAmount = @Amount Or t1.MNumber  = @Number)";
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Amount",
                    Value = filter.NumberAmount
                });
                int num = 0;
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Number",
                    Value = (object)(int.TryParse(filter.NumberAmount, out num) ? num : 0)
                });
            }
            if (!string.IsNullOrWhiteSpace(filter.MFapiaoType) && filter.MFapiaoType != "-1")
            {
                str += " and t1.MType = @MType";
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@MType",
                    Value = filter.MFapiaoType
                });
            }
            if (!string.IsNullOrWhiteSpace(filter.MInvoiceType))
            {
                str += " and t1.MInvoiceType = @MInvoiceType";
                parameters.Add(new MySqlParameter
                {
                    ParameterName = "@MInvoiceType",
                    Value = (object)int.Parse(filter.MInvoiceType)
                });
            }
            if (!string.IsNullOrWhiteSpace(filter.MIssueStatus))
            {
                string[] array = filter.MIssueStatus.Split(',');
                if (array.Count() == 0)
                {
                    str += " and t1.MIssueStatus = @MIssueStatus";
                    parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@MIssueStatus",
                        Value = filter.MIssueStatus
                    });
                }
                else
                {
                    str = str + " and t1.MIssueStatus in (" + string.Join(",", array) + ") ";
                }
            }
            str += " order by cast(t1.MNumber as signed) desc, t1.MBizDate desc ";
            if (!getCount && filter.rows > 0)
            {
                str = str + " limit " + (filter.page - 1) * filter.rows + "," + filter.rows + " ";
            }
            return string.Format(str, "JieNor-001");
        }

        public int GetTableModelListCount(MContext ctx, FPTableViewFilterModel filter)
        {
            List<MySqlParameter> list = new List<MySqlParameter>();
            string tableModelListQuerySql = GetTableModelListQuerySql(ctx, filter, out list, true);
            return int.Parse(new DynamicDbHelperMySQL(ctx).GetSingle(tableModelListQuerySql, list.ToArray()).ToString());
        }

        public List<FPTableViewModel> GetTableModelList(MContext ctx, FPTableViewFilterModel filter)
        {
            List<MySqlParameter> list = new List<MySqlParameter>();
            string tableModelListQuerySql = GetTableModelListQuerySql(ctx, filter, out list, false);
            return ModelInfoManager.GetDataModelBySql<FPTableViewModel>(ctx, tableModelListQuerySql, list.ToArray());
        }

        public FPTableViewModel GetViewModelByTableID(MContext ctx, string tableId)
        {
            List<MySqlParameter> list = new List<MySqlParameter>
            {
                new MySqlParameter
                {
                    ParameterName = "@MOrgID",
                    Value = ctx.MOrgID
                },
                new MySqlParameter
                {
                    ParameterName = "@MLocaleID",
                    Value = ctx.MLCID
                },
                new MySqlParameter
                {
                    ParameterName = "@MItemID",
                    Value = tableId
                }
            };
            string format = "SELECT \r\n                            t1.*,\r\n                            CONVERT( AES_DECRYPT(t2.MName, '{0}') USING UTF8) AS MContactName\r\n                        FROM\r\n                            t_fp_table t1\r\n                                INNER JOIN\r\n                            t_bd_contacts_l t2 ON t1.MContactID = t2.MParentID\r\n                                AND t1.MOrgID = t2.MOrgID\r\n                                AND t2.MLocaleID = @MLocaleID\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID\r\n                                AND t1.MItemID = @MItemID\r\n                                AND t1.MIsDelete = 0\r\n                                AND t2.MIsDelete = 0";
            format = string.Format(format, "JieNor-001");
            UpdateTableIssueStatus(ctx, new List<string>
            {
                tableId
            }, null);
            return ModelInfoManager.GetDataModel<FPTableViewModel>(ctx, format, list.ToArray());
        }

        public FPTableViewModel GetTableViewModelByInvoiceID(MContext ctx, string invoiceId)
        {
            List<MySqlParameter> list = new List<MySqlParameter>
            {
                new MySqlParameter
                {
                    ParameterName = "@MOrgID",
                    Value = ctx.MOrgID
                },
                new MySqlParameter
                {
                    ParameterName = "@MInvoiceID",
                    Value = invoiceId
                }
            };
            string sql = "SELECT \r\n                            t1.*, SUM(t4.MTotalAmount) AS IssuedAmount\r\n                        FROM\r\n                            t_fp_table t1\r\n                                INNER JOIN\r\n                            t_fp_invoice_table t2 ON t1.MItemID = t2.MTableID\r\n                                AND t2.MOrgID = t1.MOrgID\r\n                                AND t2.MIsDelete = 0\r\n                                LEFT JOIN\r\n                            t_fp_fapiao_table t3 ON t2.MTableID = t3.MTableID\r\n                                AND t3.MOrgID = t1.MOrgID\r\n                                AND t3.MIsDelete = 0\r\n                                LEFT JOIN\r\n                            T_FP_Fapiao t4 ON t4.MID = t3.MFapiaoID\r\n                                AND t4.MOrgID = t1.MOrgID\r\n                                AND t4.MIsDelete = 0\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID\r\n                                AND t2.MInvoiceID = @MInvoiceID\r\n                                AND t1.MIsDelete = 0 ";
            return ModelInfoManager.GetDataModel<FPTableViewModel>(ctx, sql, list.ToArray());
        }

        public List<FPFapiaoModel> GetFapiaoListByTableInvoice(MContext ctx, string tableId, string invoiceIds)
        {
            List<FPFapiaoModel> list = new List<FPFapiaoModel>();
            if (!string.IsNullOrWhiteSpace(tableId))
            {
                return fapiapDal.GetFapiaoListDataByTableIds(ctx, new List<string>
                {
                    tableId
                });
            }
            if (!string.IsNullOrWhiteSpace(invoiceIds))
            {
                List<string> tableIdListByInvoiceIds = GetTableIdListByInvoiceIds(ctx, invoiceIds);
                list.AddRange(fapiapDal.GetFapiaoListDataByTableIds(ctx, tableIdListByInvoiceIds));
            }
            return list;
        }

        public List<string> GetTableIdListByInvoiceIds(MContext ctx, string invoiceIds)
        {
            string[] value = invoiceIds.Split(',');
            string sql = "SELECT \r\n                            MTableID AS MItemID\r\n                        FROM\r\n                            t_fp_invoice_table t1\r\n                        WHERE\r\n                            t1.MInvoiceId IN ('" + string.Join("' , '", value) + "')\r\n                                AND t1.MIsDelete = 0\r\n                                AND t1.MOrgID = '" + ctx.MOrgID + "' \r\n                                AND Length(ifnull(t1.MTableID,'')) > 0 \r\n                                AND exists(\r\n                                    SELECT \r\n                                        1\r\n                                    FROM\r\n                                        t_fp_table t2\r\n                                    WHERE\r\n                                        t2.MItemID = t1.MTableID\r\n                                            AND t2.MOrgID = '" + ctx.MOrgID + "'\r\n                                            AND t2.MIsDelete = 0)";
            return (from x in ModelInfoManager.GetDataModelBySql<FPTableModel>(ctx, sql, new MySqlParameter[0])
                    select x.MItemID).ToList();
        }

        public List<FPTableModel> GetTableListByInvoiceIds(MContext ctx, string invoiceIds)
        {
            string[] value = invoiceIds.Split(',');
            string sql = "SELECT \r\n                            t2.* \r\n                        FROM\r\n                            t_fp_invoice_table t1\r\n                            inner join \r\n                            t_fp_table t2\r\n                            on t1.MTableId = t2.MItemID and t1.MOrgID = t2.MOrgID and t1.MIsDelete = t2.MIsDelete \r\n                        WHERE\r\n                            t1.MInvoiceId IN ('" + string.Join("' , '", value) + "')\r\n                                AND t1.MIsDelete = 0\r\n                                AND t1.MOrgID = '" + ctx.MOrgID + "' ";
            return ModelInfoManager.GetDataModelBySql<FPTableModel>(ctx, sql, new MySqlParameter[0]);
        }

        public List<FPTableModel> GetTableListByFapiaoIds(MContext ctx, List<string> fapiaoIds)
        {
            List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
            string sql = "SELECT \r\n                            t2.* \r\n                        FROM\r\n                            t_fp_fapiao_table t1\r\n                            inner join \r\n                            t_fp_table t2\r\n                            on t1.MTableId = t2.MItemID and t1.MOrgID = t2.MOrgID and t1.MIsDelete = t2.MIsDelete \r\n                        WHERE\r\n                            t1.MFapiaoID " + GLUtility.GetInFilterQuery(fapiaoIds, ref list, "M_ID") + "\r\n                                AND t1.MIsDelete = 0\r\n                                AND t1.MOrgID = @MOrgID ";
            return ModelInfoManager.GetDataModelBySql<FPTableModel>(ctx, sql, list.ToArray());
        }

        public List<string> GetTableIdListByFapiaoIds(MContext ctx, List<string> fapiaoIds)
        {
            string sql = "SELECT \r\n                            MTableID AS MItemID\r\n                        FROM\r\n                            t_fp_fapiao_table t1\r\n                        WHERE\r\n                            t1.MFapiaoID IN ('" + string.Join("' , '", fapiaoIds) + "')\r\n                                AND LENGTH(IFNULL(t1.MTableID, '')) > 0\r\n                                AND t1.MIsDelete = 0\r\n                                AND t1.MOrgID = '" + ctx.MOrgID + "'\r\n                                AND EXISTS( SELECT \r\n                                    1\r\n                                FROM\r\n                                    t_fp_table t2\r\n                                WHERE\r\n                                    t2.MItemID = t1.MTableID\r\n                                        AND t2.MOrgID = '" + ctx.MOrgID + "'\r\n                                        AND t2.MIsDelete = 0)";
            return (from x in ModelInfoManager.GetDataModelBySql<FPTableModel>(ctx, sql, new MySqlParameter[0])
                    select x.MItemID).Distinct().ToList();
        }

        public OperationResult DeleteTableByInvoiceIds(MContext ctx, string invoiceIds)
        {
            List<string> list = invoiceIds.Split(',').ToList();
            List<FPTableModel> tableListByInvoiceIds = GetTableListByInvoiceIds(ctx, invoiceIds);
            List<string> tableIds = (from x in tableListByInvoiceIds
                                     select x.MItemID).Distinct().ToList();
            new GLUtility().ValidateFapiaoTableCanDelete(ctx, tableIds);
            List<CommandInfo> updateTableAmountByDeleteInvocieIDs = GetUpdateTableAmountByDeleteInvocieIDs(ctx, tableListByInvoiceIds, list);
            string commandText = "update t_fp_invoice_table t set t.MIsDelete = 1 \r\n                where t.MInvoiceId in ('" + string.Join("','", list) + "') \r\n                and t.MIsDelete = 0 \r\n                and t.MOrgID = @MOrgID ";
            List<CommandInfo> list2 = new List<CommandInfo>();
            CommandInfo obj = new CommandInfo
            {
                CommandText = commandText
            };
            DbParameter[] array = obj.Parameters = ctx.GetParameters((MySqlParameter)null);
            list2.Add(obj);
            List<CommandInfo> list3 = list2;
            list3.AddRange(updateTableAmountByDeleteInvocieIDs);
            OperationResult operationResult = new OperationResult
            {
                Success = true
            };
            operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list3) > 0);
            return operationResult;
        }

        public List<CommandInfo> GetUpdateTableAmountByDeleteInvocieIDs(MContext ctx, List<FPTableModel> tables, List<string> invoiceIds)
        {
            List<CommandInfo> list = new List<CommandInfo>();
            List<IVInvoiceModel> invoiceListWithAmountByTableIds = GetInvoiceListWithAmountByTableIds(ctx, (from x in tables
                                                                                                            select x.MItemID).Distinct().ToList());
            int i;
            for (i = 0; i < tables.Count; i++)
            {
                List<IVInvoiceModel> list2 = (from x in invoiceListWithAmountByTableIds
                                              where x.MTableID == tables[i].MItemID && !invoiceIds.Contains(x.MID)
                                              select x).ToList();
                if (list2.Count == 0)
                {
                    list.AddRange(ModelInfoManager.GetDeleteCmd<FPTableModel>(ctx, tables[i].MItemID));
                    continue;
                }
                tables[i].MTaxAmount = list2.Sum((IVInvoiceModel x) => x.InvoiceEntry.Sum((IVInvoiceEntryModel y) => y.MTaxAmt));
                tables[i].MTotalAmount = list2.Sum((IVInvoiceModel x) => x.MTaxTotalAmt);
                tables[i].MAmount = list2.Sum((IVInvoiceModel x) => x.MTotalAmt);
                bool flag = tables[i].MTotalAmount > decimal.Zero;
                decimal mRTotalAmount = tables[i].MRTotalAmount;
                decimal num = tables[i].MTotalAmount - tables[i].MAjustAmount;
                if (mRTotalAmount == decimal.Zero && num == decimal.Zero)
                {
                    goto IL_024e;
                }
                if (flag && mRTotalAmount >= num)
                {
                    goto IL_024e;
                }
                int num2 = (!flag && mRTotalAmount <= num) ? 1 : 0;
                goto IL_024f;
                IL_024f:
                if (num2 != 0)
                {
                    tables[i].MIssueStatus = 2;
                }
                else if (mRTotalAmount == decimal.Zero && (num != decimal.Zero || tables[i].MTaxAmount != decimal.Zero))
                {
                    tables[i].MIssueStatus = 0;
                }
                else if ((flag && mRTotalAmount < num) || (!flag && mRTotalAmount > num))
                {
                    tables[i].MIssueStatus = 1;
                }
                list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPTableModel>(ctx, tables[i], new List<string>
                {
                    "MIssueStatus",
                    "MTaxAmount",
                    "MTotalAmount",
                    "MAmount"
                }, true));
                continue;
                IL_024e:
                num2 = 1;
                goto IL_024f;
            }
            return list;
        }

        public List<CommandInfo> GetDeleteReconcileCmds(MContext ctx, List<string> fapiaoIDs = null, List<string> tableIDs = null)
        {
            List<CommandInfo> list = new List<CommandInfo>();
            if (fapiaoIDs?.Any() ?? false)
            {
                List<CommandInfo> list2 = list;
                CommandInfo obj = new CommandInfo
                {
                    CommandText = " update t_fp_fapiao set MReconcileStatus = 0 where MID in ('" + string.Join("','", fapiaoIDs) + "') and MOrgID = @MOrgID and MIsDelete = 0 "
                };
                DbParameter[] array = obj.Parameters = ctx.GetParameters((MySqlParameter)null);
                list2.Add(obj);
                List<CommandInfo> list3 = list;
                CommandInfo obj2 = new CommandInfo
                {
                    CommandText = " update t_fp_fapiao_table set MIsDelete = 1 where MFapiaoID in ('" + string.Join("','", fapiaoIDs) + "') and MOrgID = @MOrgID and MIsDelete = 0 "
                };
                array = (obj2.Parameters = ctx.GetParameters((MySqlParameter)null));
                list3.Add(obj2);
            }
            if (tableIDs?.Any() ?? false)
            {
                List<CommandInfo> list4 = list;
                CommandInfo obj3 = new CommandInfo
                {
                    CommandText = " update t_fp_fapiao set MReconcileStatus = 0 where MID in ( select MFapiaoID from t_fp_fapiao_table where MTableID in ('" + string.Join("','", tableIDs) + "')  and MIsDelete = 0 and MOrgID = @MOrgID ) and MOrgID = @MOrgID and MIsDelete = 0 "
                };
                DbParameter[] array = obj3.Parameters = ctx.GetParameters((MySqlParameter)null);
                list4.Add(obj3);
                List<CommandInfo> list5 = list;
                CommandInfo obj4 = new CommandInfo
                {
                    CommandText = " update t_fp_fapiao_table set MIsDelete = 1 where MTableID in ('" + string.Join("','", tableIDs) + "') and MOrgID = @MOrgID and MIsDelete = 0 "
                };
                array = (obj4.Parameters = ctx.GetParameters((MySqlParameter)null));
                list5.Add(obj4);
            }
            return list;
        }

        public OperationResult FPAddLog(MContext ctx, FPFapiaoModel model)
        {
            OptLog.AddLog(OptLogTemp(model), ctx, model.MID, model.MDesc);
            return new OperationResult
            {
                Success = true
            };
        }

        public OptLogTemplate OptLogTemp(FPFapiaoModel model)
        {
            switch (model.MType)
            {
                case 0:
                    return OptLogTemplate.FaPiao_Note;
                case 1:
                    return OptLogTemplate.Sale_FaPiao_Note;
                case 2:
                    return OptLogTemplate.GL_Voucher_Note;
                default:
                    return OptLogTemplate.None;
            }
        }

        public OperationResult DeleteTableByTableIds(MContext ctx, string tableIds)
        {
            List<string> list = tableIds.Split(',').ToList();
            new GLUtility().ValidateFapiaoTableCanDelete(ctx, list);
            List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
            string str = "update t_fp_table t set t.MIsDelete = 1 where  t.MIsDelete = 0 and t.MOrgID = @MOrgID and t.MItemID ";
            str += GLUtility.GetInFilterQuery(list, ref list2, "M_ID");
            List<MySqlParameter> list3 = ctx.GetParameters((MySqlParameter)null).ToList();
            string format = "update t_fp_fapiao t set t.MReconcileStatus = 0  where t.MID in (select MFapiaoID from t_fp_fapiao_table where MTableID {0} and MIsDelete = 0 and MOrgID = @MOrgID ) and MOrgID = @MOrgID and MIsDelete = 0 ";
            format = string.Format(format, GLUtility.GetInFilterQuery(list, ref list3, "M_ID"));
            List<MySqlParameter> list4 = ctx.GetParameters((MySqlParameter)null).ToList();
            string format2 = "update t_fp_invoice_table t set t.MIsDelete = 1  where t.MTableId {0} and t.MIsDelete = 0 and t.MOrgID = @MOrgID ";
            format2 = string.Format(format2, GLUtility.GetInFilterQuery(list, ref list4, "M_ID"));
            List<MySqlParameter> list5 = ctx.GetParameters((MySqlParameter)null).ToList();
            string format3 = "update t_fp_fapiao_table t set t.MIsDelete = 1  where t.MTableId {0} and t.MIsDelete = 0 and t.MOrgID = @MOrgID ";
            format3 = string.Format(format3, GLUtility.GetInFilterQuery(list, ref list5, "M_ID"));
            List<CommandInfo> list6 = new List<CommandInfo>();
            CommandInfo obj = new CommandInfo
            {
                CommandText = str
            };
            DbParameter[] array = obj.Parameters = list2.ToArray();
            list6.Add(obj);
            CommandInfo obj2 = new CommandInfo
            {
                CommandText = format
            };
            array = (obj2.Parameters = list4.ToArray());
            list6.Add(obj2);
            CommandInfo obj3 = new CommandInfo
            {
                CommandText = format2
            };
            array = (obj3.Parameters = list4.ToArray());
            list6.Add(obj3);
            CommandInfo obj4 = new CommandInfo
            {
                CommandText = format3
            };
            array = (obj4.Parameters = list5.ToArray());
            list6.Add(obj4);
            List<CommandInfo> cmdList = list6;
            return new OperationResult
            {
                Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(cmdList) > 0)
            };
        }

        public List<CommandInfo> GetUpdateTableIssueStatus(MContext ctx, List<string> tableIds, List<string> fapiaoIds = null, List<FPFapiaoModel> fapiaos = null)
        {
            List<CommandInfo> list = new List<CommandInfo>();
            if (tableIds == null && fapiaoIds != null && fapiaos == null)
            {
                tableIds = GetTableIdListByFapiaoIds(ctx, fapiaoIds);
            }
            else if (tableIds == null && fapiaos != null)
            {
                tableIds = GetTableIdListByFapiaoIds(ctx, (from x in fapiaos
                                                           select x.MID).ToList());
            }
            if (tableIds != null && tableIds.Count > 0)
            {
                List<string> list2 = (fapiaos == null) ? new List<string>() : (from x in fapiaos
                                                                               select x.MID).ToList();
                List<FPFapiaoModel> fapiaoListDataByTableIds = fapiapDal.GetFapiaoListDataByTableIds(ctx, tableIds);
                List<FPTableModel> modelList = GetModelList(ctx, new SqlWhere().In("MItemID", tableIds), false);
                for (int j = 0; j < modelList.Count; j++)
                {
                    FPTableModel currenTable = modelList[j];
                    List<FPFapiaoModel> currentFapiaos = (from x in fapiaoListDataByTableIds
                                                          where x.MTableID == currenTable.MItemID
                                                          select x).ToList();
                    if (fapiaos != null && list2.Count > 0)
                    {
                        if (currentFapiaos.Count == 0)
                        {
                            currentFapiaos.AddRange((from x in fapiaos
                                                     where x.MTableID == currenTable.MItemID
                                                     select x).ToList());
                        }
                        else
                        {
                            int i;
                            for (i = 0; i < currentFapiaos.Count; i++)
                            {
                                if (list2.Contains(currentFapiaos[i].MID))
                                {
                                    FPFapiaoModel fPFapiaoModel = fapiaos.FirstOrDefault((FPFapiaoModel x) => x.MID == currentFapiaos[i].MID);
                                    if (fPFapiaoModel != null)
                                    {
                                        currentFapiaos[i] = fPFapiaoModel;
                                    }
                                }
                            }
                        }
                    }
                    UpdateTableIssueStatus(currenTable, (from x in currentFapiaos
                                                         where !x.MIsDelete
                                                         select x).ToList());
                    list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPTableModel>(ctx, currenTable, new List<string>
                    {
                        "MIssueStatus",
                        "MRAmount",
                        "MRTaxAmount",
                        "MRTotalAmount"
                    }, true));
                }
            }
            return list;
        }

        public OperationResult UpdateTableIssueStatus(MContext ctx, List<string> tableIds, List<string> fapiaoIds = null)
        {
            if (tableIds == null && fapiaoIds != null)
            {
                tableIds = GetTableIdListByFapiaoIds(ctx, fapiaoIds);
            }
            OperationResult operationResult = new OperationResult();
            if (tableIds != null && (from x in tableIds
                                     where !string.IsNullOrWhiteSpace(x)
                                     select x).Count() > 0)
            {
                List<CommandInfo> list = new List<CommandInfo>();
                List<FPFapiaoModel> fapiaoListDataByTableIds = fapiapDal.GetFapiaoListDataByTableIds(ctx, tableIds);
                int i;
                for (i = 0; i < tableIds.Count; i++)
                {
                    FPTableModel dataModel = GetDataModel(ctx, tableIds[i], false);
                    if (dataModel != null)
                    {
                        List<FPFapiaoModel> fapiaos = (from x in fapiaoListDataByTableIds
                                                       where x.MTableID == tableIds[i]
                                                       select x).ToList();
                        UpdateTableIssueStatus(dataModel, fapiaos);
                        list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPTableModel>(ctx, dataModel, new List<string>
                        {
                            "MIssueStatus",
                            "MRAmount",
                            "MRTaxAmount",
                            "MRTotalAmount"
                        }, true));
                    }
                }
                if (list.Count > 0)
                {
                    operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) > 0);
                }
            }
            return operationResult;
        }

        public DataTable GetNotReconcileTableSummaryByDate(MContext ctx, FPFapiaoFilterModel filter)
        {
            DataTable result = new DataTable();
            string sql = "SELECT\r\n\t                        MBizDate,\r\n\t                        MONTH (MBizDate) as MONTH,\r\n\t                        SUM(MtotalAmount) - SUM(MRTotalAmount) - Sum(MAjustAmount) AS MTotalAmount\r\n                           FROM\r\n\t                        t_fp_table\r\n                           WHERE\r\n\t                        MOrgID = @MOrgID\r\n                            " + ((filter.MFapiaoCategory != -1) ? " AND MInvoiceType = @MFapiaoCategory " : "") + "\r\n                            AND MBizDate>=@StartDate and MBizDate<=@EndDate\r\n                            AND MIsDelete = 0\r\n                            GROUP BY\r\n\t                            (YEAR(MBizDate) + MONTH(MBizDate))\r\n                                ORDER BY\r\n\t                            MBizDate";
            List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
            list.AddRange(new List<MySqlParameter>
            {
                new MySqlParameter("@StartDate", filter.MStartDate),
                new MySqlParameter("@EndDate", filter.MEndDate),
                new MySqlParameter("@MFapiaoCategory", filter.MFapiaoCategory)
            });
            DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, list.ToArray());
            if (dataSet != null && dataSet.Tables != null)
            {
                result = dataSet.Tables[0];
            }
            return result;
        }

        public DataTable GetYtdTabelSummaryAmount(MContext ctx, int year, FPTableViewFilterModel filter)
        {
            DataTable result = new DataTable();
            string sql = "\r\n                            SELECT\r\n\t                            '1' AS SummaryType,\r\n\t                            SUM(MtotalAmount) - SUM(MAjustAmount) AS MTotalAmount ,\r\n\t                            SUM(MTaxAmount) AS MTotalTaxAmount\r\n                            FROM\r\n\t                            t_fp_table\r\n                            WHERE\r\n\t                            YEAR (MBizDate) = @Year \r\n                                AND MOrgID=@MOrgID\r\n                                AND MInvoiceType = @FapiaoType \r\n                                AND MIsDelete=0\r\n                            UNION\r\n                            SELECT '2' AS SummaryType ,SUM(MTotalAmount) as MTotalAmount,SUM(MTaxAmount) AS MTotalTaxAmount\r\n                            FROM\r\n\t                        t_fp_fapiao\r\n                            WHERE\r\n                                YEAR (MBizDate) = @Year\r\n                                AND MOrgID = @MOrgID\r\n                                AND (MStatus = 1 OR MStatus=2 OR MStatus=3 OR MStatus=4)\r\n                                AND MInvoiceType = @FapiaoType\r\n                                AND MIsDelete = 0\r\n\t                       ";
            List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
            list.AddRange(new List<MySqlParameter>
            {
                new MySqlParameter("@Year", year),
                new MySqlParameter("@FapiaoType", Convert.ToInt32(filter.MInvoiceType))
            });
            DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, list.ToArray());
            if (dataSet != null && dataSet.Tables != null)
            {
                result = dataSet.Tables[0];
            }
            return result;
        }

        public DataTable GetSupplierUnReconcileFapiaoTop(MContext ctx, DateTime dateTime, int top)
        {
            DataTable result = new DataTable();
            string sql = string.Format("SELECT\r\n\t                                        v.*,\r\n\t                                        convert(AES_DECRYPT(a.MName,'{0}') using utf8) AS ContactName\r\n                                          FROM\r\n\t                                      (\r\n\t\t                                        SELECT\r\n\t\t\t                                        Sum(MtotalAmount) - SUM(MAjustAmount) AS MTotalAmount,\r\n\t\t\t                                        MContactID\r\n\t\t                                        FROM\r\n\t\t\t                                        t_fp_table\r\n\t\t                                        WHERE\r\n\t\t\t                                        YEAR (MBizDate) = @YEAR\r\n\t\t                                            AND MInvoiceType = 1\r\n\t\t                                            AND MOrgID =@MOrgID\r\n                                                    AND MIsDelete=0 \r\n\t\t                                            AND MItemID NOT IN (\r\n\t\t\t                                            SELECT\r\n\t\t\t\t                                            MTableID\r\n\t\t\t                                            FROM\r\n\t\t\t\t                                            t_fp_fapiao_table\r\n\t\t\t                                            WHERE\r\n\t\t\t\t                                            MIsDelete = 0\r\n\t\t\t                                                AND MFapiaoType = 1\r\n\t\t\t                                                AND MOrgID =@MOrgID\r\n\t\t                                            )\r\n                                                GROUP BY MContactID\r\n\t\t                                        ORDER BY\r\n\t\t\t                                        Sum(MtotalAmount) DESC\r\n\t\t                                        LIMIT 0,{1}\r\n\t                                        ) v\r\n                                           INNER JOIN t_bd_contacts_l a ON v.MContactID = a.MParentID  and a.MIsDelete=0 and a.MLocaleID=@MLCID \r\n                                            ", "JieNor-001", top);
            List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
            list.AddRange(new List<MySqlParameter>
            {
                new MySqlParameter("@YEAR", dateTime.Year)
            });
            DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, list.ToArray());
            if (dataSet != null && dataSet.Tables != null)
            {
                result = dataSet.Tables[0];
            }
            return result;
        }
    }
}
