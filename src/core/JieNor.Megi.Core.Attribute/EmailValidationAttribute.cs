namespace JieNor.Megi.Core.Attribute
{
	public class EmailValidationAttribute : RegularExpressionAttribute
	{
		public EmailValidationAttribute()
			: base("^[\\w-]+(\\.[\\w-]+)*@[\\w-]+(\\.[\\w-]+)+$", "EmailInvalid")
		{
		}
	}
}
