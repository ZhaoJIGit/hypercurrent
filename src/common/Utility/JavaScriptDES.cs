using System;

namespace JieNor.Megi.Common.Utility
{
	public static class JavaScriptDES
	{
		public static readonly string DES_KEY = "megi-001";

		public static string EncryptDES(string beinetstr, string beinetkey = null)
		{
			if (string.IsNullOrWhiteSpace(beinetstr))
			{
				return beinetstr;
			}
			beinetkey = (beinetkey ?? DES_KEY);
			return stringToHex(des(beinetkey, beinetstr, true, false, string.Empty));
		}

		public static string DecryptDES(string beinetstr, string beinetkey = null)
		{
			if (string.IsNullOrWhiteSpace(beinetstr))
			{
				return beinetstr;
			}
			beinetkey = (beinetkey ?? DES_KEY);
			return des(beinetkey, HexTostring(beinetstr), false, false, string.Empty);
		}

		public static string stringToHex(string s)
		{
			string r = "";
			string[] hexes = new string[16]
			{
				"0",
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
				"a",
				"b",
				"c",
				"d",
				"e",
				"f"
			};
			for (int i = 0; i < s.Length; i++)
			{
				r = r + hexes[RM(s[i], 4)] + hexes[s[i] & 0xF];
			}
			return r;
		}

		public static string HexTostring(string s)
		{
			string ret = string.Empty;
			for (int i = 0; i < s.Length; i += 2)
			{
				int sxx = Convert.ToInt32(s.Substring(i, 2), 16);
				ret += ((char)(ushort)sxx).ToString();
			}
			return ret;
		}

		public static int RM(int a, int bit)
		{
			return (int)((uint)a >> bit);
		}

		private static string des(string beinetkey, string message, bool encrypt, bool mode, string iv)
		{
			long[] spfunction15 = new long[64]
			{
				16843776L,
				0L,
				65536L,
				16843780L,
				16842756L,
				66564L,
				4L,
				65536L,
				1024L,
				16843776L,
				16843780L,
				1024L,
				16778244L,
				16842756L,
				16777216L,
				4L,
				1028L,
				16778240L,
				16778240L,
				66560L,
				66560L,
				16842752L,
				16842752L,
				16778244L,
				65540L,
				16777220L,
				16777220L,
				65540L,
				0L,
				1028L,
				66564L,
				16777216L,
				65536L,
				16843780L,
				4L,
				16842752L,
				16843776L,
				16777216L,
				16777216L,
				1024L,
				16842756L,
				65536L,
				66560L,
				16777220L,
				1024L,
				4L,
				16778244L,
				66564L,
				16843780L,
				65540L,
				16842752L,
				16778244L,
				16777220L,
				1028L,
				66564L,
				16843776L,
				1028L,
				16778240L,
				16778240L,
				0L,
				65540L,
				66560L,
				0L,
				16842756L
			};
			long[] spfunction14 = new long[64]
			{
				-2146402272L,
				-2147450880L,
				32768L,
				1081376L,
				1048576L,
				32L,
				-2146435040L,
				-2147450848L,
				-2147483616L,
				-2146402272L,
				-2146402304L,
				-2147483648L,
				-2147450880L,
				1048576L,
				32L,
				-2146435040L,
				1081344L,
				1048608L,
				-2147450848L,
				0L,
				-2147483648L,
				32768L,
				1081376L,
				-2146435072L,
				1048608L,
				-2147483616L,
				0L,
				1081344L,
				32800L,
				-2146402304L,
				-2146435072L,
				32800L,
				0L,
				1081376L,
				-2146435040L,
				1048576L,
				-2147450848L,
				-2146435072L,
				-2146402304L,
				32768L,
				-2146435072L,
				-2147450880L,
				32L,
				-2146402272L,
				1081376L,
				32L,
				32768L,
				-2147483648L,
				32800L,
				-2146402304L,
				1048576L,
				-2147483616L,
				1048608L,
				-2147450848L,
				-2147483616L,
				1048608L,
				1081344L,
				0L,
				-2147450880L,
				32800L,
				-2147483648L,
				-2146435040L,
				-2146402272L,
				1081344L
			};
			long[] spfunction13 = new long[64]
			{
				520L,
				134349312L,
				0L,
				134348808L,
				134218240L,
				0L,
				131592L,
				134218240L,
				131080L,
				134217736L,
				134217736L,
				131072L,
				134349320L,
				131080L,
				134348800L,
				520L,
				134217728L,
				8L,
				134349312L,
				512L,
				131584L,
				134348800L,
				134348808L,
				131592L,
				134218248L,
				131584L,
				131072L,
				134218248L,
				8L,
				134349320L,
				512L,
				134217728L,
				134349312L,
				134217728L,
				131080L,
				520L,
				131072L,
				134349312L,
				134218240L,
				0L,
				512L,
				131080L,
				134349320L,
				134218240L,
				134217736L,
				512L,
				0L,
				134348808L,
				134218248L,
				131072L,
				134217728L,
				134349320L,
				8L,
				131592L,
				131584L,
				134217736L,
				134348800L,
				134218248L,
				520L,
				134348800L,
				131592L,
				8L,
				134348808L,
				131584L
			};
			long[] spfunction12 = new long[64]
			{
				8396801L,
				8321L,
				8321L,
				128L,
				8396928L,
				8388737L,
				8388609L,
				8193L,
				0L,
				8396800L,
				8396800L,
				8396929L,
				129L,
				0L,
				8388736L,
				8388609L,
				1L,
				8192L,
				8388608L,
				8396801L,
				128L,
				8388608L,
				8193L,
				8320L,
				8388737L,
				1L,
				8320L,
				8388736L,
				8192L,
				8396928L,
				8396929L,
				129L,
				8388736L,
				8388609L,
				8396800L,
				8396929L,
				129L,
				0L,
				0L,
				8396800L,
				8320L,
				8388736L,
				8388737L,
				1L,
				8396801L,
				8321L,
				8321L,
				128L,
				8396929L,
				129L,
				1L,
				8192L,
				8388609L,
				8193L,
				8396928L,
				8388737L,
				8193L,
				8320L,
				8388608L,
				8396801L,
				128L,
				8388608L,
				8192L,
				8396928L
			};
			long[] spfunction11 = new long[64]
			{
				256L,
				34078976L,
				34078720L,
				1107296512L,
				524288L,
				256L,
				1073741824L,
				34078720L,
				1074266368L,
				524288L,
				33554688L,
				1074266368L,
				1107296512L,
				1107820544L,
				524544L,
				1073741824L,
				33554432L,
				1074266112L,
				1074266112L,
				0L,
				1073742080L,
				1107820800L,
				1107820800L,
				33554688L,
				1107820544L,
				1073742080L,
				0L,
				1107296256L,
				34078976L,
				33554432L,
				1107296256L,
				524544L,
				524288L,
				1107296512L,
				256L,
				33554432L,
				1073741824L,
				34078720L,
				1107296512L,
				1074266368L,
				33554688L,
				1073741824L,
				1107820544L,
				34078976L,
				1074266368L,
				256L,
				33554432L,
				1107820544L,
				1107820800L,
				524544L,
				1107296256L,
				1107820800L,
				34078720L,
				0L,
				1074266112L,
				1107296256L,
				524544L,
				33554688L,
				1073742080L,
				524288L,
				0L,
				1074266112L,
				34078976L,
				1073742080L
			};
			long[] spfunction10 = new long[64]
			{
				536870928L,
				541065216L,
				16384L,
				541081616L,
				541065216L,
				16L,
				541081616L,
				4194304L,
				536887296L,
				4210704L,
				4194304L,
				536870928L,
				4194320L,
				536887296L,
				536870912L,
				16400L,
				0L,
				4194320L,
				536887312L,
				16384L,
				4210688L,
				536887312L,
				16L,
				541065232L,
				541065232L,
				0L,
				4210704L,
				541081600L,
				16400L,
				4210688L,
				541081600L,
				536870912L,
				536887296L,
				16L,
				541065232L,
				4210688L,
				541081616L,
				4194304L,
				16400L,
				536870928L,
				4194304L,
				536887296L,
				536870912L,
				16400L,
				536870928L,
				541081616L,
				4210688L,
				541065216L,
				4210704L,
				541081600L,
				0L,
				541065232L,
				16L,
				16384L,
				541065216L,
				4210704L,
				16384L,
				4194320L,
				536887312L,
				0L,
				541081600L,
				536870912L,
				4194320L,
				536887312L
			};
			long[] spfunction9 = new long[64]
			{
				2097152L,
				69206018L,
				67110914L,
				0L,
				2048L,
				67110914L,
				2099202L,
				69208064L,
				69208066L,
				2097152L,
				0L,
				67108866L,
				2L,
				67108864L,
				69206018L,
				2050L,
				67110912L,
				2099202L,
				2097154L,
				67110912L,
				67108866L,
				69206016L,
				69208064L,
				2097154L,
				69206016L,
				2048L,
				2050L,
				69208066L,
				2099200L,
				2L,
				67108864L,
				2099200L,
				67108864L,
				2099200L,
				2097152L,
				67110914L,
				67110914L,
				69206018L,
				69206018L,
				2L,
				2097154L,
				67108864L,
				67110912L,
				2097152L,
				69208064L,
				2050L,
				2099202L,
				69208064L,
				2050L,
				67108866L,
				69208066L,
				69206016L,
				2099200L,
				0L,
				2L,
				69208066L,
				0L,
				2099202L,
				69206016L,
				2048L,
				67108866L,
				67110912L,
				2048L,
				2097154L
			};
			long[] spfunction8 = new long[64]
			{
				268439616L,
				4096L,
				262144L,
				268701760L,
				268435456L,
				268439616L,
				64L,
				268435456L,
				262208L,
				268697600L,
				268701760L,
				266240L,
				268701696L,
				266304L,
				4096L,
				64L,
				268697600L,
				268435520L,
				268439552L,
				4160L,
				266240L,
				262208L,
				268697664L,
				268701696L,
				4160L,
				0L,
				0L,
				268697664L,
				268435520L,
				268439552L,
				266304L,
				262144L,
				266304L,
				262144L,
				268701696L,
				4096L,
				64L,
				268697664L,
				4096L,
				266304L,
				268439552L,
				64L,
				268435520L,
				268697600L,
				268697664L,
				268435456L,
				262144L,
				268439616L,
				0L,
				268701760L,
				262208L,
				268435520L,
				268697600L,
				268439552L,
				268439616L,
				0L,
				268701760L,
				266240L,
				266240L,
				4160L,
				4160L,
				262208L,
				268435456L,
				268701696L
			};
			int[] keys = des_createKeys(beinetkey);
			int k = 0;
			int cbcleft3 = 0;
			int cbcleft2 = 0;
			int cbcright3 = 0;
			int cbcright2 = 0;
			int len = message.Length;
			int chunk = 0;
			int iterations = (keys.Length == 32) ? 3 : 9;
			int[] looping = (iterations != 3) ? (encrypt ? new int[9]
			{
				0,
				32,
				2,
				62,
				30,
				-2,
				64,
				96,
				2
			} : new int[9]
			{
				94,
				62,
				-2,
				32,
				64,
				2,
				30,
				-2,
				-2
			}) : (encrypt ? new int[3]
			{
				0,
				32,
				2
			} : new int[3]
			{
				30,
				-2,
				-2
			});
			if (encrypt)
			{
				message += "\0\0\0\0\0\0\0\0";
			}
			string result = string.Empty;
			string tempresult = string.Empty;
			if (mode)
			{
				int[] tmp = new int[8];
				int pos = 24;
				int iTmp = 0;
				while (k < iv.Length && iTmp < tmp.Length)
				{
					if (pos < 0)
					{
						pos = 24;
					}
					tmp[iTmp++] = (int)((uint)iv[k++] << pos);
					pos -= 8;
				}
				cbcleft3 = (tmp[0] | tmp[1] | tmp[2] | tmp[3]);
				cbcright3 = (tmp[4] | tmp[5] | tmp[6] | tmp[7]);
				k = 0;
			}
			while (k < len)
			{
				int right16;
				int left13;
				if (encrypt)
				{
					left13 = (int)((uint)message[k++] << 16 | message[k++]);
					right16 = (int)((uint)message[k++] << 16 | message[k++]);
				}
				else
				{
					left13 = (int)((uint)message[k++] << 24 | (uint)message[k++] << 16 | (uint)message[k++] << 8 | message[k++]);
					right16 = (int)((uint)message[k++] << 24 | (uint)message[k++] << 16 | (uint)message[k++] << 8 | message[k++]);
				}
				if (mode)
				{
					if (encrypt)
					{
						left13 ^= cbcleft3;
						right16 ^= cbcright3;
					}
					else
					{
						cbcleft2 = cbcleft3;
						cbcright2 = cbcright3;
						cbcleft3 = left13;
						cbcright3 = right16;
					}
				}
				int temp12 = (RM(left13, 4) ^ right16) & 0xF0F0F0F;
				right16 ^= temp12;
				left13 ^= temp12 << 4;
				temp12 = ((RM(left13, 16) ^ right16) & 0xFFFF);
				right16 ^= temp12;
				left13 ^= temp12 << 16;
				temp12 = ((RM(right16, 2) ^ left13) & 0x33333333);
				left13 ^= temp12;
				right16 ^= temp12 << 2;
				temp12 = ((RM(right16, 8) ^ left13) & 0xFF00FF);
				left13 ^= temp12;
				right16 ^= temp12 << 8;
				temp12 = ((RM(left13, 1) ^ right16) & 0x55555555);
				right16 ^= temp12;
				left13 ^= temp12 << 1;
				left13 = (left13 << 1 | RM(left13, 31));
				right16 = (right16 << 1 | RM(right16, 31));
				for (int j = 0; j < iterations; j += 3)
				{
					int endloop = looping[j + 1];
					int loopinc = looping[j + 2];
					for (int i = looping[j]; i != endloop; i += loopinc)
					{
						int right3 = right16 ^ keys[i];
						int right2 = (RM(right16, 4) | right16 << 28) ^ keys[i + 1];
						temp12 = left13;
						left13 = right16;
						right16 = (int)(temp12 ^ (spfunction14[RM(right3, 24) & 0x3F] | spfunction12[RM(right3, 16) & 0x3F] | spfunction10[RM(right3, 8) & 0x3F] | spfunction8[right3 & 0x3F] | spfunction15[RM(right2, 24) & 0x3F] | spfunction13[RM(right2, 16) & 0x3F] | spfunction11[RM(right2, 8) & 0x3F] | spfunction9[right2 & 0x3F]));
					}
					temp12 = left13;
					left13 = right16;
					right16 = temp12;
				}
				left13 = (RM(left13, 1) | left13 << 31);
				right16 = (RM(right16, 1) | right16 << 31);
				temp12 = ((RM(left13, 1) ^ right16) & 0x55555555);
				right16 ^= temp12;
				left13 ^= temp12 << 1;
				temp12 = ((RM(right16, 8) ^ left13) & 0xFF00FF);
				left13 ^= temp12;
				right16 ^= temp12 << 8;
				temp12 = ((RM(right16, 2) ^ left13) & 0x33333333);
				left13 ^= temp12;
				right16 ^= temp12 << 2;
				temp12 = ((RM(left13, 16) ^ right16) & 0xFFFF);
				right16 ^= temp12;
				left13 ^= temp12 << 16;
				temp12 = ((RM(left13, 4) ^ right16) & 0xF0F0F0F);
				right16 ^= temp12;
				left13 ^= temp12 << 4;
				if (mode)
				{
					if (encrypt)
					{
						cbcleft3 = left13;
						cbcright3 = right16;
					}
					else
					{
						left13 ^= cbcleft2;
						right16 ^= cbcright2;
					}
				}
				if (encrypt)
				{
					tempresult = tempresult + (char)RM(left13, 24) + (char)(RM(left13, 16) & 0xFF) + (char)(RM(left13, 8) & 0xFF) + (char)(left13 & 0xFF) + (char)RM(right16, 24) + (char)(RM(right16, 16) & 0xFF) + (char)(RM(right16, 8) & 0xFF) + (char)(right16 & 0xFF);
				}
				else
				{
					int tmpch4 = RM(left13, 16) & 0xFFFF;
					char c;
					if (tmpch4 != 0)
					{
						string str = tempresult;
						c = (char)tmpch4;
						tempresult = str + c.ToString();
					}
					tmpch4 = (left13 & 0xFFFF);
					if (tmpch4 != 0)
					{
						string str2 = tempresult;
						c = (char)tmpch4;
						tempresult = str2 + c.ToString();
					}
					tmpch4 = (RM(right16, 16) & 0xFFFF);
					if (tmpch4 != 0)
					{
						string str3 = tempresult;
						c = (char)tmpch4;
						tempresult = str3 + c.ToString();
					}
					tmpch4 = (right16 & 0xFFFF);
					if (tmpch4 != 0)
					{
						string str4 = tempresult;
						c = (char)tmpch4;
						tempresult = str4 + c.ToString();
					}
				}
				chunk += (encrypt ? 16 : 8);
				if (chunk == 512)
				{
					result += tempresult;
					tempresult = string.Empty;
					chunk = 0;
				}
			}
			return result + tempresult;
		}

		private static int[] des_createKeys(string beinetkey)
		{
			int[] pc2bytes26 = new int[16]
			{
				0,
				4,
				536870912,
				536870916,
				65536,
				65540,
				536936448,
				536936452,
				512,
				516,
				536871424,
				536871428,
				66048,
				66052,
				536936960,
				536936964
			};
			int[] pc2bytes25 = new int[16]
			{
				0,
				1,
				1048576,
				1048577,
				67108864,
				67108865,
				68157440,
				68157441,
				256,
				257,
				1048832,
				1048833,
				67109120,
				67109121,
				68157696,
				68157697
			};
			int[] pc2bytes24 = new int[16]
			{
				0,
				8,
				2048,
				2056,
				16777216,
				16777224,
				16779264,
				16779272,
				0,
				8,
				2048,
				2056,
				16777216,
				16777224,
				16779264,
				16779272
			};
			int[] pc2bytes23 = new int[16]
			{
				0,
				2097152,
				134217728,
				136314880,
				8192,
				2105344,
				134225920,
				136323072,
				131072,
				2228224,
				134348800,
				136445952,
				139264,
				2236416,
				134356992,
				136454144
			};
			int[] pc2bytes22 = new int[16]
			{
				0,
				262144,
				16,
				262160,
				0,
				262144,
				16,
				262160,
				4096,
				266240,
				4112,
				266256,
				4096,
				266240,
				4112,
				266256
			};
			int[] pc2bytes21 = new int[16]
			{
				0,
				1024,
				32,
				1056,
				0,
				1024,
				32,
				1056,
				33554432,
				33555456,
				33554464,
				33555488,
				33554432,
				33555456,
				33554464,
				33555488
			};
			int[] pc2bytes20 = new int[16]
			{
				0,
				268435456,
				524288,
				268959744,
				2,
				268435458,
				524290,
				268959746,
				0,
				268435456,
				524288,
				268959744,
				2,
				268435458,
				524290,
				268959746
			};
			int[] pc2bytes19 = new int[16]
			{
				0,
				65536,
				2048,
				67584,
				536870912,
				536936448,
				536872960,
				536938496,
				131072,
				196608,
				133120,
				198656,
				537001984,
				537067520,
				537004032,
				537069568
			};
			int[] pc2bytes18 = new int[16]
			{
				0,
				262144,
				0,
				262144,
				2,
				262146,
				2,
				262146,
				33554432,
				33816576,
				33554432,
				33816576,
				33554434,
				33816578,
				33554434,
				33816578
			};
			int[] pc2bytes17 = new int[16]
			{
				0,
				268435456,
				8,
				268435464,
				0,
				268435456,
				8,
				268435464,
				1024,
				268436480,
				1032,
				268436488,
				1024,
				268436480,
				1032,
				268436488
			};
			int[] pc2bytes16 = new int[16]
			{
				0,
				32,
				0,
				32,
				1048576,
				1048608,
				1048576,
				1048608,
				8192,
				8224,
				8192,
				8224,
				1056768,
				1056800,
				1056768,
				1056800
			};
			int[] pc2bytes15 = new int[16]
			{
				0,
				16777216,
				512,
				16777728,
				2097152,
				18874368,
				2097664,
				18874880,
				67108864,
				83886080,
				67109376,
				83886592,
				69206016,
				85983232,
				69206528,
				85983744
			};
			int[] pc2bytes14 = new int[16]
			{
				0,
				4096,
				134217728,
				134221824,
				524288,
				528384,
				134742016,
				134746112,
				16,
				4112,
				134217744,
				134221840,
				524304,
				528400,
				134742032,
				134746128
			};
			int[] pc2bytes13 = new int[16]
			{
				0,
				4,
				256,
				260,
				0,
				4,
				256,
				260,
				1,
				5,
				257,
				261,
				1,
				5,
				257,
				261
			};
			int iterations = (beinetkey.Length < 24) ? 1 : 3;
			int[] keys = new int[32 * iterations];
			int[] shifts = new int[16]
			{
				0,
				0,
				1,
				1,
				1,
				1,
				1,
				1,
				0,
				1,
				1,
				1,
				1,
				1,
				1,
				0
			};
			int l = 0;
			int i = 0;
			for (int k = 0; k < iterations; k++)
			{
				int[] tmp = new int[8];
				int pos = 24;
				int iTmp = 0;
				while (l < beinetkey.Length && iTmp < tmp.Length)
				{
					if (pos < 0)
					{
						pos = 24;
					}
					tmp[iTmp++] = (int)((uint)beinetkey[l++] << pos);
					pos -= 8;
				}
				int left10 = tmp[0] | tmp[1] | tmp[2] | tmp[3];
				int right10 = tmp[4] | tmp[5] | tmp[6] | tmp[7];
				int temp9 = (RM(left10, 4) ^ right10) & 0xF0F0F0F;
				right10 ^= temp9;
				left10 ^= temp9 << 4;
				temp9 = ((RM(right10, -16) ^ left10) & 0xFFFF);
				left10 ^= temp9;
				right10 ^= temp9 << 16;
				temp9 = ((RM(left10, 2) ^ right10) & 0x33333333);
				right10 ^= temp9;
				left10 ^= temp9 << 2;
				temp9 = ((RM(right10, -16) ^ left10) & 0xFFFF);
				left10 ^= temp9;
				right10 ^= temp9 << 16;
				temp9 = ((RM(left10, 1) ^ right10) & 0x55555555);
				right10 ^= temp9;
				left10 ^= temp9 << 1;
				temp9 = ((RM(right10, 8) ^ left10) & 0xFF00FF);
				left10 ^= temp9;
				right10 ^= temp9 << 8;
				temp9 = ((RM(left10, 1) ^ right10) & 0x55555555);
				right10 ^= temp9;
				left10 ^= temp9 << 1;
				temp9 = (left10 << 8 | (RM(right10, 20) & 0xF0));
				left10 = (right10 << 24 | (right10 << 8 & 0xFF0000) | (RM(right10, 8) & 0xFF00) | (RM(right10, 24) & 0xF0));
				right10 = temp9;
				for (int j = 0; j < shifts.Length; j++)
				{
					if (shifts[j] == 1)
					{
						left10 = (left10 << 2 | RM(left10, 26));
						right10 = (right10 << 2 | RM(right10, 26));
					}
					else
					{
						left10 = (left10 << 1 | RM(left10, 27));
						right10 = (right10 << 1 | RM(right10, 27));
					}
					left10 &= -15;
					right10 &= -15;
					int lefttemp = pc2bytes26[RM(left10, 28)] | pc2bytes25[RM(left10, 24) & 0xF] | pc2bytes24[RM(left10, 20) & 0xF] | pc2bytes23[RM(left10, 16) & 0xF] | pc2bytes22[RM(left10, 12) & 0xF] | pc2bytes21[RM(left10, 8) & 0xF] | pc2bytes20[RM(left10, 4) & 0xF];
					int righttemp = pc2bytes19[RM(right10, 28)] | pc2bytes18[RM(right10, 24) & 0xF] | pc2bytes17[RM(right10, 20) & 0xF] | pc2bytes16[RM(right10, 16) & 0xF] | pc2bytes15[RM(right10, 12) & 0xF] | pc2bytes14[RM(right10, 8) & 0xF] | pc2bytes13[RM(right10, 4) & 0xF];
					temp9 = ((RM(righttemp, 16) ^ lefttemp) & 0xFFFF);
					keys[i++] = (lefttemp ^ temp9);
					keys[i++] = (righttemp ^ temp9 << 16);
				}
			}
			return keys;
		}
	}
}
