using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPayItemGroupAmtModel : ICloneable
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public int ItemType
		{
			get;
			set;
		}

		[DataMember]
		public string ItemTypeName
		{
			get;
			set;
		}

		[DataMember]
		public decimal Amount
		{
			get;
			set;
		}

		[DataMember]
		public int MCoefficient
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
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
		public string MPayItemID
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
		public bool MIsActive
		{
			get;
			set;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
