using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSStorageServerModel : BDModel
	{
		/// <summary>
		/// ����������(����)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MDBServerName
		{
			get;
			set;
		}

		/// <summary>
		/// �������˿�(����)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MDBServerPort
		{
			get;
			set;
		}

		/// <summary>
		/// �û���(����)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MUserName
		{
			get;
			set;
		}

		/// <summary>
		/// ����(����)
		/// </summary>
		[DataMember]
		[ColumnEncrypt]
		public string MPassWord
		{
			get;
			set;
		}

		/// <summary>
		/// ���ݿ�ǰ׺
		/// </summary>
		[DataMember]
		public string MDBNamePrefix
		{
			get;
			set;
		}

		/// <summary>
		/// ���ݿ��׼����
		/// </summary>
		[DataMember]
		public string MStandardDBName
		{
			get;
			set;
		}

		/// <summary>
		/// ���ݿ�����
		/// </summary>
		[DataMember]
		public int MDBCount
		{
			get;
			set;
		}

		/// <summary>
		/// ����������
		/// </summary>
		[DataMember]
		public int MServerType
		{
			get;
			set;
		}

		/// <summary>
		/// ������ݿ�����
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
