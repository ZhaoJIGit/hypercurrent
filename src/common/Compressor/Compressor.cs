using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Yahoo.Yui.Compressor;

namespace JieNor.Megi.Common.Compressor
{
	public class Compressor
	{
		public static void Compress(string path)
		{
			string parttenStr = ConfigurationManager.AppSettings["CompressFileExtension"];
			if (!string.IsNullOrEmpty(parttenStr))
			{
				string[] parttens = parttenStr.Split(',');
				IEnumerable<string> files = from file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
				where parttens.Contains(file.Substring(file.LastIndexOf(".") + 1))
				select file;
				foreach (string item in files)
				{
					FileInfo finfo = new FileInfo(item);
					string strContent3 = File.ReadAllText(item, Encoding.UTF8);
					if (!string.IsNullOrEmpty(strContent3))
					{
						if (finfo.Extension.ToLower() == ".js")
						{
							File.SetAttributes(item, FileAttributes.Normal);
							JavaScriptCompressor js = new JavaScriptCompressor();
							js.CompressionType = CompressionType.Standard;
							js.Encoding = Encoding.UTF8;
							js.IgnoreEval = false;
							js.ThreadCulture = CultureInfo.CurrentCulture;
							js.ObfuscateJavascript = true;
							strContent3 = js.Compress(strContent3);
							File.WriteAllText(item, strContent3, Encoding.UTF8);
						}
						else if (finfo.Extension.ToLower() == ".css")
						{
							CssCompressor css = new CssCompressor();
							strContent3 = css.Compress(strContent3);
							File.WriteAllText(item, strContent3, Encoding.UTF8);
						}
					}
				}
			}
		}
	}
}
