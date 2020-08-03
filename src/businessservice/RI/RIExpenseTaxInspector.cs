using JieNor.Megi.BusinessContract.RI;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.RI
{
	public class RIExpenseTaxInspector : IRIInspectable
	{
		public RIExpenseTaxInspector()
		{
			base.enginers = new List<Func<MContext, RICategoryModel, int, int, RIInspectionResult>>
			{
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ET_TaxPayable_TaxRemain),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ET_TaxPayable_Transfered),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ET_SalaryPayable),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ET_Expense)
			};
		}

		public RIInspectionResult ET_TaxPayable_TaxRemain(MContext ctx, RICategoryModel category, int year, int period)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			RIInspectionResult rIInspectionResult2 = rIInspectionResult;
			string[] obj = new string[2];
			int num = year * 100 + period;
			obj[0] = num.ToString();
			obj[1] = "";
			rIInspectionResult2.MUrlParam = obj;
			List<BDAccountModel> accountIncludeDisable = dataPool.AccountIncludeDisable;
			Dictionary<string, string> riItemMapAccountNumberDic = GetRiItemMapAccountNumberDic();
			if (!riItemMapAccountNumberDic.Keys.Contains(category.MItemID))
			{
				rIInspectionResult.MPassed = true;
				return rIInspectionResult;
			}
			string accountNumber = riItemMapAccountNumberDic[category.MItemID];
			BDAccountModel account = (from x in accountIncludeDisable
			where x.MNumber == accountNumber
			select x).FirstOrDefault();
			if (account == null)
			{
				rIInspectionResult.MPassed = true;
				return rIInspectionResult;
			}
			if (!account.MIsActive)
			{
				rIInspectionResult.MPassed = true;
				rIInspectionResult.MAccountIsDisable = true;
				return rIInspectionResult;
			}
			List<GLBalanceModel> list = (from x in dataPool.BalanceList
			where x.MAccountID == account.MItemID && x.MCheckGroupValueID == "0"
			select x).ToList();
			if (list == null || list.Count() == 0)
			{
				rIInspectionResult.MMessageParam = new string[1]
				{
					account.MFullName
				};
				RIInspectionResult rIInspectionResult3 = rIInspectionResult;
				string[] obj2 = new string[2];
				num = year * 100 + period;
				obj2[0] = num.ToString();
				obj2[1] = account.MItemID;
				rIInspectionResult3.MUrlParam = obj2;
				rIInspectionResult.MPassed = true;
				rIInspectionResult.MNoLinkUrlIfPassed = false;
				return rIInspectionResult;
			}
			decimal d = list.Sum((GLBalanceModel x) => x.MBeginBalance) - list.Sum((GLBalanceModel x) => x.MDebit);
			if (d > decimal.Zero)
			{
				rIInspectionResult.MPassed = false;
				rIInspectionResult.MMessageParam = new string[1]
				{
					account.MFullName
				};
				RIInspectionResult rIInspectionResult4 = rIInspectionResult;
				string[] obj3 = new string[2];
				num = year * 100 + period;
				obj3[0] = num.ToString();
				obj3[1] = account.MItemID;
				rIInspectionResult4.MUrlParam = obj3;
			}
			else
			{
				rIInspectionResult.MMessageParam = new string[1]
				{
					account.MFullName
				};
				RIInspectionResult rIInspectionResult5 = rIInspectionResult;
				string[] obj4 = new string[2];
				num = year * 100 + period;
				obj4[0] = num.ToString();
				obj4[1] = account.MItemID;
				rIInspectionResult5.MUrlParam = obj4;
				rIInspectionResult.MPassed = true;
				rIInspectionResult.MNoLinkUrlIfPassed = false;
			}
			return rIInspectionResult;
		}

		private Dictionary<string, string> GetRiItemMapAccountNumberDic()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("5130", "2221.02");
			dictionary.Add("5140", "2221.03");
			dictionary.Add("5150", "2221.04");
			dictionary.Add("5160", "2221.05");
			dictionary.Add("5170", "2221.06");
			dictionary.Add("5180", "2221.07");
			dictionary.Add("5190", "2221.08");
			dictionary.Add("5191", "2221.09");
			dictionary.Add("5192", "2221.10");
			dictionary.Add("5193", "2221.11");
			dictionary.Add("5194", "2221.12");
			dictionary.Add("5195", "2221.13");
			dictionary.Add("5196", "2221.14");
			dictionary.Add("5197", "2221.15");
			dictionary.Add("5198", "2221.16");
			return dictionary;
		}

		public RIInspectionResult ET_TaxPayable_Transfered(MContext ctx, RICategoryModel category, int year, int period)
		{
			return new RIInspectionResult
			{
				MPassed = true
			};
		}

		public RIInspectionResult ET_SalaryPayable(MContext ctx, RICategoryModel category, int year, int period)
		{
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = false;
			rIInspectionResult.MMessageParam = new string[1]
			{
				"6"
			};
			rIInspectionResult.children = new List<RIInspectionResult>
			{
				new RIInspectionResult
				{
					MLinkUrl = category.MLinkUrl,
					MMessageParam = new string[2]
					{
						"陈攀",
						"-1000.00"
					},
					MUrlParam = new string[1]
					{
						"xxxxxxx"
					}
				},
				new RIInspectionResult
				{
					MLinkUrl = category.MLinkUrl,
					MMessageParam = new string[2]
					{
						"李丹",
						"-2000.00"
					},
					MUrlParam = new string[1]
					{
						"xxxxxxx"
					}
				},
				new RIInspectionResult
				{
					MLinkUrl = category.MLinkUrl,
					MMessageParam = new string[2]
					{
						"蓝宝兴",
						"-3000.00"
					},
					MUrlParam = new string[1]
					{
						"xxxxxxx"
					}
				},
				new RIInspectionResult
				{
					MLinkUrl = category.MLinkUrl,
					MMessageParam = new string[2]
					{
						"段仪",
						"-4000.00"
					},
					MUrlParam = new string[1]
					{
						"xxxxxxx"
					}
				},
				new RIInspectionResult
				{
					MLinkUrl = category.MLinkUrl,
					MMessageParam = new string[2]
					{
						"罗高勇",
						"-5000.00"
					},
					MUrlParam = new string[1]
					{
						"xxxxxxx"
					}
				},
				new RIInspectionResult
				{
					MLinkUrl = category.MLinkUrl,
					MMessageParam = new string[2]
					{
						"林福全",
						"-6000.00"
					},
					MUrlParam = new string[1]
					{
						"xxxxxxx"
					}
				}
			};
			return rIInspectionResult;
		}

		public RIInspectionResult ET_Expense(MContext ctx, RICategoryModel category, int year, int period)
		{
			return new RIInspectionResult
			{
				MPassed = true
			};
		}
	}
}
