using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.Context;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.API
{
	public class APILangTableHelper
	{
		private class LangTableFilter
		{
			public string TableName
			{
				get;
				set;
			}

			public List<string> IncludeColumns
			{
				get;
				set;
			}

			public string Where
			{
				get;
				set;
			}

			public bool MultLang
			{
				get;
				set;
			}

			public bool FilterByActiveLocale
			{
				get;
				set;
			}
		}

		private static Hashtable tableSqlCache;

		private static COMTableInfoRepository tableInfoRep = new COMTableInfoRepository();

		private static List<string> excludeFields = new List<string>
		{
			"mpkid",
			"mparentid",
			"mlocaleid",
			"morgid",
			"misactive",
			"misdelete",
			"mcreatorid",
			"mcreatedate",
			"mmodifierid",
			"mmodifydate"
		};

		private static List<KeyValuePair<string, LangTableFilter>> tableData = new List<KeyValuePair<string, LangTableFilter>>
		{
			new KeyValuePair<string, LangTableFilter>("t_bd_account_l", new LangTableFilter
			{
				TableName = "t_bd_account_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_bankaccount_l", new LangTableFilter
			{
				TableName = "t_bd_bankaccount_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_banktype_l", new LangTableFilter
			{
				TableName = "t_bd_banktype_l",
				Where = " ",
				FilterByActiveLocale = true
			}),
			new KeyValuePair<string, LangTableFilter>("t_bas_currency_l", new LangTableFilter
			{
				TableName = "t_bas_currency_l",
				Where = " ",
				FilterByActiveLocale = true
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_contactstype_l", new LangTableFilter
			{
				TableName = "t_bd_contactstype_l",
				Where = " ",
				FilterByActiveLocale = true
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_contacts_l", new LangTableFilter
			{
				TableName = "t_bd_contacts_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_reg_taxrate_l", new LangTableFilter
			{
				TableName = "t_reg_taxrate_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_pa_payitem_l", new LangTableFilter
			{
				TableName = "t_pa_payitem_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_pa_payitemgroup_l", new LangTableFilter
			{
				TableName = "t_pa_payitemgroup_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_employees_l", new LangTableFilter
			{
				TableName = "t_bd_employees_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_track_l", new LangTableFilter
			{
				TableName = "t_bd_track_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_trackentry_l", new LangTableFilter
			{
				TableName = "t_bd_trackentry_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_expenseitem_l", new LangTableFilter
			{
				TableName = "t_bd_expenseitem_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_bd_item_l", new LangTableFilter
			{
				TableName = "t_bd_item_l",
				Where = " and MOrgID = @MOrgID "
			}),
			new KeyValuePair<string, LangTableFilter>("t_bas_country_l", new LangTableFilter
			{
				TableName = "t_bas_country_l",
				Where = " ",
				FilterByActiveLocale = true
			}),
			new KeyValuePair<string, LangTableFilter>("t_bas_province_l", new LangTableFilter
			{
				TableName = "t_bas_province_l",
				Where = " ",
				FilterByActiveLocale = true
			})
		};

		public static string GetLangSQL(MContext ctx, string key)
		{
			LangTableFilter value = tableData.FirstOrDefault((KeyValuePair<string, LangTableFilter> x) => x.Key == key).Value;
			value.MultLang = ctx.MultiLang;
			tableSqlCache = (tableSqlCache ?? Hashtable.Synchronized(new Hashtable()));
			key = (value.MultLang ? (key + "_" + string.Join("_", ctx.MActiveLocaleIDS.ToArray()) + "_mult_lang") : key);
			if (tableSqlCache[key] == null)
			{
				string multLangTableSQL = GetMultLangTableSQL(ctx, value);
				tableSqlCache[key] = multLangTableSQL;
			}
			return tableSqlCache[key] as string;
		}

		private static string GetMultLangTableSQL(MContext ctx, LangTableFilter filter)
		{
			List<MTableColumnModel> tableColumnModel = tableInfoRep.GetTableColumnModel(filter.TableName, null);
			bool flag = tableColumnModel.Exists((MTableColumnModel x) => x.Name.ToLower() == "morgid");
			string empty = string.Empty;
			string str = "( SELECT \n                            MParentID," + (flag ? "MOrgID," : "") + "MIsDelete,";
			empty += str;
			string[] array = ctx.MActiveLocaleIDS.ToArray();
			for (int i = 0; i < tableColumnModel.Count; i++)
			{
				if (!excludeFields.Contains(tableColumnModel[i].Name.ToLower()) && (filter.IncludeColumns == null || filter.IncludeColumns.Contains(tableColumnModel[i].Name.ToLower())))
				{
					string name = tableColumnModel[i].Name;
					bool flag2 = tableColumnModel[i].Type == "varbinary";
					string arg = "MLocaleID";
					string text = flag2 ? string.Format("(convert(AES_DECRYPT({0},'{1}') using utf8)) ", name, "JieNor-001") : (name ?? "");
					if (filter.MultLang)
					{
						string format = "CONCAT('[',\n                                    GROUP_CONCAT('{{\"LCID\"',\n                                        ':',\n                                        JSON_QUOTE({0}),\n                                        ',',\n                                        '\"Value\"',\n                                        ':',\n                                        JSON_QUOTE({1}),\n                                        '}}'),\n                                    ']') AS {2},";
						empty += string.Format(format, arg, text, name);
						string text2 = string.Empty;
						for (int j = 0; j < array.Length; j++)
						{
							string str2 = "TRIM(BOTH ',' FROM GROUP_CONCAT(CASE {0}\n                                    WHEN '" + array[j] + "' THEN {1}\n                                    ELSE ''\n                                END)) AS {2}_" + array[j] + ",";
							text2 += str2;
						}
						empty += string.Format(text2, arg, text, name);
					}
					else
					{
						string str3 = text + " " + (flag2 ? name : "") + ",";
						empty += str3;
					}
				}
			}
			string format2 = " FROM {0}\n                            WHERE\n                                MIsDelete = 0\n                                {1}" + ((!filter.MultLang) ? " and MLocaleID = @MLocaleID ) " : ("\n                            GROUP BY MParentID," + (flag ? "MOrgID," : "") + " MIsDelete )"));
			if (filter.FilterByActiveLocale)
			{
				string str4 = " and MLocaleID in ('" + string.Join("','", ctx.MActiveLocaleIDS) + "') ";
				filter.Where = (filter.Where ?? string.Empty);
				filter.Where += str4;
			}
			return empty.TrimEnd(',') + string.Format(format2, filter.TableName, filter.Where);
		}
	}
}
