using System;

namespace JieNor.Megi.EntityModel.Enum
{
	[Flags]
	public enum OperateTime
	{
		Save = 0x1,
		Approve = 0x2,
		Verification = 0x4,
		Print = 0x8,
		Export = 0x10,
		Import = 0x20,
		Draft = 0x40,
		All = 0x7F
	}
}
