using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JieNor.Megi.Core.Utility
{
	public class ResultHelper
	{
		public static OperationResult ToOperationResult<T>(List<T> models) where T : BaseModel
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			List<T> list = (from x in models
			where x.ValidationErrors != null && Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)x.ValidationErrors)
			select x).ToList();
			if (list != null && list.Any())
			{
				operationResult.MessageList = (from x in Union((from x in list
				select x.ValidationErrors).ToList())
				select x.Message).ToList();
				operationResult.Message = string.Join(";", operationResult.MessageList);
				operationResult.Success = false;
			}
			return operationResult;
		}

		public static ImportResult ToImportResult<T>(List<T> models) where T : BaseModel
		{
			ImportResult importResult = new ImportResult
			{
				Success = true
			};
			List<T> list = (from x in models
			where x.ValidationErrors != null && Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)x.ValidationErrors)
			select x).ToList();
			if (list != null && list.Any())
			{
				importResult.MessageList = (from x in Union((from x in list
				select x.ValidationErrors).ToList())
				select MText.Decode(Regex.Replace(x.Message, "<.*?>", "")).RemoveLineBreaks()).ToList();
				importResult.Message = string.Join(";", importResult.MessageList);
				importResult.Success = false;
			}
			return importResult;
		}

		public static OperationResult ToOperationResult<T>(T model) where T : BaseModel
		{
			return ToOperationResult(new List<T>
			{
				model
			});
		}

		public static ImportResult ToImportResult<T>(T model) where T : BaseModel
		{
			return ToImportResult(new List<T>
			{
				model
			});
		}

		private static List<T> Union<T>(List<List<T>> list)
		{
			List<T> list2 = new List<T>();
			for (int i = 0; i < list.Count; i++)
			{
				list2.AddRange((IEnumerable<T>)list[i]);
			}
			return list2;
		}
	}
}
