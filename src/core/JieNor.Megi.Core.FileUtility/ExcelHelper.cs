using JieNor.Megi.Common.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace JieNor.Megi.Core.FileUtility
{
	public class ExcelHelper
	{
		public static string[] GetExcelHeader(int count = 50)
		{
			string[] array = new string[count];
			int num = (count % 26 == 0) ? (count / 26) : (count / 26 + 1);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				string arg = string.Empty;
				if (i > 0)
				{
					arg = ((char)(ushort)(65 + i - 1)).ToString();
				}
				for (int j = 65; j <= 90; j++)
				{
					if (num2 + 1 > count)
					{
						break;
					}
					string text = array[num2] = string.Format("{0}{1}", arg, (char)j);
					num2++;
				}
			}
			return array;
		}

		public static DataTable ReadCSV(Stream fs)
		{
			DataTable dataTable = new DataTable();
			dataTable.TableName = "default";
			StreamReader streamReader = null;
			try
			{
				streamReader = new StreamReader(fs, Encoding.Default);
				string text = "";
				List<string[]> list = new List<string[]>();
				while ((text = streamReader.ReadLine()) != null)
				{
					list.Add(text.TrimEnd(',').Split(','));
				}
				int num = (from f in list
				select f.Count()).Max();
				string[] excelHeader = GetExcelHeader(num);
				for (int i = 0; i < num; i++)
				{
					DataColumn column = new DataColumn(excelHeader[i]);
					dataTable.Columns.Add(column);
				}
				foreach (string[] item in list)
				{
					DataRow row = CreateDataRow(dataTable, item);
					dataTable.Rows.Add(row);
				}
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
				throw new FileException(FileExceptionType.FileReadError);
			}
			finally
			{
				streamReader.Close();
				fs.Close();
			}
			return dataTable;
		}

		public static bool IsExceedMaxEmptyRow(List<int> emptyRowList, int maxEmptyRowCount = 10)
		{
			int count = emptyRowList.Count;
			return count > maxEmptyRowCount && emptyRowList[count - 1] - emptyRowList[count - maxEmptyRowCount - 1] == maxEmptyRowCount;
		}

		public static void RemoveEmptyRow(DataTable result, List<int> emptyRowList)
		{
			if (emptyRowList.Any())
			{
				emptyRowList = (from v in emptyRowList
				orderby v descending
				select v).ToList();
				int count = result.Rows.Count;
				bool flag = true;
				int num = count - 1;
				while (num >= 0 && flag)
				{
					if (emptyRowList.Contains(num))
					{
						result.Rows.RemoveAt(num);
					}
					else
					{
						flag = false;
					}
					num--;
				}
			}
		}

		private static DataRow CreateDataRow(DataTable dt, string[] arrLine)
		{
			DataRow dataRow = dt.NewRow();
			int num = 0;
			for (int i = 0; i < arrLine.Length; i++)
			{
				int num2 = arrLine[i].Length - arrLine[i].Replace("\"", "").Length;
				if (arrLine[i].IndexOf("\"") != -1 && num2 == 1)
				{
					dataRow[num] = arrLine[i];
					for (int j = i + 1; j < arrLine.Length; j++)
					{
						DataRow dataRow2 = dataRow;
						int columnIndex = num;
						dataRow2[columnIndex] = dataRow2[columnIndex] + "," + arrLine[j];
						i++;
						if (arrLine[j].IndexOf("\"") != -1)
						{
							break;
						}
					}
				}
				else
				{
					dataRow[num] = arrLine[i];
				}
				dataRow[num] = Convert.ToString(dataRow[num]).Replace("\"", "").Trim();
				num++;
			}
			return dataRow;
		}
	}
}
