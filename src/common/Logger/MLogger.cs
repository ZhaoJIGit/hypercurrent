using JieNor.Megi.EntityModel.Context;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace JieNor.Megi.Common.Logger
{
	public class MLogger
	{
		private static string logFilePath = ConfigurationManager.AppSettings["logFilePath"];

		private static string writeLog = ConfigurationManager.AppSettings["writeLog"];

		private static string logFileSizes = ConfigurationManager.AppSettings["logFileSize"];

		public static string LogFilePath
		{
			set
			{
				logFilePath = value;
			}
		}

		public static int LogFileSizes => string.IsNullOrEmpty(logFilePath) ? 1024 : int.Parse(logFileSizes);

		public static bool WriteLog => writeLog == "1";

		public static void Log(string text, MContext ctx = null)
		{
			try
			{
				if (WriteLog)
				{
					ctx = (ctx ?? new MContext());
					if (!Directory.Exists(logFilePath))
					{
						Directory.CreateDirectory(logFilePath);
					}
					string arg = logFilePath;
					DateTime now = DateTime.Now;
					string fileName = string.Format("{0}{1}.txt", arg, now.ToString("yyyy-MM-dd"));
					StringBuilder builder = new StringBuilder();
					StringBuilder stringBuilder = builder;
					now = DateTime.Now;
					stringBuilder.Append(now.ToString("yyyy-MM-dd hh:mm:ss") + ":" + text);
					File.AppendAllText(fileName, builder.ToString());
				}
			}
			catch (Exception)
			{
			}
		}

		public static void Log(string funcName, Exception ex, MContext ctx = null)
		{
			try
			{
				if (WriteLog)
				{
					ctx = (ctx ?? new MContext());
					if (!Directory.Exists(logFilePath))
					{
						Directory.CreateDirectory(logFilePath);
					}
					string arg = logFilePath;
					DateTime now = DateTime.Now;
					string fileName = string.Format("{0}{1}.txt", arg, now.ToString("yyyy-MM-dd"));
					StringBuilder builder = new StringBuilder();
					StringBuilder stringBuilder = builder;
					now = DateTime.Now;
					stringBuilder.Append(string.Format("-----------------------{0}--------------------------\r\n", now.ToString("yyyy-MM-dd hh:mm:ss")));
					builder.Append($"UserID:{ctx.MUserID}\r\n");
					builder.Append($"UserIP:{ctx.MUserIP}\r\n");
					builder.Append($"UserOrgID:{ctx.MOrgID}\r\n");
					builder.Append($"AccessToken:{ctx.MAccessToken}\r\n");
					builder.Append($"UserOrgID:{ctx.MAccessCode}\r\n");
					builder.Append($"AccessCode:{ctx.MOrgID}\r\n");
					builder.Append($"funcName:{funcName}\r\n");
					builder.Append(string.Format("Message:{0}\r\n", ex.Message + "\r\n" + ex.Source + "\r\n" + ex.StackTrace));
					builder.Append("\r\n\r\n\r\n");
					File.AppendAllText(fileName, builder.ToString());
				}
			}
			catch (Exception)
			{
			}
		}

		public static void Log(Exception ex)
		{
			try
			{
				if (WriteLog)
				{
					if (!Directory.Exists(logFilePath))
					{
						Directory.CreateDirectory(logFilePath);
					}
					string arg = logFilePath;
					DateTime now = DateTime.Now;
					string fileName = string.Format("{0}{1}.txt", arg, now.ToString("yyyy-MM-dd"));
					StringBuilder builder = new StringBuilder();
					StringBuilder stringBuilder = builder;
					now = DateTime.Now;
					stringBuilder.Append(string.Format("-----------------------{0}--------------------------\r\n", now.ToString("yyyy-MM-dd hh:mm:ss")));
					builder.Append($"Message:{ex.Message}\r\n{ex.Source}\r\n{ex.StackTrace}");
					builder.Append("\r\n\r\n\r\n");
					File.AppendAllText(fileName, builder.ToString());
				}
			}
			catch
			{
			}
		}

		public static string GetMethodInfo()
		{
			string str8 = "";
			str8 = str8 + "命名空间名:" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "\n";
			str8 = str8 + "类名:" + MethodBase.GetCurrentMethod().DeclaringType.FullName + "\n";
			str8 = str8 + "方法名:" + MethodBase.GetCurrentMethod().Name + "\n";
			str8 += "\n";
			StackTrace ss = new StackTrace(true);
			MethodBase mb = ss.GetFrame(1).GetMethod();
			str8 = str8 + mb.DeclaringType.Namespace + "\n";
			str8 = str8 + mb.DeclaringType.Name + "\n";
			str8 = str8 + mb.DeclaringType.FullName + "\n";
			return str8 + mb.Name + "\n";
		}
	}
}
