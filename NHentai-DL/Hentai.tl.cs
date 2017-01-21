using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class HentaiTl:ISitePlugin{
		public string Name{get{return "Hentai.tl";}}
		public string ExampleURL{get{return "hentai.tl/manga/gallery-name";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("hentai\\.tl/manga/[^/]+");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://" + GalleryUrl;
			if(!GalleryUrl.EndsWith("/", StringComparison.InvariantCulture))
				GalleryUrl += "/";
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<title>(.+) on Hentai Manga and Hentai Doujin</title>").Groups[1].Value;
			int Pages;
			if(!int.TryParse(Regex.Match(Body, "&nbsp;OF&nbsp;([0-9]+)&nbsp;").Groups[1].Value, out Pages))
				Pages = 0;
			int Page = 1;
			Match imagematch = Regex.Match(Body, "/imglink/manga/([0-9]+/[0-9]+)/[0-9]+\\.(jpg|png|bmp|jpeg|gif)");
			for(Page = 1; Page <= Pages; Page++){
				Images.Add(new ImageInfo { Filename = string.Format("{0:0000}.{1}", Page, imagematch.Groups[2].Value), URL = new Uri(string.Format("http://www.hentai.tl/imglink/manga/{0}/{1:000}.{2}",imagematch.Groups[1].Value,Page,imagematch.Groups[2].Value)), HttpReferer = new Uri(GalleryUrl) });
			}
			/*foreach(Match match in ){
				if(match.Groups.Count != 5)
					continue;
				Images.Add(new ImageInfo { Filename = string.Format("{0:0000}.{1}", Page, match.Groups[4].Value), URL = new Uri(string.Format("http://hentai.tl/imglink/manga/{0}/{1}/{2}.{3}", match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value)), HttpReferer = new Uri(GalleryUrl) });
				Page++;
			}*/
			return new GalleryInfo { Name = Title, Images = Images };
		}
	}
}
