using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Context
{
	[DataContract]
	public class MContext
	{
		private string _mLCID = string.Empty;

		private string _mDefaultLocaleID = string.Empty;

		[DataMember]
		public List<string> MActiveLocaleIDS = new List<string>();

		[DataMember]
		private DateTime _mBeginDate;

		[DataMember]
		private DateTime _mGLBeginDate;

		[DataMember]
		private bool _mFAInited;

		[DataMember]
		private string _mFAFrefix;

		[DataMember]
		private DateTime _mFABeginDate;

		[DataMember]
		public bool IsExpired
		{
			get
			{
				return MLastAccessTime.AddMinutes((double)MAccessTokenValidMinute) < DateNow || MLogout;
			}
			private set
			{
			}
		}

		[DataMember]
		public string MEmail
		{
			get;
			set;
		}

		[DataMember]
		public string MLCID
		{
			get
			{
				return string.IsNullOrEmpty(_mLCID) ? MDefaultLocaleID : _mLCID;
			}
			set
			{
				_mLCID = value;
			}
		}

		[DataMember]
		public string MUserID
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
		public int LoginRedirect
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgName
		{
			get;
			set;
		}

		[DataMember]
		public int MOrgVersionID
		{
			get;
			set;
		}

		[DataMember]
		public Dictionary<string, bool> Access
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultLocaleID
		{
			get
			{
				return string.IsNullOrEmpty(_mDefaultLocaleID) ? LangCodeEnum.EN_US : _mDefaultLocaleID;
			}
			set
			{
				_mDefaultLocaleID = value;
			}
		}

		[DataMember]
		public string MMaster
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpireDate
		{
			get;
			set;
		}

		[DataMember]
		public string MUsedStatusID
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
		public string MUserName
		{
			get
			{
				return (MLCID == "0x7804" || MLCID == "0x7C04") ? (MLastName + MFirstName) : (MFirstName + " " + MLastName);
			}
			set
			{
			}
		}

		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MMobilePhone
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MRegDate
		{
			get;
			set;
		}

		public DateTime DateNow
		{
			get
			{
				if (string.IsNullOrEmpty(MZoneFormat))
				{
					return DateTime.Now;
				}
				DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
				return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(MZoneFormat));
			}
			set
			{
			}
		}

		[DataMember]
		public string DateNowString
		{
			get
			{
				return DateNow.ToString("yyyy-MM-dd HH:mm:ss");
			}
			private set
			{
			}
		}

		[DataMember]
		public string MUserIP
		{
			get;
			set;
		}

		[DataMember]
		public bool IsSys
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MBeginDate
		{
			get
			{
				return _mBeginDate;
			}
			set
			{
				_mBeginDate = value;
			}
		}

		[DataMember]
		public DateTime MGLBeginDate
		{
			get
			{
				return _mGLBeginDate;
			}
			set
			{
				_mGLBeginDate = value;
			}
		}

		[DataMember]
		public bool MFAInited
		{
			get
			{
				return _mFAInited;
			}
			set
			{
				_mFAInited = value;
			}
		}

		[DataMember]
		public string MFAFrefix
		{
			get
			{
				return _mFAFrefix;
			}
			set
			{
				_mFAFrefix = value;
			}
		}

		[DataMember]
		public DateTime MFABeginDate
		{
			get
			{
				return _mFABeginDate;
			}
			set
			{
				_mFABeginDate = value;
			}
		}

		[DataMember]
		public string MBasCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MPosition
		{
			get;
			set;
		}

		[DataMember]
		public int MOrgTaxType
		{
			get;
			set;
		}

		[DataMember]
		public string MRole
		{
			get;
			set;
		}

		[DataMember]
		public bool IsSelfData
		{
			get;
			set;
		}

		[DataMember]
		public bool MExistsOrg
		{
			get;
			set;
		}

		[DataMember]
		public string MZoneFormat
		{
			get;
			set;
		}

		[DataMember]
		public string MDateFormat
		{
			get;
			set;
		}

		[DataMember]
		public string MTimeFormat
		{
			get;
			set;
		}

		[DataMember]
		public string MDigitGrpFormat
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsShowCSymbol
		{
			get;
			set;
		}

		[DataMember]
		public string MAccessToken
		{
			get;
			set;
		}

		[DataMember]
		public string MConsumerKey
		{
			get;
			set;
		}

		[DataMember]
		public string MAccessCode
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastAccessTime
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCurrentLoginTime
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginOrgName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginOrgId
		{
			get;
			set;
		}

		[DataMember]
		public int MAccessTokenValidMinute
		{
			get;
			set;
		}

		[DataMember]
		public string MRedirectUrl
		{
			get;
			set;
		}

		[DataMember]
		public bool MLogout
		{
			get;
			set;
		}

		[DataMember]
		public bool MInitBalanceOver
		{
			get;
			set;
		}

		[DataMember]
		public string MBrowserTabIndex
		{
			get;
			set;
		}

		[DataMember]
		public int MRegProgress
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTableID
		{
			get;
			set;
		}

		[DataMember]
		public List<ModuleEnum> MEnabledModules
		{
			get;
			set;
		}

		[DataMember]
		public int MOrgListShowType
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MUserCreateDate
		{
			get;
			set;
		}

		[DataMember]
		public string MLegalTradingName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsBeta
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxCode
		{
			get;
			set;
		}

		[DataMember]
		public int MServerType
		{
			get;
			set;
		}

		[DataMember]
		public bool MultiLang
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsHadAddOrgAuth
		{
			get;
			set;
		}

		[DataMember]
		public string AppSource
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MResourceIds
		{
			get;
			set;
		}

		[DataMember]
		public int MVoucherNumberLength
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherNumberFilledChar
		{
			get;
			set;
		}

		public MySqlParameter[] GetParameters(MySqlParameter[] exitsParameters)
		{
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", MOrgID),
				new MySqlParameter("@MLocaleID", MLCID),
				new MySqlParameter("@MLCID", MLCID),
				new MySqlParameter("@MUserID", MUserID),
				new MySqlParameter("@MBeginDate", MBeginDate),
				new MySqlParameter("@MGLBeginDate", MGLBeginDate),
				new MySqlParameter("@MFABeginDate", MFABeginDate),
				new MySqlParameter("@DateNow", DateNow),
				new MySqlParameter("@MCreateDate", DateNow),
				new MySqlParameter("@MModifyDate", DateNow),
				new MySqlParameter("@MCreatorID", MUserID),
				new MySqlParameter("@MModifierID", MUserID),
				new MySqlParameter("@AppSource", AppSource),
				new MySqlParameter("@MEmail", MEmail)
			};
			if (exitsParameters != null && exitsParameters.Count() > 0)
			{
				list.AddRange(exitsParameters);
			}
			return list.ToArray();
		}

		public MySqlParameter[] GetParameters(MySqlParameter exitsParameter = null)
		{
			if (exitsParameter != null)
			{
				return GetParameters(new MySqlParameter[1]
				{
					exitsParameter
				});
			}
			return GetParameters(new MySqlParameter[0]);
		}

		public MySqlParameter[] GetParameters(string name, object value)
		{
			return GetParameters(new MySqlParameter[1]
			{
				new MySqlParameter(name, value)
			});
		}
	}
}
