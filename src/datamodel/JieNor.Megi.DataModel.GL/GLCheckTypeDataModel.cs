using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[Serializable]
	public class GLCheckTypeDataModel
	{
		[DataMember]
		public int MCheckType
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckTypeColumnName
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckTypeName
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckTypeGroupID
		{
			get;
			set;
		}

		[DataMember]
		public int MShowType
		{
			get;
			set;
		}

		public List<GLTreeModel> MDataList
		{
			get;
			set;
		}

		public bool MHasDetail
		{
			get
			{
				return MDataList != null && MDataList.Any();
			}
		}
	}
}
