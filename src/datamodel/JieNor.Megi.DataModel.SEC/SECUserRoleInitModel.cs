using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECUserRoleInitModel
	{
		[DataMember]
		public string Id
		{
			get;
			set;
		}

		[DataMember]
		public string MPosition
		{
			get;
			set;
		}

		[DataMember]
		public bool MView
		{
			get;
			set;
		}

		[DataMember]
		public bool MViewDisabled
		{
			get;
			set;
		}

		[DataMember]
		public bool MChange
		{
			get;
			set;
		}

		[DataMember]
		public bool MChangeDisabled
		{
			get;
			set;
		}

		[DataMember]
		public bool MApprove
		{
			get;
			set;
		}

		[DataMember]
		public bool MApproveDisabled
		{
			get;
			set;
		}

		[DataMember]
		public bool MExport
		{
			get;
			set;
		}

		[DataMember]
		public bool MExportDisabled
		{
			get;
			set;
		}

		[DataMember]
		public List<SECUserRoleInitModel> children
		{
			get;
			set;
		}
	}
}
