using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL {
	public class AsmHentaiCom : ISitePlugin {
		public string Name { get { return "AsmHentai.com"; } }
		public string ExampleURL { get { return "asmhentai.com/g/123456"; } }
		public Regex URLRegex { get { return _URLRegex; } }
		private Regex _URLRegex = new Regex("asmhentai\\.com/(g|gallery)/([0-9]+)");
		public GalleryInfo GetGallery(string GalleryUrl) {
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(Settings.Proxy != null) wc.Proxy = new WebProxy(Settings.Proxy);
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			GalleryUrl = string.Format("https://asmhentai.com/g/{0}/", URLRegex.Match(GalleryUrl).Groups[2].Value);
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<h1>(.+)</h1>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "//images\\.asmhentai\\.com/([0-9]+)/([0-9]+)/([0-9]+)t\\.(jpg|jpeg|bmp|png|gif)")) {
				if(match.Groups.Count != 5) continue;
				Images.Add(new ImageInfo { Filename = string.Format("{0:0000}.{1}", Page, match.Groups[4].Value), URL = new Uri(string.Format("https://images.asmhentai.com/{0}/{1}/{2}.{3}", match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value)) });
				Page++;
			}
			return new GalleryInfo { Name = Title, Images = Images };
		}
	}
}
