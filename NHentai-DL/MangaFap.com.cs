using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class MangaFapCom:ISitePlugin{
		public string Name{get{return "MangaFap.com";}}
		public string ExampleURL{get{return "mangafap.com/gallery-name";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("mangafap\\.com/[^/ \\t]+[/]{0,1}");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://"+GalleryUrl;
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "Descripton:([^<]+)").Groups[1].Value.Trim();
			if(string.IsNullOrEmpty(Title))
				Title = Regex.Match(Body, "Read (.+) English Hentai Manga Online</title>").Groups[1].Value.Trim();
			if(string.IsNullOrEmpty(Title))
				Title = Regex.Match(Body, "<title>([^>]+)</title>").Groups[1].Value.Trim();
			string ID = Regex.Match(Body, "download\\.php\\?id=([0-9]+)").Groups[1].Value;
			int Pages = 0;
			int.TryParse(Regex.Match(Body, "Total No of Images in Gallery: ([0-9]+)").Groups[1].Value, out Pages);
			for(int Page = 1; Page <= Pages; Page++){
				Images.Add(new ImageInfo{ Filename=string.Format("{0:0000}.jpg",Page), URL = new Uri(string.Format("http://mangafap.com/images/{0}/{1}.jpg", ID, Page))});
			}
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}
