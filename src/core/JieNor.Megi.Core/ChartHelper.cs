using JieNor.Megi.Core.DataModel;
using System;

namespace JieNor.Megi.Core
{
	public class ChartHelper
	{
		public static ChartScaleModel GetChartScaleModel(double maxAmt, double minAmt, int scaleCount)
		{
			if (minAmt > maxAmt)
			{
				double num = maxAmt;
				maxAmt = minAmt;
				minAmt = num;
			}
			if (maxAmt == 0.0 && minAmt == 0.0)
			{
				maxAmt = 20.0;
			}
			double num2 = Math.Abs(maxAmt);
			double num3 = Math.Abs(minAmt);
			num2 = ((num2 % (double)scaleCount == 0.0) ? num2 : ((double)(((int)num2 / scaleCount + 1) * scaleCount)));
			num3 = ((num3 % (double)scaleCount == 0.0) ? num3 : ((double)(((int)num3 / scaleCount + 1) * scaleCount)));
			ChartScaleModel chartScaleModel = new ChartScaleModel();
			if (num2 > num3)
			{
				chartScaleModel.start_scale = 0.0 - num2;
				chartScaleModel.end_scale = num2;
				chartScaleModel.scale_space = num2 * 2.0 / (double)scaleCount;
			}
			else
			{
				chartScaleModel.start_scale = 0.0 - num3;
				chartScaleModel.end_scale = num3;
				chartScaleModel.scale_space = num3 * 2.0 / (double)scaleCount;
			}
			chartScaleModel.originy = 0.0;
			return chartScaleModel;
		}

		public static ChartScaleModel GetChartScaleModel(double maxAmt, double minAmt)
		{
			int scaleCount = 4;
			return GetChartScaleModel(maxAmt, minAmt, scaleCount);
		}
	}
}
