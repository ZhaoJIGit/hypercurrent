using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Logger;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.Core.FileUtility
{
	public class FileHelper
	{
		private const int DEL_DAYS_BEFORE_FOR_IMPORT = 36000;

		public static string RandomNumber
		{
			get
			{
				return DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100000, 1000000);
			}
		}

		public static string GetUploadPath(string pathType)
		{
			return string.Join("\\", HttpContext.Current.Server.MapPath(pathType), ContextHelper.MContext.MUserID);
		}

		public static string UploadFile(HttpPostedFile file, string path)
		{
			string fileName = file.FileName;
			try
			{
				CreateEmptyDirectory(ref path, 36000, false);
				RenameFile(ref fileName);
				string filename = string.Join("\\", path, fileName);
				file.SaveAs(filename);
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
			return fileName;
		}

		private static void RenameFile(ref string fileNameWithExt)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithExt);
			string extension = Path.GetExtension(fileNameWithExt);
			fileNameWithExt = string.Format("{0}{1}{2}", fileNameWithoutExtension, RandomNumber, extension);
		}

		public static string GetOriginalFileName(string fileNameWithExt)
		{
			int length = RandomNumber.Length;
			fileNameWithExt = Path.GetFileName(fileNameWithExt);
			string text = Path.GetFileNameWithoutExtension(fileNameWithExt);
			string extension = Path.GetExtension(fileNameWithExt);
			if (text.Length > length)
			{
				text = text.Substring(0, text.Length - length);
			}
			return string.Format("{0}{1}", text, extension);
		}

		public static void UploadFileStream(Stream stream, ref string path, string fileName, bool createDir = false)
		{
			try
			{
				if (createDir)
				{
					CreateEmptyDirectory(ref path, 0, true);
				}
				using (FileStream destination = File.Create(string.Join("\\", path, fileName)))
				{
					stream.Seek(0L, SeekOrigin.Begin);
					stream.CopyTo(destination);
				}
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
		}

		public static Stream GetFileStream(string fullPath)
		{
			return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
		}

		public static void CreateRandomDirectory(ref string path, RandomDirectoryType type)
		{
			CreateEmptyDirectory(ref path, (int)type, true);
		}

		public static void CreateDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				try
				{
					Directory.CreateDirectory(path);
				}
				catch (Exception ex)
				{
					MLogger.Log(ex);
				}
			}
		}

		private static void CreateEmptyDirectory(ref string path, int delDaysBefore = 0, bool createRandomDir = false)
		{
			DeleteDirectoryFiles(path, delDaysBefore, false, createRandomDir);
			if (createRandomDir)
			{
				path = path + "\\" + RandomNumber;
			}
			CreateDirectory(path);
		}

		private static FileInfo[] GetSubFiles(DirectoryInfo dir)
		{
			FileInfo[] result = new FileInfo[0];
			try
			{
				result = dir.GetFiles();
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
			return result;
		}

		private static DirectoryInfo[] GetSubDirs(DirectoryInfo dir)
		{
			DirectoryInfo[] result = new DirectoryInfo[0];
			try
			{
				result = dir.GetDirectories();
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
			return result;
		}

		private static void DeleteDirectoryFiles(string path, int delDaysBefore = 0, bool isSubDir = false, bool createRandomDir = false)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (directoryInfo != null && directoryInfo.Exists)
			{
				DateTime t = DateTime.Now.AddDays((double)(-Math.Abs(delDaysBefore)));
				FileInfo[] subFiles = GetSubFiles(directoryInfo);
				DirectoryInfo[] subDirs = GetSubDirs(directoryInfo);
				if (!createRandomDir && subFiles.Any())
				{
					FileInfo[] array = subFiles;
					foreach (FileInfo fileInfo in array)
					{
						try
						{
							if (delDaysBefore == 0 || fileInfo.CreationTime <= t)
							{
								fileInfo.IsReadOnly = false;
								File.Delete(fileInfo.FullName);
							}
						}
						catch (Exception ex)
						{
							MLogger.Log(ex);
						}
					}
				}
				if (subDirs.Any())
				{
					DirectoryInfo[] array2 = subDirs;
					foreach (DirectoryInfo directoryInfo2 in array2)
					{
						DeleteDirectoryFiles(directoryInfo2.FullName, delDaysBefore, true, false);
					}
				}
				if (isSubDir && !GetSubDirs(directoryInfo).Any() && !GetSubFiles(directoryInfo).Any() && directoryInfo.CreationTime <= t)
				{
					try
					{
						directoryInfo.Delete();
					}
					catch (Exception ex2)
					{
						MLogger.Log(ex2);
					}
				}
			}
		}

		public static void ValidateFile(HttpPostedFile file, FileType fileType, FileValidateType validateType)
		{
			switch (validateType)
			{
			case FileValidateType.All:
				ValidateFileType(file, fileType);
				ValidateFileSize(file);
				break;
			case FileValidateType.FileType:
				ValidateFileType(file, fileType);
				break;
			case FileValidateType.FileSize:
				ValidateFileSize(file);
				break;
			}
		}

		public static string FormatFileSize(int byt)
		{
			double value;
			if (byt >= 1048576)
			{
				value = (double)(byt / 1048576);
				return Math.Round(value, 2) + " MB";
			}
			value = (double)(byt / 1024);
			return Math.Round(value, 2) + " KB";
		}

		public static string[] GetRealFileExtension(Stream stream, string ext = null)
		{
			string code = string.Empty;
			BinaryReader binaryReader = new BinaryReader(stream);
			try
			{
				binaryReader.BaseStream.Position = 0L;
				if (binaryReader.BaseStream.Length == 0L && !string.IsNullOrWhiteSpace(ext))
				{
					string a = ext.ToUpper();
					if (!(a == "DOC") && !(a == "DOCX"))
					{
						goto end_IL_000e;
					}
					return new string[2]
					{
						"DOC",
						"DOCX"
					};
				}
				byte b = binaryReader.ReadByte();
				code = b.ToString();
				b = binaryReader.ReadByte();
				code += b.ToString();
				end_IL_000e:;
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
				throw new FileException(FileExceptionType.FormatError);
			}
			return GetFileTypeByCode(code);
		}

		private static void ValidateFileType(HttpPostedFile file, FileType fileType)
		{
			string supportExtension = GetSupportExtension(fileType);
			Regex regex = new Regex(supportExtension);
			bool flag = false;
			string text = Path.GetExtension(file.FileName).TrimStart('.').ToUpper();
			if (regex.IsMatch(text))
			{
				if (text == "TXT" || text == "CSV")
				{
					flag = true;
				}
				else
				{
					string[] realFileExtension = GetRealFileExtension(file.InputStream, text);
					string[] array = realFileExtension;
					foreach (string input in array)
					{
						if (regex.IsMatch(input))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				return;
			}
			if (regex.IsMatch(text) && (fileType == FileType.ExcelExcludeCSV || fileType == FileType.ExcelIncludeCSV))
			{
				throw new FileException(FileExceptionType.ExcelFormatError);
			}
			throw new FileException(FileExceptionType.FormatUnSupport);
		}

		private static void ValidateFileSize(HttpPostedFile file)
		{
			int maxUploadSize = FtpHelper.MaxUploadSize;
			if (file.ContentLength <= maxUploadSize)
			{
				return;
			}
			throw new FileException(FileExceptionType.SizeExceedLimit);
		}

		public static string GetSupportExtension(FileType patternType)
		{
			string result = string.Empty;
			switch (patternType)
			{
			case FileType.Default:
				result = "TXT|CSV|DOC|DOCX|XLS|XLSX|PPT|PPTX|PDF|GIF|JPG|JPEG|PNG|BMP|RAR|ZIP|7Z";
				break;
			case FileType.Img:
				result = "GIF|JPG|JPEG|PNG|BMP";
				break;
			case FileType.ExcelIncludeCSV:
				result = "XLS|XLSX|CSV";
				break;
			case FileType.ExcelExcludeCSV:
				result = "XLS|XLSX";
				break;
			}
			return result;
		}

		private static string[] GetFileTypeByCode(string code)
		{
			string text = string.Empty;
			switch (code)
			{
			case "208207":
				text = "XLS,DOC,PPT";
				break;
			case "213203":
				text = "CSV";
				break;
			case "8075":
				text = "DOCX,XLSX,PPTX,ZIP";
				break;
			case "8297":
				text = "RAR";
				break;
			case "55122":
				text = "7Z";
				break;
			case "3780":
				text = "PDF";
				break;
			case "13780":
				text = "PNG";
				break;
			case "255216":
				text = "JPG,JPEG";
				break;
			case "7173":
				text = "GIF";
				break;
			case "6677":
				text = "BMP";
				break;
			}
			return text.Split(',');
		}
	}
}
