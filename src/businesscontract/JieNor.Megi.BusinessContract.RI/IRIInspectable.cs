using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.RI
{
	public abstract class IRIInspectable
	{
		public List<Func<MContext, RICategoryModel, int, int, RIInspectionResult>> enginers;

		public GLDataPool GetDataPool(MContext ctx, int year = 0, int period = 0, int periodDiff = 6, bool equalToken = true)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			return GLDataPool.GetInstance(ctx, equalToken, year, period, periodDiff);
		}
	}
}
