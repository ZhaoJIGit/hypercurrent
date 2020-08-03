namespace JieNor.Megi.EntityModel.BD
{
	public class SalaryItemTreeChildModel
	{
		public string id
		{
			get;
			set;
		}

		public string text
		{
			get;
			set;
		}

		public string _parentId
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		public bool MIsActive
		{
			get
			{
				return IsActive;
			}
			set
			{
			}
		}

		public int ItemType
		{
			get;
			set;
		}
	}
}
