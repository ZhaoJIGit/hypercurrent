using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAccountEditModel : BDAccountModel
	{
		[DataMember]
		public int MBankAccountType
		{
			get;
			set;
		}

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAllowEdit
		{
			get;
			set;
		}

		[DataMember]
		public string MExistParentID
		{
			get;
			set;
		}

		[DataMember]
		public string MOriParentNumber
		{
			get;
			set;
		}

		[DataMember]
		public IOAccountMatchResultEnum MatchResult
		{
			get;
			set;
		}

		[DataMember]
		public string MMatchNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MNewNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MNewNumberSaved
		{
			get;
			set;
		}

		[DataMember]
		public string MParentCode
		{
			get;
			set;
		}

		[DataMember]
		public string MNumberStandard
		{
			get;
			set;
		}

		public string MOriName
		{
			get;
			set;
		}

		[DataMember]
		public string MOriNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MMigrationID
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceCheckType
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceCurrency
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceID
		{
			get;
			set;
		}

		[DataMember]
		public string MSourcePKFields
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceCode
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceName
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiID
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiCode
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiName
		{
			get;
			set;
		}

		[DataMember]
		public List<BDAccountListModel> SysAcctList
		{
			get;
			set;
		}

		[DataMember]
		public List<BDContactsEditModel> ContactList
		{
			get;
			set;
		}

		[DataMember]
		public List<BDEmployeesModel> EmployeeList
		{
			get;
			set;
		}

		[DataMember]
		public List<BDItemModel> ItemList
		{
			get;
			set;
		}

		[DataMember]
		public List<BDTrackModel> TrackList
		{
			get;
			set;
		}

		[DataMember]
		public List<MigrateLogBaseModel> MigrateLogList
		{
			get;
			set;
		}

		[DataMember]
		public string SaveFlag
		{
			get;
			set;
		}

		public BDAccountEditModel()
		{
			base.MultiLanguage = new List<MultiLanguageFieldList>();
		}

		public BDAccountEditModel(string tableName)
			: base(tableName)
		{
		}
	}
}
