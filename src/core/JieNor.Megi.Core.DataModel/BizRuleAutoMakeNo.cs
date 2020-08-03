using Fasterflect;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleAutoMakeNo : BizRuleBase
	{
		public OperateTime CurrentOperateContent
		{
			get;
			set;
		}

		[DataMember]
		public override string RuleName
		{
			get
			{
				return "自动编码";
			}
		}

		public BizRuleAutoMakeNo(string propertyName, string properDesc)
			: base(propertyName, properDesc)
		{
		}

		public override void Verification(MContext ctx)
		{
			if (CurrentOperateContent == base.OperateContent && base.BizData != null)
			{
				object propertyValue = base.BizData.GetPropertyValue(base.PropertyName);
				if (CheckRepeatOrExist(propertyValue))
				{
					CodingRuleModel codeingRule = GetCodeingRule(ctx);
					if (codeingRule != null)
					{
						string billNo = GetBillNo(codeingRule);
						if (!string.IsNullOrWhiteSpace(billNo))
						{
							base.BizData.SetPropertyValue(base.PropertyName, billNo);
						}
					}
				}
			}
		}

		private bool CheckRepeatOrExist(object billNoValue)
		{
			if (billNoValue == null || string.IsNullOrWhiteSpace(billNoValue.ToString()))
			{
				return true;
			}
			string text = string.Format("select count(*) from {0} Where {1} ='{2}' ", base.BizData.TableName, base.PropertyName, billNoValue.ToString().Trim());
			if (base.BizData.IsUpdate || !string.IsNullOrWhiteSpace(base.BizData.PKFieldValue))
			{
				text += string.Format(" And {0} != '{1}' ", base.BizData.PKFieldName, base.BizData.PKFieldValue);
			}
			if (base.BizData.GetType().GetProperties().Any((PropertyInfo f) => f.Name.EqualsIgnoreCase("MOrgID")) && base.BizData.TryGetPropertyValue("MOrgID") != null)
			{
				text += string.Format(" And MOrgID = '{0}' ", base.BizData.GetPropertyValue("MOrgID").ToString());
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(base.MContext);
			return dynamicDbHelperMySQL.Exists(text.ToString());
		}

		private string GetBillNo(CodingRuleModel rule)
		{
			List<string> list = new List<string>();
			string result = "";
			IOrderedEnumerable<CodingRuleEntryModel> orderedEnumerable = from f in rule.CodingRuleEntries
			orderby f.MSeq
			select f;
			foreach (CodingRuleEntryModel item in orderedEnumerable)
			{
				string mDataType = item.MDataType;
				CodeElementType codeElementType = CodeElementType.Prefix;
				if (mDataType.EqualsIgnoreCase(codeElementType.ToString()))
				{
					list.Add(item.MValue);
				}
				else
				{
					string mDataType2 = item.MDataType;
					codeElementType = CodeElementType.Txt;
					if (mDataType2.EqualsIgnoreCase(codeElementType.ToString()))
					{
						object propertyValue = base.BizData.GetPropertyValue(item.MFieldName);
						if (propertyValue != null)
						{
							list.Add(propertyValue.ToString());
						}
					}
					else
					{
						string mDataType3 = item.MDataType;
						codeElementType = CodeElementType.Date;
						if (mDataType3.EqualsIgnoreCase(codeElementType.ToString()))
						{
							object propertyValue2 = base.BizData.GetPropertyValue(item.MFieldName);
							if (propertyValue2 != null)
							{
								DateTime now = DateTime.Now;
								if (DateTime.TryParse(propertyValue2.ToString(), out now))
								{
									list.Add(now.ToString(item.MFormat));
								}
							}
						}
						else
						{
							string mDataType4 = item.MDataType;
							codeElementType = CodeElementType.Serial;
							if (mDataType4.EqualsIgnoreCase(codeElementType.ToString()))
							{
								list.Add(GetMaxSerialIndex(rule, item));
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				result = string.Join("", list);
			}
			return result;
		}

		private string GetMaxSerialIndex(CodingRuleModel rule, CodingRuleEntryModel ruleEntry)
		{
			long mSerial = ruleEntry.MSerialStart;
			BillNumberModel billNumberModel = new BillNumberModel();
			billNumberModel.MContext = base.MContext;
			billNumberModel.MRuleID = rule.MID;
			billNumberModel.MObjectType = rule.MObjectType;
			billNumberModel.MID = UUIDHelper.GetGuid();
			billNumberModel.MSerial = mSerial;
			billNumberModel.MOrgID = base.MContext.MOrgID;
			string sql = "select MID,MSerial+1 as MSerial From T_Bas_BillNumber Where MOrgID=@MOrgID And MRuleID=@MRuleID";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MRuleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = rule.MID;
			array[1].Value = base.MContext.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(base.MContext);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, array);
			if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
			{
				object obj = dataSet.Tables[0].Rows[0]["MSerial"];
				if (obj != null && long.TryParse(obj.ToString(), out mSerial))
				{
					billNumberModel.MID = dataSet.Tables[0].Rows[0]["MID"].ToString();
					billNumberModel.MSerial = mSerial;
					billNumberModel.IsUpdate = true;
					billNumberModel.IsNew = false;
				}
				else
				{
					billNumberModel.MSerial = mSerial;
					billNumberModel.IsUpdate = false;
					billNumberModel.IsNew = true;
				}
			}
			else
			{
				billNumberModel.MSerial = mSerial;
				billNumberModel.IsUpdate = false;
				billNumberModel.IsNew = true;
			}
			OperationResult operationResult = ModelInfoManager.InsertOrUpdate<BillNumberModel>(base.MContext, billNumberModel, null);
			return mSerial.ToString(ruleEntry.MFormat);
		}

		private CodingRuleModel GetCodeingRule(MContext ctx)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MObjectType", SqlOperators.Equal, base.BizData.GetType().Name);
			sqlWhere.AddDeleteFilter("MIsDelete", SqlOperators.Equal, false);
			sqlWhere.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
			List<CodingRuleModel> dataModelList = ModelInfoManager.GetDataModelList<CodingRuleModel>(ctx, sqlWhere, false, false);
			if (dataModelList == null || dataModelList.Count == 0)
			{
				return null;
			}
			CodingRuleModel codingRuleModel = dataModelList.FirstOrDefault((CodingRuleModel f) => f.MIsDefault);
			if (codingRuleModel == null)
			{
				codingRuleModel = dataModelList[0];
			}
			return ModelInfoManager.GetDataEditModel<CodingRuleModel>(ctx, codingRuleModel.MID, false, true);
		}
	}
}
