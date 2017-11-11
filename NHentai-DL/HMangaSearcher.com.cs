using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace NHentaiDL{
	public class HMangaSearcherCom:ISitePlugin{
		public string Name{get{return "HMangaSearcher.com";}}
		public string ExampleURL{get{return "hmangasearcher.com/m/Gallery+Name";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("hmangasearcher\\.com/(m|c|f)/[^/]+(/[0-9]+|)");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("www.", StringComparison.InvariantCulture))
				GalleryUrl = "www."+GalleryUrl;
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://"+GalleryUrl;
			Match galleryMatch = Regex.Match(GalleryUrl, "hmangasearcher\\.com/(m|c|f)/([^/]+)(/[0-9]+|)");
			string Title = galleryMatch.Groups[2].Value;
			int Page = 1;
			int Chapter;
			string Body;
			switch(galleryMatch.Groups[1].Value){
				case "m":
					Body = wc.DownloadString(GalleryUrl);
					int i = Body.IndexOf("<h4>You may also like:</h4>", StringComparison.InvariantCulture);
					if(i>=0) Body = Body.Substring(0,i);
					foreach(Match cmatch in Regex.Matches(Body, "/(c|f)/([^/]+)/([0-9]+)")){
						Body = wc.DownloadString(string.Format("http://www.hmangasearcher.com/f/{0}/{1}",cmatch.Groups[2].Value,cmatch.Groups[3].Value));
						if(!int.TryParse(cmatch.Groups[3].Value, out Chapter)) Chapter = 0;
						foreach(Match match in Regex.Matches(Body, "http://h[^\\.]+\\.hmangasearcher\\.com/manga/[^/]+/[^/]+/[^/]+/[^\\.]+\\.(jpg|png|bmp|jpeg|gif)")){
							Images.Add(new ImageInfo{ Filename=string.Format("c{0:0000}_{1:0000}.{2}",Chapter,Page,match.Groups[1].Value), URL=new Uri(match.Value) });
							Page++;
						}
					}
					break;
				case "c":
				case "f":
					Body = wc.DownloadString(string.Format("http://www.hmangasearcher.com/f/{0}/{1}", galleryMatch.Groups[2].Value, galleryMatch.Groups[3].Value));
					if(!int.TryParse(galleryMatch.Groups[3].Value, out Chapter)) Chapter = 0;					
					foreach(Match match in Regex.Matches(Body, "http://h[^\\.]+\\.hmangasearcher\\.com/manga/[^/]+/[^/]+/[^/]+/[^\\.]+\\.(jpg|png|bmp|jpeg|gif)")){
						Images.Add(new ImageInfo{ Filename=string.Format("c{0:0000}_{1:0000}.{2}",Chapter,Page,match.Groups[1].Value), URL=new Uri(match.Value) });
						Page++;
					}
					break;
			}
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}
