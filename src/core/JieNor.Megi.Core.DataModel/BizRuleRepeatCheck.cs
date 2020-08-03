using Fasterflect;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleRepeatCheck : BizRuleBase
	{
		public string Properties
		{
			get;
			set;
		}

		public bool IsBillHead
		{
			get;
			set;
		}

		[DataMember]
		public override string RuleName
		{
			get
			{
				return "唯一性校验";
			}
		}

		public BizRuleRepeatCheck(string propertyName, string properties, string properDesc, bool billHead)
			: base(propertyName, properDesc)
		{
			IsBillHead = billHead;
			Properties = properties;
			IsBillHead = billHead;
		}

		public override void Verification(MContext ctx)
		{
			if (base.BizData != null && !string.IsNullOrWhiteSpace(base.PropertyName) && !string.IsNullOrWhiteSpace(Properties))
			{
				if (IsBillHead)
				{
					CheckBillHeadFieldRepeat();
				}
				else
				{
					CheckBillEntryFieldRepeat();
				}
			}
		}

		private void CheckBillEntryFieldRepeat()
		{
			List<string> billEntryProperty = GetBillEntryProperty();
			if (billEntryProperty.Count != 0)
			{
				IEnumerable enumerable = base.BizData.GetPropertyValue(base.PropertyName) as IEnumerable;
				if (enumerable != null)
				{
					Dictionary<int, string> dictionary = new Dictionary<int, string>();
					int num = 0;
					foreach (BaseModel item2 in enumerable)
					{
						if (item2 == null)
						{
							break;
						}
						num++;
						string fldValues = GetFldValues(billEntryProperty);
						dictionary.Add(num, fldValues);
					}
					if (dictionary.Count >= 2)
					{
						List<string> list = new List<string>();
						Dictionary<int, string> dictionary2 = new Dictionary<int, string>(dictionary);
						foreach (KeyValuePair<int, string> item3 in dictionary)
						{
							IEnumerable<KeyValuePair<int, string>> enumerable2 = from f in dictionary2
							where f.Value.EqualsIgnoreCase(item3.Value) && f.Key != item3.Key
							select f;
							if (enumerable2.Count() > 0)
							{
								List<int> values = enumerable2.Select(delegate(KeyValuePair<int, string> row)
								{
									KeyValuePair<int, string> keyValuePair = row;
									return keyValuePair.Key;
								}).ToList();
								string message = string.Format("{0}唯一性校验：第{1}行数据与第{2}重复，请检查。", base.ProperDesc, string.Join(",", values), item3.Key);
								base.SetMessageInfor(message, AlertEnum.Error);
							}
							foreach (KeyValuePair<int, string> item4 in enumerable2)
							{
								dictionary2.Remove(item4.Key);
							}
						}
					}
				}
			}
		}

		private void CheckBillHeadFieldRepeat()
		{
			List<string> billHeadProperty = GetBillHeadProperty();
			if (billHeadProperty.Count != 0)
			{
				string fldValues = GetFldValues(billHeadProperty);
				string text = string.Join(",", billHeadProperty);
				if (billHeadProperty.Count != 1 || !string.IsNullOrWhiteSpace(fldValues))
				{
					string text2 = "";
					if (billHeadProperty.Count == 1)
					{
						text2 = string.Format("    Select {2},{0} As MValue \r\n                                                From {1} \r\n                                                Where {2} <> @PKID And FOrgID=@OrgID And {0} = @MValue ", text, base.BizData.TableName, base.BizData.PKFieldName);
					}
					else
					{
						text = "'@^^@'," + text;
						text2 = string.Format("  Select {2},MValue \r\n                                            From ( select {2},concat_ws({0}) as MValue \r\n                                                   from {1} Where {2} <> @PKID And FOrgID=@OrgID \r\n                                                 ) B \r\n                                            Where MValue =@MValue ", text, base.BizData.TableName, base.BizData.PKFieldName);
					}
					MySqlParameter[] cmdParms = new MySqlParameter[0];
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(base.MContext);
					DataSet dataSet = dynamicDbHelperMySQL.Query(text2, cmdParms);
					if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
					{
						string message = string.Format("唯一性校验：数据库中已经存在{0}为 的数据，请重新输入", base.ProperDesc);
						base.SetMessageInfor(message, AlertEnum.Error);
					}
				}
			}
		}

		private string GetFldValues(List<string> properties)
		{
			List<string> list = new List<string>();
			foreach (string property in properties)
			{
				object propertyValue = base.BizData.GetPropertyValue(property);
				if (propertyValue == null)
				{
					list.Add("");
				}
				else
				{
					list.Add(propertyValue.ToString());
				}
			}
			return string.Join("@^^@", list);
		}

		private List<string> GetBillHeadProperty()
		{
			List<string> list = new List<string>();
			List<PropertyInfo> source = base.BizData.GetType().GetProperties().ToList();
			List<string> list2 = Properties.Split(',').ToList();
			foreach (string item in list2)
			{
				if (source.Any((PropertyInfo f) => f.Name.EqualsIgnoreCase(item)))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private List<string> GetBillEntryProperty()
		{
			List<string> list = new List<string>();
			PropertyInfo[] source = (from f in base.BizData.GetType().GetProperties()
			where f.PropertyType.IsGenericType && ModelInfoManager.HasEntryAttribute(f) && f.PropertyType.GetGenericArguments()[0].GetTypeInfo().IsSubclassOf(typeof(BaseModel))
			select f).ToArray();
			List<string> list2 = Properties.Split(',').ToList();
			foreach (string item in list2)
			{
				if (source.Any((PropertyInfo f) => f.Name.EqualsIgnoreCase(item)))
				{
					list.Add(item);
				}
			}
			return list;
		}
	}
}
