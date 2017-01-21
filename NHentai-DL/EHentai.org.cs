using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL{
	public class EHentaiOrg:ISitePlugin{
		public string Name{get{return "EHentai.org";}}
		public string ExampleURL{get{return "g.e-hentai.org/g/12345/1a2b3c4d5e/";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("g.e-hentai.org/g/[0-9]+/[0-9a-fA-F]+");
		public GalleryInfo GetGallery(string GalleryUrl){
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://"+GalleryUrl;
			if(!GalleryUrl.EndsWith("/", StringComparison.InvariantCulture))
				GalleryUrl += "/";
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<h1 id=\"gn\">([^<]+)</h1>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, @"http://g\.e-hentai\.org/s/[0-9a-fA-F]+/[0-9]+-[0-9]+")){
				string Body2 = wc.DownloadString(match.Value);
				Match match2 = Regex.Match(Body2 , @"(http://[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}:{0,1}[0-9]{0,5}/h/[0-9a-fA-F]+-[0-9]+-[0-9]+-[0-9]+-[a-zA-Z]+/keystamp=[0-9]+-[0-9a-fA-F]+/.+\.)([a-zA-Z]+)");
				Images.Add(new ImageInfo{Filename = string.Format("{0:0000}.{1}", Page, match2.Groups[1].Value), URL = new Uri(match.Value)});
				Page++;
			}
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}
