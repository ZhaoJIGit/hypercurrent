using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace JieNor.Megi.Core.FileUtility
{
	public class FtpHelper
	{
		private static string ftpServerIP;

		private static bool usePassive;

		private static string ftpUserID;

		private static string ftpPassword;

		private static string ftpRootDir;

		public const string IMPORT_FOLDER_NAME = "ImportTemp";

		public static int MaxUploadSize
		{
			get;
			set;
		}

		static FtpHelper()
		{
			ftpServerIP = string.Format("ftp://{0}:{1}", ConfigurationManager.AppSettings["FtpServerIp"], ConfigurationManager.AppSettings["FtpPort"]);
			usePassive = true;
			ftpUserID = ConfigurationManager.AppSettings["FtpUserId"];
			ftpPassword = ConfigurationManager.AppSettings["FtpPassword"];
			ftpRootDir = ConfigurationManager.AppSettings["FtpRootDir"];
			MaxUploadSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxUploadSize"]) * 1024;
			string text = Convert.ToString(ConfigurationManager.AppSettings["UsePassive"]);
			usePassive = ((string.IsNullOrWhiteSpace(text) || text == "1") && true);
		}

		public static void UploadFile(string orgId, Stream uploadFileStream, string uploadFileName, int uploadFileSize)
		{
			object[] obj = new object[4]
			{
				orgId,
				null,
				null,
				null
			};
			DateTime now = DateTime.Now;
			obj[1] = now.Year;
			now = DateTime.Now;
			obj[2] = now.Month;
			obj[3] = uploadFileName;
			string path = AppendRootDir(string.Join("/", obj));
			FtpUpload(uploadFileStream, uploadFileSize, path);
		}

		public static Stream GetDownloadStream(string filePath, string fileName)
		{
			Uri requestUri = new Uri(string.Join("/", ftpServerIP, AppendRootDir(filePath.Trim('/')), fileName));
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUri);
			ftpWebRequest.Method = "RETR";
			ftpWebRequest.UseBinary = true;
			ftpWebRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
			return ftpWebResponse.GetResponseStream();
		}

		private static string AppendRootDir(string path)
		{
			if (!string.IsNullOrWhiteSpace(ftpRootDir))
			{
				path = string.Join("/", ftpRootDir, path);
			}
			return path;
		}

		private static void FtpUpload(Stream uploadFileStream, int uploadFileSize, string path)
		{
			FtpWebRequest ftpWebRequest = null;
			Stream stream = null;
			try
			{
				CheckPathExist(path);
				string uriString = string.Join("/", ftpServerIP, path);
				ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(uriString));
				ftpWebRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
				ftpWebRequest.KeepAlive = false;
				ftpWebRequest.Method = "STOR";
				ftpWebRequest.UsePassive = usePassive;
				ftpWebRequest.UseBinary = true;
				ftpWebRequest.ContentLength = uploadFileSize;
				int num = 2048;
				byte[] buffer = new byte[num];
				stream = ftpWebRequest.GetRequestStream();
				uploadFileStream.Position = 0L;
				for (int num2 = uploadFileStream.Read(buffer, 0, num); num2 != 0; num2 = uploadFileStream.Read(buffer, 0, num))
				{
					stream.Write(buffer, 0, num2);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			finally
			{
				if (uploadFileStream != null)
				{
					uploadFileStream.Close();
				}
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private static void DeleteFile(string path)
		{
			try
			{
				string uriString = string.Join("/", ftpServerIP, AppendRootDir(path));
				FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(uriString));
				ftpWebRequest.UseBinary = true;
				ftpWebRequest.UsePassive = false;
				ftpWebRequest.KeepAlive = false;
				ftpWebRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
				ftpWebRequest.Method = "DELE";
				FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
				ftpWebResponse.Close();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		private static void CheckPathExist(string filePath)
		{
			string text = filePath.Substring(0, filePath.LastIndexOf("/"));
			string[] array = text.Split('/');
			string text2 = "/";
			foreach (string text3 in array)
			{
				if (!string.IsNullOrWhiteSpace(text3))
				{
					text2 = text2 + text3 + "/";
					FtpMakeDir(text2);
				}
			}
		}

		private static bool FtpMakeDir(string localFile)
		{
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(ftpServerIP + localFile);
			ftpWebRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			ftpWebRequest.Method = "MKD";
			try
			{
				FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
				ftpWebResponse.Close();
			}
			catch (Exception)
			{
				ftpWebRequest.Abort();
				return false;
			}
			ftpWebRequest.Abort();
			return true;
		}
	}
}
