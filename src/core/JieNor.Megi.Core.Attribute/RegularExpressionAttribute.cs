using System.Text.RegularExpressions;

namespace JieNor.Megi.Core.Attribute
{
	public class RegularExpressionAttribute : ValidationBaseAttribute
	{
		public string Expression
		{
			get;
			set;
		}

		public RegularExpressionAttribute(string expression, string langKey)
		{
			Expression = expression;
			base.LangKey = langKey;
		}

		public override bool IsValid(string value)
		{
			return Regex.IsMatch(value, Expression);
		}
	}
}
