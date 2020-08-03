using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSStorageModel : BDModel
	{
		/// <summary>
		/// 服务器标识
		/// </summary>
		[DataMember]
		public string MServerID
		{
			get;
			set;
		}

		/// <summary>
		/// 服务器名称(加密)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MDBServerName
		{
			get;
			set;
		}

		/// <summary>
		/// 服务器端口(加密)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MDBServerPort
		{
			get;
			set;
		}

		/// <summary>
		/// 用户名(加密)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MUserName
		{
			get;
			set;
		}

		/// <summary>
		/// 密码(加密)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MPassWord
		{
			get;
			set;
		}

		[DataMember]
		public string MConOtherInfo
		{
			get;
			set;
		}

		/// <summary>
		/// 数据库名称(加密)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MBDName
		{
			get;
			set;
		}

		[DataMember]
		public string MStandardDBName
		{
			get;
			set;
		}

		public SYSStorageModel()
			: base("T_Sys_Storage")
		{
		}
	}
}
