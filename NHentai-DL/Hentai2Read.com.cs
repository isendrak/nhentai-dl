using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class Hentai2Read:ISitePlugin{
		public string Name{get{return "Hentai2read.com";}}
		public string ExampleURL{get{return "hentai2read.com/gallery_name/";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("hentai2read\\.com/[^/]+/");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(Settings.Proxy != null) wc.Proxy = new WebProxy(Settings.Proxy);
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://" + GalleryUrl;
			if(!GalleryUrl.EndsWith("/", StringComparison.InvariantCulture))
				GalleryUrl += "/";
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<title>(.+) - Read").Groups[1].Value.Trim();
			int Page = 0;
			MatchCollection chapterLinks = Regex.Matches(Body, "http[s]{0,1}://hentai2read\\.com/thumbnails/[^/]+/[^/\"]+");
			for(int Chapter = 0; Chapter < chapterLinks.Count; Chapter++){
				Match chapterLink = chapterLinks[chapterLinks.Count - 1 - Chapter];
				Body = wc.DownloadString(chapterLink.Groups[0].Value);
				foreach(Match pageLink in Regex.Matches(Body, "//hentaicdn.com/hentai/([0-9]+)/([0-9]+)/thumbnails/(.+)tmb\\.(jpg|png|bmp|jpeg|gif)(?:\\?[^\"]+|)")){
					if(pageLink.Groups.Count != 5) continue;
					Images.Add(new ImageInfo { Filename = string.Format("c{0:00000}_{1:00000}.{2}", Chapter, Page, pageLink.Groups[4].Value), URL = new Uri(string.Format("https://hentaicdn.com/hentai/{0}/{1}/{2}.{3}{4}", pageLink.Groups[1].Value, pageLink.Groups[2].Value, pageLink.Groups[3].Value, pageLink.Groups[4].Value, pageLink.Groups[5].Value)) });
					Page++;
				}
				//Chapter--;
				Page = 0;
			}
			return new GalleryInfo { Name = Title, Images = Images };
		}
	}
}
