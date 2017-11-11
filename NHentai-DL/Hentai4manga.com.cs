using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class Hentai4mangaCom:ISitePlugin{
		public string Name{get{return "Hentai4manga.com";}}
		public string ExampleURL{get{return "hentai4manga.com/hentai_manga/gallery-name";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("hentai4manga\\.com/hentai_manga/[^/]+");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://" + GalleryUrl;
			if(!GalleryUrl.EndsWith("/", StringComparison.InvariantCulture))
				GalleryUrl += "/";
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<div id=\"view_title_manga\">([^<]+)</div>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "/imglink/manga-thumb[0-9]+x[0-9]+/([0-9]+)/([0-9]+)/([0-9]+)\\.(jpg|png|bmp|jpeg|gif)")){
				if(match.Groups.Count != 5)
					continue;
				Images.Add(new ImageInfo { Filename = string.Format("{0:0000}.{1}", Page, match.Groups[4].Value), URL = new Uri(string.Format("http://hentai4manga.com/imglink/manga/{0}/{1}/{2}.{3}", match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value)), HttpReferer = new Uri(string.Format("{0}{1}/", GalleryUrl, Page - 1)) });
				Page++;
			}
			return new GalleryInfo { Name = Title, Images = Images };
		}
	}
}
