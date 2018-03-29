using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class DoujinsCom:ISitePlugin{
		public string Name{get{return "Doujins.com";}}
		public string ExampleURL{get{return "doujins.com/gallery/123abc";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("doujins\\.com/gallery/[a-zA-Z0-9_-]+");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(Settings.Proxy != null) wc.Proxy = new WebProxy(Settings.Proxy);
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("https://", StringComparison.InvariantCulture))
				GalleryUrl = "https://"+GalleryUrl;
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<title>([^<]+)</title>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "data-file=\"(https://static.doujins.com/[^\\.]+\\.([a-zA-Z]+)[^\"]*)\"")){
				if(match.Groups.Count != 3) continue;
				Images.Add(new ImageInfo{ Filename = string.Format("{0:0000}.{1}", Page, match.Groups[2].Value), URL = new Uri(WebUtility.HtmlDecode(match.Groups[1].Value)) });
				Page++;
			}
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}
