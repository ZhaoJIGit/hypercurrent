using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASMegiLanguageModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MAppID
		{
			get;
			set;
		}

		[DataMember]
		public string MKeyName
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceFile
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public string MHelperUrl
		{
			get;
			set;
		}

		[DataMember]
		public string MVersion
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

		[DataMember]
		public bool MIsDelete
		{
			get;
			set;
		}

		[DataMember]
		public string MCreatorID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCreateDate
		{
			get;
			set;
		}

		[DataMember]
		public string MModifierID
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
		public string MTooltip
		{
			get;
			set;
		}
	}
}
