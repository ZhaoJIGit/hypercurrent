using System;
using System.IO;

namespace JieNor.Megi.Common.Utility
{
	public class FileUtility
	{
		public static string FileNewName => DateTime.Now.ToString("yyyyMMddHHmmss");

		public static void DeleteFile(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static void DeleteFolder(string path)
		{
			if (Directory.Exists(path))
			{
				string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
				foreach (string d in fileSystemEntries)
				{
					if (File.Exists(d))
					{
						File.Delete(d);
					}
					else
					{
						DeleteFolder(d);
					}
				}
			}
		}

		public static string GetFileExtendName(string file)
		{
			int lastIndex = file.LastIndexOf('.');
			if (lastIndex != -1)
			{
				return file.Substring(lastIndex + 1);
			}
			return string.Empty;
		}

		public static string GetPhysicalPath(string path)
		{
			string basePath = AppDomain.CurrentDomain.BaseDirectory;
			if (path.IsEmpty() || path.Length == 1)
			{
				return basePath;
			}
			if (path.IndexOf('~') == 0)
			{
				path = path.Substring(1);
			}
			path = path.Replace('/', '\\');
			if (path.IndexOf('\\') == 0)
			{
				return basePath.Substring(0, basePath.Length - 1) + path;
			}
			return basePath.Substring(0, basePath.Length) + path;
		}
	}
}
