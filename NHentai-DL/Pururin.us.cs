using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace NHentaiDL {
	public class PururinUs : ISitePlugin {
		public string Name { get { return "Pururin.us"; } }
		public string ExampleURL { get { return "pururin.us/gallery/12345/gallery-name"; } }
		public Regex URLRegex { get { return _URLRegex; } }
		private Regex _URLRegex = new Regex("pururin.us/(gallery/([0-9]+)|read/[0-9]+/[0-9]+)");
		public GalleryInfo GetGallery(string GalleryUrl) {
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(Settings.Proxy != null) wc.Proxy = new WebProxy(Settings.Proxy);
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			if(!GalleryUrl.ToLower().StartsWith("http://", StringComparison.InvariantCulture))
				GalleryUrl = "http://" + GalleryUrl;
			GalleryUrl = GalleryUrl.Trim().Trim('/');
			Match m = _URLRegex.Match(GalleryUrl);
			if(m.Groups[1].Value.ToLower().IndexOf("read/", StringComparison.InvariantCulture) == -1) {
				GalleryUrl = "http://pururin.us/read/" + m.Groups[2].Value + "/01";
			}
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<div class=\"title\">([^<]+)</div>").Groups[1].Value.Trim();
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "http:\\\\/\\\\/pururin\\.us\\\\/assets\\\\/images\\\\/data\\\\/([0-9]+)\\\\/([0-9]+)\\.([a-zA-Z0-9]+)")) {
				Images.Add(new ImageInfo { Filename = string.Format("{0:0000}.{1}", Page, match.Groups[3].Value), URL = new Uri(string.Format("http://pururin.us/assets/images/data/{0}/{1}.{2}", match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value)) });
				++Page;
			}
			return new GalleryInfo { Name = Title, Images = Images };
		}
	}
}
