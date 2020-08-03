using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace JieNor.Megi.DataRepository.COM
{
	public class ExcelDataHelper
	{
		public static DataTable ReadExcelData(MContext ctx, Stream fileStream, string fileName)
		{
			fileStream.Position = 0L;
			if (fileName.ToLower().EndsWith("csv"))
			{
				try
				{
					return ExcelHelper.ReadCSV(fileStream);
				}
				catch (FileException)
				{
					throw new Exception(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "FileReadError", "The file read error!"));
				}
			}
			IWorkbook workbook = WorkbookFactory.Create(fileStream);
			ISheet sheetAt = workbook.GetSheetAt(0);
			int lastRowNum = GetLastRowNum(sheetAt);
			int cellCount = GetCellCount(sheetAt, lastRowNum);
			DataTable dataTable = new DataTable();
			string[] excelHeader = ExcelHelper.GetExcelHeader(cellCount);
			List<int> list = new List<int>();
			for (int i = 0; i < cellCount; i++)
			{
				DataColumn column = new DataColumn(excelHeader[i]);
				dataTable.Columns.Add(column);
			}
			for (int j = 0; j <= lastRowNum; j++)
			{
				if (ExcelHelper.IsExceedMaxEmptyRow(list, 10))
				{
					break;
				}
				IRow row = sheetAt.GetRow(j);
				if (row == null)
				{
					if (j != lastRowNum)
					{
						list.Add(j);
						dataTable.Rows.Add(dataTable.NewRow());
					}
				}
				else
				{
					if (row.LastCellNum == -1)
					{
						list.Add(j);
					}
					DataRow dataRow = dataTable.NewRow();
					int num = 0;
					for (int k = 0; k < cellCount; k++)
					{
						ICell cell = row.GetCell(k);
						if (cell == null)
						{
							num++;
						}
						else
						{
							string cellValue = GetCellValue(cell);
							if (string.IsNullOrWhiteSpace(cellValue))
							{
								num++;
							}
							dataRow[k] = cellValue;
						}
					}
					dataTable.Rows.Add(dataRow);
					if (num == cellCount)
					{
						list.Add(j);
					}
				}
			}
			ExcelHelper.RemoveEmptyRow(dataTable, list);
			if (string.IsNullOrEmpty(dataTable.TableName))
			{
				dataTable.TableName = "default";
			}
			return dataTable;
		}

		private static int GetCellCount(ISheet sheet, int rowCount)
		{
			int result = 0;
			List<int> list = new List<int>();
			for (int i = 0; i < rowCount; i++)
			{
				IRow row = sheet.GetRow(i);
				if (row != null)
				{
					int item = (row.LastCellNum > 50) ? 50 : row.LastCellNum;
					list.Add(item);
				}
			}
			if (list.Any())
			{
				result = list.Max();
			}
			return result;
		}

		private static string GetCellValue(ICell cellObj)
		{
			string result = string.Empty;
			try
			{
				if (cellObj.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cellObj))
				{
					result = Convert.ToString(DateUtil.GetJavaDate(cellObj.NumericCellValue)).Replace("0:00:00", "").Trim();
				}
				else
				{
					CellType cellType = cellObj.CellType;
					if (cellType == CellType.Numeric || cellType == CellType.Formula)
					{
						try
						{
							result = Convert.ToString(cellObj.NumericCellValue);
						}
						catch (Exception)
						{
							try
							{
								result = Convert.ToString(cellObj.StringCellValue);
							}
							catch (Exception)
							{
							}
						}
					}
					else
					{
						result = Convert.ToString(cellObj).Trim();
					}
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		private static int GetLastRowNum(ISheet sheet)
		{
			return Math.Max(sheet.PhysicalNumberOfRows, sheet.LastRowNum);
		}
	}
}
