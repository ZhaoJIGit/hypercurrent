using JieNor.Megi.Common.Mongo.BusinessService;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.LOG;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Core.Logger
{
	public class MSqlLogger
	{
		private static string sqlLogFilePath = ConfigurationManager.AppSettings["sqlLogFilePath"];

		private static bool writeSqlLog = ConfigurationManager.AppSettings["writeSqlLog"] == "1";

		private static string writeSqlLogSeq = ConfigurationManager.AppSettings["writeSqlLogSeq"];

		private static bool writeSqlLog2Mongo = ConfigurationManager.AppSettings["writeSqlLog2Mongo"] == "1";

		private static string writeSqlLogType = ConfigurationManager.AppSettings["writeSqlLogType"];

		private static List<string> writeSqlLogTypeList
		{
			get
			{
				return string.IsNullOrWhiteSpace(writeSqlLogType) ? new List<string>() : writeSqlLogType.Split(';').ToList();
			}
		}

		private static void LOG(MContext ctx, List<MySqlCommand> cmds)
		{
			try
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					WriteSqlLog(ctx, cmds);
				});
				thread.Start();
			}
			catch (Exception)
			{
			}
		}

		public static void LOG(MContext ctx, MySqlCommand cmd)
		{
			try
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					WriteSqlLog(ctx, new List<MySqlCommand>
					{
						cmd
					});
				});
				thread.Start();
			}
			catch (Exception)
			{
			}
		}

		public static void LOG(MContext ctx, List<CommandInfo> cmds, MySqlConnection conn)
		{
			try
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					WriteSqlLog(ctx, ConvertComandInfos2Commands(conn, cmds));
				});
				thread.Start();
			}
			catch (Exception)
			{
			}
		}

		public static void LOG(MContext ctx, CommandInfo cmds, MySqlConnection conn)
		{
			try
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					WriteSqlLog(ctx, ConvertComandInfos2Commands(conn, new List<CommandInfo>
					{
						cmds
					}));
				});
				thread.Start();
			}
			catch (Exception)
			{
			}
		}

		private static void WriteSqlLog(MContext ctx, List<MySqlCommand> cmds)
		{
			try
			{
				if (writeSqlLog)
				{
					if (writeSqlLog2Mongo)
					{
						WriteLog2Mongo(ctx, cmds);
					}
					else
					{
						WriteLog2File(ctx, cmds);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private static void WriteLog2File(MContext ctx, List<MySqlCommand> cmds)
		{
			if (writeSqlLog)
			{
				if (!Directory.Exists(sqlLogFilePath))
				{
					Directory.CreateDirectory(sqlLogFilePath);
				}
				writeSqlLogSeq = (writeSqlLogSeq ?? "1");
				int num = int.Parse(writeSqlLogSeq);
				num = ((num > 60 || num < 0) ? 5 : num);
				int minute = DateTime.Now.Minute;
				string path = string.Format("{0}SqlLog_{1}.txt", sqlLogFilePath, minute / num);
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				List<MLog> list = ConvertCommands2MLogList(ctx, cmds);
				using (StreamWriter streamWriter = new StreamWriter(path, true))
				{
					foreach (MLog item in list)
					{
						streamWriter.WriteLine(javaScriptSerializer.Serialize(item));
					}
				}
			}
		}

		private static void WriteLog2Mongo(MContext ctx, List<MySqlCommand> cmds)
		{
			MongoMLogBusiness mongoMLogBusiness = new MongoMLogBusiness();
			List<MLog> logs = ConvertCommands2MLogList(ctx, cmds);
			mongoMLogBusiness.SaveMLog(logs);
		}

		private static List<MLog> ConvertCommands2MLogList(MContext ctx, List<MySqlCommand> cmds)
		{
			List<MLog> list = new List<MLog>();
			int num = 0;
			while (cmds != null && num < cmds.Count)
			{
				KeyValuePair<string, string> tableNameFromSql = GetTableNameFromSql(cmds[num].CommandText.ToLower());
				if (writeSqlLogTypeList.Contains(tableNameFromSql.Key))
				{
					list.Add(new MLog
					{
						Id = UUIDHelper.GetGuid(),
						MDate = DateTime.Now,
						MOrgID = ctx.MOrgID,
						MSql = cmds[num].CommandText,
						MUserID = ctx.MUserID,
						MOperateType = tableNameFromSql.Key,
						MParameter = ConvertMySqlParameters2String(cmds[num].Parameters),
						MDatabase = cmds[num].Connection.Database,
						MTable = tableNameFromSql.Value
					});
				}
				num++;
			}
			return list;
		}

		private static KeyValuePair<string, string> GetTableNameFromSql(string commandText)
		{
			string value = string.Empty;
			string text = (commandText.Length > 40) ? commandText.Substring(0, 40) : commandText;
			string text2 = text.TrimStart().Substring(0, 6).ToLower();
			try
			{
				if (text2 == "insert")
				{
					text = text.TrimStart().Replace("insert", "").Replace("into", "")
						.TrimStart();
					value = text.Substring(0, text.IndexOf('(')).TrimEnd();
				}
				else if (text2 == "update")
				{
					text = text.TrimStart().Replace("update", "").TrimStart();
					value = text.Substring(0, text.IndexOf(' ')).TrimEnd();
				}
				else if (text2 == "delete")
				{
					text = text.TrimStart().Replace(text2, "").Replace("from", "")
						.TrimStart();
					value = text.Substring(0, text.IndexOf(' ')).TrimEnd();
				}
			}
			catch (Exception)
			{
			}
			return new KeyValuePair<string, string>(text2, value);
		}

		private static string ConvertMySqlParameters2String(MySqlParameterCollection parameters)
		{
			string text = "{";
			int num = 0;
			while (parameters != null && num < parameters.Count)
			{
				text = text + "'" + parameters[num].ParameterName + "':'" + parameters[num].Value.ToString() + "',";
				num++;
			}
			return text.TrimEnd(',') + "}";
		}

		private static List<MySqlCommand> ConvertComandInfos2Commands(MySqlConnection conn, List<CommandInfo> cmds)
		{
			List<MySqlCommand> list = new List<MySqlCommand>();
			if (cmds == null || !cmds.Any())
			{
				return list;
			}
			for (int i = 0; i < cmds.Count; i++)
			{
				list.Add(ConvertComandInfo2Command(conn, cmds[i]));
			}
			return list;
		}

		private static MySqlCommand ConvertComandInfo2Command(MySqlConnection conn, CommandInfo cmdInfo)
		{
			MySqlCommand mySqlCommand = new MySqlCommand
			{
				CommandText = cmdInfo.CommandText,
				Connection = conn
			};
			try
			{
				if (cmdInfo.Parameters != null && cmdInfo.Parameters.Any())
				{
					for (int i = 0; i < cmdInfo.Parameters.Count(); i++)
					{
						MySqlParameter value = (MySqlParameter)cmdInfo.Parameters[i];
						if (!mySqlCommand.Parameters.Contains(value))
						{
							mySqlCommand.Parameters.Add(value);
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return mySqlCommand;
		}
	}
}
