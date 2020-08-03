using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RI
{
	[DataContract]
	public class RIInspectionResult
	{
		[DataMember]
		private string[] _urlParam;

		[DataMember]
		private string[] _messageParam;

		[DataMember]
		public string MMessage
		{
			get;
			set;
		}

		[DataMember]
		public bool MAccountIsDisable
		{
			get;
			set;
		}

		[DataMember]
		public string MTopPassedMessage
		{
			get;
			set;
		}

		[DataMember]
		public string MTopFailedMessage
		{
			get;
			set;
		}

		[DataMember]
		public string MLinkUrl
		{
			get;
			set;
		}

		[DataMember]
		public bool MPassed
		{
			get;
			set;
		}

		[DataMember]
		public bool MRequirePass
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
		public bool MNoLinkUrlIfPassed
		{
			get;
			set;
		}

		[DataMember]
		public bool MNoNeedInspect
		{
			get;
			set;
		}

		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string[] MUrlParam
		{
			get
			{
				return _urlParam ?? new string[0];
			}
			set
			{
				_urlParam = value;
			}
		}

		[DataMember]
		public string[] MMessageParam
		{
			get
			{
				return _messageParam ?? new string[0];
			}
			set
			{
				_messageParam = value;
			}
		}

		[DataMember]
		public List<RIInspectionResult> children
		{
			get;
			set;
		}
	}
}
