using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace JieNor.Megi.Core.MResource
{
	public class MResouceBase : IDisposable
	{
		private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

		private bool disposed = false;

		public DateTime ModifyTime
		{
			get;
			set;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					handle.Dispose();
				}
				disposed = true;
			}
		}
	}
}
