using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSStorageServerModel : BDModel
	{
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

		/// <summary>
		/// 数据库前缀
		/// </summary>
		[DataMember]
		public string MDBNamePrefix
		{
			get;
			set;
		}

		/// <summary>
		/// 数据库标准名称
		/// </summary>
		[DataMember]
		public string MStandardDBName
		{
			get;
			set;
		}

		/// <summary>
		/// 数据库数量
		/// </summary>
		[DataMember]
		public int MDBCount
		{
			get;
			set;
		}

		/// <summary>
		/// 服务器类型
		/// </summary>
		[DataMember]
		public int MServerType
		{
			get;
			set;
		}

		/// <summary>
		/// 最大数据库数量
		/// </summary>
		[DataMember]
		public bool MMaxDBCount
		{
			get;
			set;
		}
		
		public DateTime MConversionDate
		{
			get;
			set;
		}

		public SYSStorageServerModel()
			: base("T_Sys_StorageServer")
		{
		}
	}
}
