using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class HBrowse:ISitePlugin{
		public string Name{get{return "HBrowse.com";}}
		public string ExampleURL{get{return "hbrowse.com/123456";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("hbrowse\\.com/(thumbnails/){0,1}[0-9]+");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			GalleryUrl = GalleryUrl.Replace("thumbnails/", "");
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://" + GalleryUrl;
			if(!GalleryUrl.EndsWith("/", StringComparison.InvariantCulture))
				GalleryUrl += "/";
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<title>([^>]+)</title>").Groups[1].Value.Trim();
			int Page = 0;
			int Chapter = 0;
			foreach(Match chapterLink in Regex.Matches(Body, "http://www\\.hbrowse\\.com/thumbnails/[0-9]+/c[0-9]+")){
				Body = wc.DownloadString(chapterLink.Groups[0].Value);
				foreach(Match pageLink in Regex.Matches(Body, "http://www\\.hbrowse\\.com/data/([0-9]+)/c([0-9]+)/zzz/([0-9]+)\\.(jpg|png|bmp|jpeg|gif)")){
					if(pageLink.Groups.Count != 5) continue;
					Images.Add(new ImageInfo { Filename = string.Format("c{0:00000}_{1:00000}.{2}", Chapter, Page, pageLink.Groups[4].Value), URL = new Uri(string.Format("http://www.hbrowse.com/data/{0}/c{1}/{2}.{3}", pageLink.Groups[1].Value, pageLink.Groups[2].Value, pageLink.Groups[3].Value, pageLink.Groups[4].Value)) });
					Page++;
				}
				Chapter++;
				Page = 0;
			}
			return new GalleryInfo { Name = Title, Images = Images };
		}
	}
}
