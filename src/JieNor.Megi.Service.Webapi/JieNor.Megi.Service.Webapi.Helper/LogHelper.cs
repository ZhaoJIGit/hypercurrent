using System;
using System.IO;
using System.Text;

namespace JieNor.Megi.Service.Webapi.Helper
{
	public class LogHelper
	{
		public static void WriteLog(string controllerName, string actionName, string message)
		{
			StreamWriter streamWriter = null;
			try
			{
				string text = AppDomain.CurrentDomain.BaseDirectory + "System\\Log\\";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				DateTime now = DateTime.Now;
				string arg = now.ToString("yyyy-MM-dd");
				string path = $"{text}{arg}.txt";
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = stringBuilder;
				now = DateTime.Now;
				stringBuilder2.Append("Time:    " + now.ToString() + "\r\n");
				stringBuilder.Append("Controller:" + controllerName + "\r\n");
				stringBuilder.Append("Action:  " + actionName + " \r\n");
				stringBuilder.Append("Message: " + message + "\r\n");
				stringBuilder.Append("Message: " + message + "\r\n");
				stringBuilder.Append("-----------------------------------------------------------\r\n\r\n");
				streamWriter = (File.Exists(path) ? File.AppendText(path) : File.CreateText(path));
				streamWriter.WriteLine(stringBuilder.ToString());
			}
			catch (Exception)
			{
			}
			finally
			{
				streamWriter?.Close();
			}
		}
	}
}
