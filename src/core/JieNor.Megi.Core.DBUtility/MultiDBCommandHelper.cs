using System.Collections.Generic;

namespace JieNor.Megi.Core.DBUtility
{
	public class MultiDBCommandHelper
	{
		public static MultiDBCommand[] ToArray(List<MultiDBCommand> cmdList)
		{
			int count = cmdList.Count;
			MultiDBCommand[] array = new MultiDBCommand[count];
			int num = 0;
			for (int i = 0; i < cmdList.Count; i++)
			{
				array[num] = cmdList[i];
			}
			return array;
		}
	}
}
