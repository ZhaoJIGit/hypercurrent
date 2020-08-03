using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSStorageModel : BDModel
	{
		/// <summary>
		/// ��������ʶ
		/// </summary>
		[DataMember]
		public string MServerID
		{
			get;
			set;
		}

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

		[DataMember]
		public string MConOtherInfo
		{
			get;
			set;
		}

		/// <summary>
		/// ���ݿ�����(����)
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
