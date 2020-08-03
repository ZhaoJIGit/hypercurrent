using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASDataDictionary
	{
		public static List<BASDataDictionaryModel> GetDictList(string codeTableName, string languageCode)
		{
			return GetDictList(codeTableName, languageCode, null, false);
		}

		public static List<BASDataDictionaryModel> GetDictList(string codeTableName, string languageCode, List<string> filterValues, bool ignoreLocale = false)
		{
			int num = 0;
			num = ((filterValues != null) ? filterValues.Count : 0);
			string text = "SELECT T.MEntryID AS DictCode,TL.MName AS DictName, T.MValue AS DictValue, T.MParentEntryID as ParentDictCode FROM T_Bas_DictEntry T\r\n                        INNER JOIN T_Bas_DictEntry_L TL on T.MEntryID = TL.MParentID and TL.MIsDelete = 0 \r\n                        WHERE T.MDictName = @MCodeName and T.MIsDelete = 0  ";
			if (!ignoreLocale)
			{
				text += " AND TL.MLocaleID = @LaunguageCode";
			}
			MySqlParameter[] array = new MySqlParameter[num + 2];
			array[0] = new MySqlParameter("@MCodeName", codeTableName);
			array[1] = new MySqlParameter("@LaunguageCode", languageCode);
			if (num > 0)
			{
				int num2 = 0;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string filterValue in filterValues)
				{
					string arg = $"value{num2}";
					stringBuilder.AppendFormat(" T.MValue=@{0} OR", arg);
					array[num2 + 2] = new MySqlParameter($"@{arg}", filterValue);
					num2++;
				}
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
				text = $"{text} AND ({stringBuilder})";
			}
			DataSet dataSet = DbHelperMySQL.Query(text, array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return ModelInfoManager.DataTableToList<BASDataDictionaryModel>(dataSet.Tables[0]);
		}
	}
}
