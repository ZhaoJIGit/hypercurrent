using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPrintSettingEntryModel : BDEntryModel
	{
		[DataMember]
		public string MPayItemID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsShow
		{
			get;
			set;
		}

		[DataMember]
		public string MPayItemName
		{
			get;
			set;
		}

		[DataMember]
		public int MPayItemType
		{
			get;
			set;
		}

		public PAPrintSettingEntryModel()
			: base("T_PA_PrintSettingEntry")
		{
		}

		public PAPrintSettingEntryModel(string tableName)
			: base(tableName)
		{
		}
	}
}
