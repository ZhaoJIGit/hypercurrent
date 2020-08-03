using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactslModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		public string MPKID
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public string MLocaleID
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MPAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MPStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MOurFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MOurLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MPRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MRealAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MRealStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MRealRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MBankAccName
		{
			get;
			set;
		}

		[DataMember]
		public string MBankName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		public BDContactslModel()
			: base("BD_Contacts_L")
		{
		}
	}
}
