using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Core.DisLock
{
	public class DisLockConfig
	{
		public TimeSpan DefaultRelease
		{
			get;
			set;
		} = new TimeSpan(0, 5, 0);


		public int DefaultReleaseMin
		{
			set
			{
				DefaultRelease = new TimeSpan(0, value, 0);
			}
		}

		public int RetryInterval
		{
			get;
			set;
		} = 200;


		public int RetryCount
		{
			get;
			set;
		} = 25;


		public bool SyncLockEnable
		{
			get;
			set;
		}

		public int HystrixInterval
		{
			get;
			set;
		} = 5;


		public List<LockOperation> LockOperations
		{
			get;
			set;
		}
	}
}
