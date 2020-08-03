using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.HtmlHelper;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlExpenseType
	{
		public static MvcHtmlString SelectOptions()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("<option value=\"{0}\">{1}</option>", 0, HtmlLang.Write(LangModule.BD, "Manufacturing Expense", "Manufacturing Expense")));
			stringBuilder.Append(string.Format("<option value=\"{0}\">{1}</option>", 1, HtmlLang.Write(LangModule.BD, "SellingExpenses", "Selling Expenses")));
			stringBuilder.Append(string.Format("<option value=\"{0}\">{1}</option>", 2, HtmlLang.Write(LangModule.BD, "RDExpenses", "Research and Development Expenditure")));
			stringBuilder.Append(string.Format("<option value=\"{0}\">{1}</option>", 3, HtmlLang.Write(LangModule.BD, "AdministrationExpense", "Administration Expense")));
			stringBuilder.Append(string.Format("<option value=\"{0}\">{1}</option>", 4, HtmlLang.Write(LangModule.BD, "OtherExpenses", "Other Expenses")));
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
