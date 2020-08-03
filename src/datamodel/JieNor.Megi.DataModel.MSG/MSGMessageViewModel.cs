using JieNor.Megi.Core;
using System.Collections.Generic;

namespace JieNor.Megi.DataModel.MSG
{
	public class MSGMessageViewModel : MSGMessageModel
	{
		public string MSenderFirstName
		{
			get;
			set;
		}

		public string MSenderEmail
		{
			get;
			set;
		}

		public string MSenderLastName
		{
			get;
			set;
		}

		public string MReceiverFirstName
		{
			get;
			set;
		}

		public string MReceiverLastName
		{
			get;
			set;
		}

		public string MLCID
		{
			get;
			set;
		}

		public string MReceiverEmail
		{
			get;
			set;
		}

		public List<MSGMessageListModel> MGroupMessageList
		{
			get;
			set;
		}

		public MSGMessageUserModel MSenderInfo
		{
			get
			{
				MSGMessageUserModel mSGMessageUserModel = new MSGMessageUserModel();
				mSGMessageUserModel.MUserID = base.MSenderID;
				mSGMessageUserModel.MUserName = MConverter.ToUserName(MLCID, MSenderFirstName, MSenderLastName);
				mSGMessageUserModel.MEmail = MSenderEmail;
				return mSGMessageUserModel;
			}
			set
			{
			}
		}

		public MSGMessageUserModel MReceiverInfo
		{
			get
			{
				MSGMessageUserModel mSGMessageUserModel = new MSGMessageUserModel();
				mSGMessageUserModel.MUserID = base.MReceiverID;
				mSGMessageUserModel.MUserName = MConverter.ToUserName(MLCID, MReceiverFirstName, MReceiverLastName);
				mSGMessageUserModel.MEmail = MReceiverEmail;
				return mSGMessageUserModel;
			}
			set
			{
			}
		}
	}
}
