using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Core.MultiLanguage
{
	public class MultiLangHelper
	{
		public static void UpdateValue(string localId, string fildName, string filedValue, List<MultiLanguageFieldList> multLangList)
		{
			if (multLangList == null)
			{
				multLangList = new List<MultiLanguageFieldList>();
			}
			MultiLanguageFieldList multiLanguageFieldList = (from x in multLangList
			where x.MFieldName == fildName
			select x).FirstOrDefault();
			if (multiLanguageFieldList == null)
			{
				MultiLanguageFieldList item = new MultiLanguageFieldList
				{
					MFieldName = fildName,
					MMultiLanguageField = new List<MultiLanguageField>
					{
						new MultiLanguageField
						{
							MLocaleID = localId,
							MValue = filedValue
						}
					}
				};
				multLangList.Add(item);
			}
			else
			{
				for (int i = 0; i < multLangList.Count; i++)
				{
					MultiLanguageFieldList multiLanguageFieldList2 = multLangList[i];
					if (!(multiLanguageFieldList2.MFieldName != fildName))
					{
						bool flag = false;
						for (int j = 0; j < multiLanguageFieldList2.MMultiLanguageField.Count; j++)
						{
							if (!(multiLanguageFieldList2.MMultiLanguageField[j].MLocaleID != localId))
							{
								flag = true;
								multLangList[i].MMultiLanguageField[j].MValue = filedValue;
							}
						}
						if (!flag)
						{
							MultiLanguageField item2 = new MultiLanguageField
							{
								MLocaleID = localId,
								MValue = filedValue
							};
							multLangList[i].MMultiLanguageField.Add(item2);
						}
					}
				}
			}
		}
	}
}
