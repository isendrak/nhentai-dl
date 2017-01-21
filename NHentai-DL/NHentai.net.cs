using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class NHentaiNet:ISitePlugin{
		public string Name{get{return "NHentai.net";}}
		public string ExampleURL{get{return "nhentai.net/g/123456";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("nhentai\\.net/g/[0-9]+");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(!GalleryUrl.ToLower().StartsWith("https://", StringComparison.InvariantCulture))
				GalleryUrl = "https://"+GalleryUrl;
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<h1>([^<]+)</h1>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "[ \\t]src=\"//t.nhentai.net/galleries/([0-9]+)/([0-9]+)t\\.(jpg|png|bmp|jpeg|gif)")){
				if(match.Groups.Count != 4) continue;
				Images.Add(new ImageInfo{ Filename=string.Format("{0:0000}.{1}",Page,match.Groups[3].Value), URL=new Uri(string.Format("https://i.nhentai.net/galleries/{0}/{1}.{2}",match.Groups[1].Value,match.Groups[2].Value,match.Groups[3].Value)) });
				Page++;
			}
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}
