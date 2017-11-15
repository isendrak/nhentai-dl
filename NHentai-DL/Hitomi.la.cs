using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class HitomiLa:ISitePlugin{
		public string Name{get{return "Hitomi.la";}}
		public string ExampleURL{get{return "hitomi.la/galleries/123456.html";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("hitomi\\.la/galleries/[0-9]+\\.html");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(Settings.Proxy != null) wc.Proxy = new WebProxy(Settings.Proxy);
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://"+GalleryUrl;
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<h1><a href=\"/reader/[0-9]+.html\">(.+)</a></h1>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "//tn.hitomi.la/smalltn/([0-9]+)/(.+)\\.(jpg|png|bmp|jpeg|gif)\\.(jpg|png|bmp|jpeg|gif)")){
				if(match.Groups.Count != 5) continue;
				Images.Add(new ImageInfo{Filename = string.Format("{0:0000}.{1}", Page, match.Groups[4].Value), URL = new Uri(string.Format("http://{0}.hitomi.la/galleries/{1}/{2}.{3}", (char)(97+(Page%2)), match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value))});
				Page++;
			}
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}
