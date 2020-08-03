using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDTrackSimpleModel
	{
		private List<MultiLanguageFieldList> _multiLanguage;

		[DataMember]
		[ApiMember("TrackingCategoryName", ApiMemberType.MultiLang, false, false)]
		public string MCategoryName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackingOptionName", ApiMemberType.MultiLang, false, false)]
		public string MOptionName
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
	}
}
