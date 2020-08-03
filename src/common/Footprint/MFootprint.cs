using JieNor.Megi.EntityModel.Context;
using Segment;
using Segment.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace JieNor.Megi.Common.Footprint
{
	public class MFootprint
	{
		private static Hashtable FootprintPool = Hashtable.Synchronized(new Hashtable());

		private static string SegmentKey = ConfigurationManager.AppSettings["SegmentKey"];

		protected bool SegmentIdentified
		{
			get;
			set;
		}

		protected bool SegmentInitialized
		{
			get;
			set;
		}

		private static MFootprint GetFootprint(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return null;
			}
			object fp = FootprintPool[key];
			if (fp == null)
			{
				fp = new MFootprint();
				lock (FootprintPool.SyncRoot)
				{
					FootprintPool[key] = fp;
				}
			}
			return fp as MFootprint;
		}

		public static MFootprint Identify(MContext ctx, IDictionary<string, object> extTraits)
		{
			try
			{
				string key = ctx.MAccessToken;
				string userId = ctx.MUserID;
				Dictionary<string, object> traits = new Dictionary<string, object>();
				traits.Add("user_email", ctx.MEmail);
				traits.Add("user_name", ctx.MUserName);
				traits.Add("org_name", ctx.MOrgName);
				traits.Add("org_edition", (ctx.MOrgVersionID == 0) ? "Standard" : "SmartLedger");
				traits.Add("org_expiry", ctx.MExpireDate.ToString("yyyy-MM-dd"));
				if (extTraits != null)
				{
					foreach (KeyValuePair<string, object> extTrait in extTraits)
					{
						traits.Add(extTrait.Key, extTrait.Value);
					}
				}
				return Identify(key, ctx.MUserID, traits);
			}
			catch (Exception)
			{
			}
			return new MFootprint();
		}

		public static MFootprint Identify(MContext ctx)
		{
			return Identify(ctx, null);
		}

		public static MFootprint Identify(string key, string userId, IDictionary<string, object> traits)
		{
			try
			{
				MFootprint fp = GetFootprint(key);
				fp.Initialize();
				fp.SegmentIdentify(userId, traits);
				return fp;
			}
			catch (Exception)
			{
			}
			return new MFootprint();
		}

		public static MFootprint Track(MContext ctx, string evt, IDictionary<string, object> properties, Options options, IDictionary<string, object> extTraits)
		{
			try
			{
				MFootprint fp = Identify(ctx, extTraits);
				fp.SegmentTrack(ctx.MUserID, evt, properties, options);
			}
			catch (Exception)
			{
			}
			return new MFootprint();
		}

		public static MFootprint Track(MContext ctx, string evt, IDictionary<string, object> properties, Options options)
		{
			return Track(ctx, evt, properties, options, null);
		}

		public static MFootprint Track(MContext ctx, string evt, IDictionary<string, object> properties)
		{
			return Track(ctx, evt, properties, null, null);
		}

		public static MFootprint Track(MContext ctx, string evt, Options options)
		{
			return Track(ctx, evt, null, options, null);
		}

		public static MFootprint Track(MContext ctx, string evt)
		{
			return Track(ctx, evt, null, null, null);
		}

		public static MFootprint Track(string key, string evt, IDictionary<string, object> properties, Options options, string userId, Dictionary<string, object> traits)
		{
			try
			{
				MFootprint fp = GetFootprint(key);
				fp.SegmentTrack(userId, evt, properties, options);
			}
			catch (Exception)
			{
			}
			return new MFootprint();
		}

		public static MFootprint Track(string key, string evt, string userId = null, Dictionary<string, object> traits = null)
		{
			return Track(key, evt, null, null, userId, traits);
		}

		public static MFootprint Track(string key, string evt, IDictionary<string, object> properties, string userId = null, Dictionary<string, object> traits = null)
		{
			return Track(key, evt, properties, null, userId, traits);
		}

		public static MFootprint Track(string key, string evt, Options options, string userId = null, Dictionary<string, object> traits = null)
		{
			return Track(key, evt, null, options, userId, traits);
		}

		public static MFootprint Page(MContext ctx, string evt, string category, IDictionary<string, object> properties, Options options, Dictionary<string, object> extTraits)
		{
			try
			{
				MFootprint fp = Identify(ctx, extTraits);
				fp.SegmentPage(ctx.MUserID, evt, category, properties, options);
			}
			catch (Exception)
			{
			}
			return new MFootprint();
		}

		public static MFootprint Page(MContext ctx, string evt, string category, IDictionary<string, object> properties, Options options)
		{
			return Page(ctx, evt, category, properties, options, null);
		}

		public static MFootprint Page(MContext ctx, string evt, string category, IDictionary<string, object> properties)
		{
			return Page(ctx, evt, category, properties, null, null);
		}

		public static MFootprint Page(MContext ctx, string evt, string category, Options options)
		{
			return Page(ctx, evt, category, null, options, null);
		}

		public static MFootprint Page(MContext ctx, string evt, string category)
		{
			return Page(ctx, evt, category, null, null, null);
		}

		public static MFootprint Page(string key, string evt, string category, IDictionary<string, object> properties, Options options, string userId, Dictionary<string, object> traits)
		{
			try
			{
				MFootprint fp = GetFootprint(key);
				fp.SegmentPage(userId, evt, category, properties, options);
			}
			catch (Exception)
			{
			}
			return new MFootprint();
		}

		public static MFootprint Page(string key, string evt, string category, IDictionary<string, object> properties, string userId = null, Dictionary<string, object> traits = null)
		{
			return Page(key, evt, category, properties, null, userId, traits);
		}

		public static MFootprint Page(string key, string evt, string category, Options options, string userId = null, Dictionary<string, object> traits = null)
		{
			return Page(key, evt, category, null, options, userId, traits);
		}

		public static MFootprint Page(string key, string evt, string category, string userId = null, Dictionary<string, object> traits = null)
		{
			return Page(key, evt, category, null, null, userId, traits);
		}

		protected MFootprint Initialize()
		{
			try
			{
				SegmentInitialzie();
			}
			catch (Exception)
			{
			}
			return this;
		}

		protected MFootprint SegmentInitialzie()
		{
			try
			{
				if (SegmentInitialized)
				{
					return this;
				}
				Analytics.Initialize(SegmentKey);
				SegmentInitialized = true;
			}
			catch (Exception)
			{
				SegmentInitialized = false;
			}
			return this;
		}

		protected MFootprint SegmentIdentify(string userId, IDictionary<string, object> traits)
		{
			try
			{
				if (!SegmentInitialized || !SegmentIdentified)
				{
					return this;
				}
				Analytics.Client.Identify(userId, traits);
				SegmentIdentified = true;
			}
			catch (Exception)
			{
				SegmentIdentified = false;
			}
			return this;
		}

		protected MFootprint SegmentTrack(string userId, string evt, IDictionary<string, object> properties, Options options)
		{
			try
			{
				if (!SegmentInitialized || !SegmentIdentified)
				{
					return this;
				}
				Analytics.Client.Track(userId, evt, properties, options);
			}
			catch (Exception)
			{
			}
			return this;
		}

		protected MFootprint SegmentPage(string userId, string evt, string category, IDictionary<string, object> properties, Options options)
		{
			try
			{
				if (!SegmentInitialized)
				{
					return this;
				}
				Analytics.Client.Page(userId, evt, category, properties, options);
			}
			catch (Exception)
			{
			}
			return this;
		}
	}
}
