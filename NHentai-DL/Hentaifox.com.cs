using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class HentaifoxCom:ISitePlugin{
		public string Name{get{return "Hentaifox.com";}}
		public string ExampleURL{get{return "hentaifox.com/gallery/123456";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("hentaifox\\.com/gallery/[0-9]+");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(Settings.Proxy != null) wc.Proxy = new WebProxy(Settings.Proxy);
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://"+GalleryUrl;
			if(!GalleryUrl.EndsWith("/", StringComparison.InvariantCulture))
			   GalleryUrl += "/";
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<h1>([^<]+)</h1>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "<img class=\"lazy no_image\" data-src=\"//i([0-9]+)\\.hentaifox\\.com/([0-9]+)/([0-9]+)/([0-9]+)t\\.(jpg|png|bmp|jpeg|gif)\"")){
				if(match.Groups.Count != 6) continue;
				Images.Add(new ImageInfo{ Filename = string.Format("{0:0000}.{1}", Page, match.Groups[5].Value), URL = new Uri(string.Format("http://i{0}.hentaifox.com/{1}/{2}/{3}.{4}", match.Groups[1], match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value)) });
				Page++;
			}
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}
