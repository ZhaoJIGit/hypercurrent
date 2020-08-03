using System;

namespace JieNor.Megi.EntityModel.Enum
{
	public class StringCompute
	{
		private char[] _ArrChar1;

		private char[] _ArrChar2;

		private Result _Result;

		private DateTime _BeginTime;

		private DateTime _EndTime;

		private int _ComputeTimes;

		private int[,] _Matrix;

		private int _Column;

		private int _Row;

		public Result ComputeResult => _Result;

		public StringCompute(string str1, string str2)
		{
			StringComputeInit(str1, str2);
		}

		public StringCompute()
		{
		}

		private void StringComputeInit(string str1, string str2)
		{
			_ArrChar1 = str1.ToCharArray();
			_ArrChar2 = str2.ToCharArray();
			_Result = default(Result);
			_ComputeTimes = 0;
			_Row = _ArrChar1.Length + 1;
			_Column = _ArrChar2.Length + 1;
			_Matrix = new int[_Row, _Column];
		}

		public void Compute()
		{
			_BeginTime = DateTime.Now;
			InitMatrix();
			int num = 0;
			for (int i = 1; i < _Row; i++)
			{
				for (int j = 1; j < _Column; j++)
				{
					num = ((_ArrChar1[i - 1] != _ArrChar2[j - 1]) ? 1 : 0);
					int[,] matrix = _Matrix;
					int num2 = i;
					int num3 = j;
					int num4 = Minimum(_Matrix[i - 1, j] + 1, _Matrix[i, j - 1] + 1, _Matrix[i - 1, j - 1] + num);
					matrix[num2, num3] = num4;
					_ComputeTimes++;
				}
			}
			_EndTime = DateTime.Now;
			int value = (_Row > _Column) ? _Row : _Column;
			_Result.Rate = decimal.One - (decimal)_Matrix[_Row - 1, _Column - 1] / (decimal)value;
			_Result.UseTime = (_EndTime - _BeginTime).ToString();
			_Result.ComputeTimes = _ComputeTimes.ToString();
			_Result.Difference = _Matrix[_Row - 1, _Column - 1];
		}

		public void SpeedyCompute()
		{
			InitMatrix();
			int num = 0;
			for (int i = 1; i < _Row; i++)
			{
				for (int j = 1; j < _Column; j++)
				{
					num = ((_ArrChar1[i - 1] != _ArrChar2[j - 1]) ? 1 : 0);
					int[,] matrix = _Matrix;
					int num2 = i;
					int num3 = j;
					int num4 = Minimum(_Matrix[i - 1, j] + 1, _Matrix[i, j - 1] + 1, _Matrix[i - 1, j - 1] + num);
					matrix[num2, num3] = num4;
					_ComputeTimes++;
				}
			}
			int value = (_Row > _Column) ? _Row : _Column;
			_Result.Rate = decimal.One - (decimal)_Matrix[_Row - 1, _Column - 1] / (decimal)value;
			_Result.ComputeTimes = _ComputeTimes.ToString();
			_Result.Difference = _Matrix[_Row - 1, _Column - 1];
		}

		public void Compute(string str1, string str2)
		{
			StringComputeInit(str1, str2);
			Compute();
		}

		public void SpeedyCompute(string str1, string str2)
		{
			StringComputeInit(str1, str2);
			SpeedyCompute();
		}

		private void InitMatrix()
		{
			for (int i = 0; i < _Column; i++)
			{
				_Matrix[0, i] = i;
			}
			for (int j = 0; j < _Row; j++)
			{
				_Matrix[j, 0] = j;
			}
		}

		private int Minimum(int First, int Second, int Third)
		{
			int num = First;
			if (Second < num)
			{
				num = Second;
			}
			if (Third < num)
			{
				num = Third;
			}
			return num;
		}
	}
}
