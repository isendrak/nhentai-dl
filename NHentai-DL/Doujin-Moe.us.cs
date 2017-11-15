using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class DoujinMoeUs:ISitePlugin{
		public string Name{get{return "Doujin-Moe.us";}}
		public string ExampleURL{get{return "doujin-moe.us/123abc";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("(?:doujin-moe\\.us|doujins\\.com)/[a-zA-Z0-9_-]+");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(Settings.Proxy != null) wc.Proxy = new WebProxy(Settings.Proxy);
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://"+GalleryUrl;
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<title>(?:Doujin-moe|Doujins|Doujins\\.com)[ \t]*-[ \t]*([^<]+)</title>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "file=\"(http[s]{0,1})://(static\\.doujin-moe\\.us|static[0-9]*\\.doujins\\.com)/r-([^\\.]+)\\.(jpg|png|bmp|jpeg|gif)\\?st=([^&]+)&e=([0-9]+)\"")){
				if(match.Groups.Count != 7) continue;
				Images.Add(new ImageInfo{ Filename = string.Format("{0:0000}.{1}", Page, match.Groups[3].Value), URL = new Uri(string.Format("{0}://{1}/r-{2}.{3}?st={4}&e={5}", match.Groups[1], Regex.Replace(match.Groups[2].Value,"static[0-9]+\\.","static."), match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value, match.Groups[6])) });
				Page++;
			}
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}
