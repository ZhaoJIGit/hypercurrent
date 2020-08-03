using JieNor.Megi.Core.DataModel;
using System;

namespace JieNor.Megi.Core.DBUtility
{
	public class OrgAppStorageModel
	{
		public string MDetailID
		{
			get;
			set;
		}

		public string MOrgID
		{
			get;
			set;
		}

		[ColumnEncrypt]
		public string MDBServerName
		{
			get;
			set;
		}

		[ColumnEncrypt]
		public string MDBServerPort
		{
			get;
			set;
		}

		[ColumnEncrypt]
		public string MUserName
		{
			get;
			set;
		}

		[ColumnEncrypt]
		public string MPassWord
		{
			get;
			set;
		}

		[ColumnEncrypt]
		public string MBDName
		{
			get;
			set;
		}

		public string MConOtherInfo
		{
			get;
			set;
		}

		public bool MIsDelete
		{
			get;
			set;
		}

		public string MCreatorID
		{
			get;
			set;
		}

		public DateTime MCreateDate
		{
			get;
			set;
		}

		public string MModifierID
		{
			get;
			set;
		}

		public DateTime MModifyDate
		{
			get;
			set;
		}
	}
}
