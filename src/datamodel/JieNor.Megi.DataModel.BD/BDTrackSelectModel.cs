using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDTrackSelectModel
	{
		private List<MultiLanguageFieldList> _multiLanguage;

		[DataMember]
		[ApiMember("TrackingCategoryID", IsPKField = true)]
		public string TrackingCategoryID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackingOptionID", IsPKField = true)]
		public string TrackingOptionID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackingCategoryName", ApiMemberType.MultiLang, false, false)]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackingOptionName", ApiMemberType.MultiLang, false, false)]
		public string TrackingOptionName
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
		public List<MultiLanguageFieldList> MultiLanguage
		{
			get
			{
				return _multiLanguage ?? new List<MultiLanguageFieldList>();
			}
			set
			{
				_multiLanguage = value;
			}
		}

		[DataMember]
		public bool IsHaveError
		{
			get;
			set;
		}
	}
}
