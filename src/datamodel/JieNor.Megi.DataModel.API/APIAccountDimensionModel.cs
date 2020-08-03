using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;

namespace JieNor.Megi.DataModel.API
{
	public class APIAccountDimensionModel
	{
		[ApiMember("Type")]
		public string MType
		{
			get;
			set;
		}

		[ApiMember("Value")]
		public string MName
		{
			get;
			set;
		}

		[ApiMember("OptionID")]
		public string MOptionID
		{
			get;
			set;
		}

		[ApiMember("OptionName", ApiMemberType.MultiLang, false, false)]
		public string MOptionName
		{
			get;
			set;
		}

		public bool Matched
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		public APIAccountDimensionModel()
		{
		}

		public APIAccountDimensionModel(DimensionType type, DimensionName name, string optionId, string optionName, bool matched = true, bool isActive = true)
		{
			switch (type)
			{
			case DimensionType.MasterData:
				MType = "MASTERDATA";
				break;
			case DimensionType.Tracking:
				MType = "TRACKING";
				break;
			}
			switch (name)
			{
			case DimensionName.Contact:
				MName = "Contact";
				break;
			case DimensionName.Employee:
				MName = "Employee";
				break;
			case DimensionName.Item:
				MName = "Item";
				break;
			case DimensionName.ExpenseItems:
				MName = "ExpenseItems";
				break;
			case DimensionName.PayrollItems:
				MName = "PayrollItems";
				break;
			}
			MOptionID = optionId;
			MOptionName = optionName;
			Matched = matched;
			IsActive = IsActive;
		}

		public APIAccountDimensionModel(DimensionType type, string name, string optionId, string optionName, bool matched = true, bool isActive = true)
		{
			switch (type)
			{
			case DimensionType.MasterData:
				MType = "MASTERDATA";
				break;
			case DimensionType.Tracking:
				MType = "TRACKING";
				break;
			}
			MName = name;
			MOptionID = optionId;
			MOptionName = optionName;
			Matched = matched;
			IsActive = isActive;
		}
	}
}
