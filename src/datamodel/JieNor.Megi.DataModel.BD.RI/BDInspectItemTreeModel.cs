using JieNor.Megi.DataModel.RI;
using System.Collections.Generic;

namespace JieNor.Megi.DataModel.BD.RI
{
	public class BDInspectItemTreeModel
	{
		public int __active__ = 1;

		public string id
		{
			get;
			set;
		}

		public string text
		{
			get;
			set;
		}

		public List<BDInspectItemTreeModel> children
		{
			get;
			set;
		}

		public int MIndex
		{
			get;
			set;
		}

		public bool MEnable
		{
			get;
			set;
		}

		public bool MRequirePass
		{
			get;
			set;
		}

		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		public bool MIsDelete
		{
			get;
			set;
		}

		public string MSettingID
		{
			get;
			set;
		}

		public string MSettingParamID
		{
			get;
			set;
		}

		public RICategorySettingParamModel MParameter
		{
			get;
			set;
		}
	}
}
