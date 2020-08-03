using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsInfoFilterModel : SqlWhere
	{
		[DataMember]
		public string typeId
		{
			get;
			set;
		}

		[DataMember]
		public string searchFilter
		{
			get;
			set;
		}

		[DataMember]
		public string keyword
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFromExport
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFromExcel
		{
			get;
			set;
		}

		[DataMember]
		public string Sort
		{
			get;
			set;
		}

		[DataMember]
		public string Order
		{
			get;
			set;
		}

		[DataMember]
		public bool showInvoice
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MModifyDate
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeArchived
		{
			get;
			set;
		}

		public string ItemID
		{
			get;
			set;
		}
	}
}
