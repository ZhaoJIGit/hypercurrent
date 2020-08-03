using JieNor.Megi.Common.Mongo.Utility;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace JieNor.Megi.Core.Mongo
{
	public class MongoBDHelper
	{
		public static string DataBase = "basedata";

		private static List<KeyValuePair<string, string>> BDTableList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("t_bd_account", "t_bd_account"),
			new KeyValuePair<string, string>("t_bd_account_l", "t_bd_account"),
			new KeyValuePair<string, string>("t_bd_bankaccount", "t_bd_bankaccount"),
			new KeyValuePair<string, string>("t_bd_bankaccount_l", "t_bd_bankaccount"),
			new KeyValuePair<string, string>("t_bd_banktype", "t_bd_banktype"),
			new KeyValuePair<string, string>("t_bd_banktype_l", "t_bd_banktype"),
			new KeyValuePair<string, string>("t_bd_contactstype", "t_bd_contactstype"),
			new KeyValuePair<string, string>("t_bd_contactstype_l", "t_bd_contactstype"),
			new KeyValuePair<string, string>("t_bd_contacts", "t_bd_contacts"),
			new KeyValuePair<string, string>("t_bd_contacts_l", "t_bd_contacts"),
			new KeyValuePair<string, string>("t_bd_contactstypelink", "t_bd_contactstypelink"),
			new KeyValuePair<string, string>("t_bd_employees", "t_bd_employees"),
			new KeyValuePair<string, string>("t_bd_employees_l", "t_bd_employees"),
			new KeyValuePair<string, string>("t_bd_item", "t_bd_item"),
			new KeyValuePair<string, string>("t_bd_item_l", "t_bd_item"),
			new KeyValuePair<string, string>("t_bd_expenseitem", "t_bd_expenseitem"),
			new KeyValuePair<string, string>("t_bd_expenseitem_l", "t_bd_expenseitem"),
			new KeyValuePair<string, string>("t_reg_currency", "t_reg_currency"),
			new KeyValuePair<string, string>("t_reg_taxrate", "t_reg_taxrate"),
			new KeyValuePair<string, string>("t_reg_taxrate_l", "t_reg_taxrate"),
			new KeyValuePair<string, string>("t_bd_track", "t_bd_track"),
			new KeyValuePair<string, string>("t_bd_track_l", "t_bd_track"),
			new KeyValuePair<string, string>("t_bd_trackentry", "t_bd_track"),
			new KeyValuePair<string, string>("t_bd_trackentry_l", "t_bd_track"),
			new KeyValuePair<string, string>("t_bd_contactstracklink", "t_bd_contactstracklink"),
			new KeyValuePair<string, string>("t_pa_payitemgroup", "t_pa_payitemgroup"),
			new KeyValuePair<string, string>("t_pa_payitemgroup_l", "t_pa_payitemgroup"),
			new KeyValuePair<string, string>("t_pa_payitem", "t_pa_payitem"),
			new KeyValuePair<string, string>("t_pa_payitem_l", "t_pa_payitem"),
			new KeyValuePair<string, string>("t_bas_currency", "t_bas_currency"),
			new KeyValuePair<string, string>("t_bas_currency_l", "t_bas_currency"),
			new KeyValuePair<string, string>("t_gl_checkgroup", "t_gl_checkgroup"),
			new KeyValuePair<string, string>("t_gl_checkgroupvalue", "t_gl_checkgroupvalue")
		};

		private static string _connectionString
		{
			get;
			set;
		}

		public static string ConnectionString
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_connectionString))
				{
					_connectionString = ConfigurationManager.ConnectionStrings["MegiBDCacheConnection"].ConnectionString;
				}
				return _connectionString;
			}
		}

		public static bool Insert<T>(MContext ctx, T data) where T : BaseModel
		{
			string collectionName = GetCollectionName<T>(ctx, null);
			return new MongoDbHelper(ConnectionString, DataBase).Insert(data, collectionName);
		}

		public static bool Insert<T>(MContext ctx, List<T> data) where T : BaseModel
		{
			string collectionName = GetCollectionName(ctx, (data == null || data.Count == 0) ? null : data[0]);
			RemoveAll<T>(ctx);
			return new MongoDbHelper(ConnectionString, DataBase).Insert(data, collectionName);
		}

		public static T FindOne<T>(MContext ctx) where T : BaseModel
		{
			string collectionName = GetCollectionName<T>(ctx, null);
			return new MongoDbHelper(ConnectionString, DataBase).FindOne<T>(collectionName);
		}

		public static List<T> FindAll<T>(MContext ctx) where T : BaseModel
		{
			string collectionName = GetCollectionName<T>(ctx, null);
			return new MongoDbHelper(ConnectionString, DataBase).FindAll<T>(collectionName);
		}

		public static bool RemoveAll<T>(MContext ctx) where T : BaseModel
		{
			string collectionName = GetCollectionName<T>(ctx, null);
			return new MongoDbHelper(ConnectionString, DataBase).RemoveAll<T>(collectionName);
		}

		private static List<string> GetCollectionNames(MContext ctx, string tableName)
		{
			List<string> list = new List<string>
			{
				tableName.ToLower() + "_" + ctx.MOrgID
			};
			for (int i = 0; i < ctx.MActiveLocaleIDS.Count; i++)
			{
				list.Add(tableName.ToLower() + "_" + ctx.MOrgID + "_" + ctx.MActiveLocaleIDS[i]);
			}
			return list;
		}

		private static string GetCollectionName<T>(MContext ctx, T model = null) where T : BaseModel
		{
			string empty = string.Empty;
			empty = ((model == null) ? (Activator.CreateInstance(typeof(T)) as BaseModel).TableName : model.TableName);
			return empty.ToLower() + "_" + ctx.MOrgID + (ctx.MultiLang ? "" : ("_" + ctx.MLCID));
		}

		public static void Monitoring(MContext ctx, List<string> sqlList)
		{
			List<string> tableNames = GetTableNames(sqlList);
			MongoDbHelper BDHelper = new MongoDbHelper(ConnectionString, DataBase);
			for (int i = 0; i < tableNames.Count; i++)
			{
				List<string> collectionNames = GetCollectionNames(ctx, tableNames[i].ToLower());
				collectionNames.ForEach(delegate(string x)
				{
					if (BDHelper.CollectionExists(x))
					{
						BDHelper.Drop(x);
					}
				});
			}
		}

		private static List<string> GetTableNames(List<string> sqlList)
		{
			List<string> tableList = new List<string>();
			if (sqlList == null || sqlList.Count == 0)
			{
				return tableList;
			}
			Regex regex = new Regex("^insert\\s+into\\s+(\\w+)\\s*\\(|^insert\\s+into\\s+(\\w+)\\s+select|^insert\\s+into\\s+(\\w+)\\s+values|^update\\s+(\\w+)\\s+set|^delete\\s+from\\s+(\\w+)\\s+", RegexOptions.IgnoreCase);
			for (int j = 0; j < sqlList.Count; j++)
			{
				Match match = regex.Match(sqlList[j].Trim());
				if (match.Success)
				{
					GroupCollection groups = match.Groups;
					if (groups[1].Length > 0)
					{
						tableList.Add(groups[1].ToString().ToLower());
					}
					if (groups[2].Length > 0)
					{
						tableList.Add(groups[2].ToString().ToLower());
					}
					if (groups[3].Length > 0)
					{
						tableList.Add(groups[3].ToString().ToLower());
					}
					if (groups[4].Length > 0)
					{
						tableList.Add(groups[4].ToString().ToLower());
					}
					if (groups[5].Length > 0)
					{
						tableList.Add(groups[5].ToString().ToLower());
					}
				}
			}
			tableList = tableList.Distinct().ToList();
			List<string> list = new List<string>();
			int i;
			for (i = 0; i < tableList.Count; i++)
			{
				KeyValuePair<string, string> keyValuePair = BDTableList.FirstOrDefault((KeyValuePair<string, string> x) => x.Key == tableList[i]);
				if (!string.IsNullOrWhiteSpace(keyValuePair.Key) && !list.Contains(keyValuePair.Value))
				{
					list.Add(keyValuePair.Value);
				}
			}
			return list;
		}
	}
}
