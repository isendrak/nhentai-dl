using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace NHentaiDL{
	public class ExamplePlugin:ISitePlugin{
		//Name of the Plugin (should/has to be unique)
		public string Name{get{return "ExamplePlugin";}}
		//Example URI (shown in the help message)
		public string ExampleURL{get{return "example-plugin.xxx/galleryid-12345";}}
		//Regular expression for recognizing URIs associated to this plugin
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("example-plugin.xxx/galleryid-[0-9]+");
		//Empty constructor, needs to be "public"
		public ExamplePlugin(){}
		//Get infos about the gallery (title, images)
		public GalleryInfo GetGallery(string GalleryUrl){
			//Example for parsing an exemplary gallery type...
			List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			wc.Headers[HttpRequestHeader.UserAgent] = Settings.UserAgent;
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<h1>([^<]+)</h1>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "http://images.example-plugin.xxx/thumbs/([0-9a-fA-F]+)\\.(jpg|png|bmp|jpeg|gif)")){
				Images.Add(new ImageInfo{ Filename=string.Format("{0:0000}.{1}",Page,match.Groups[2].Value), URL=new Uri(string.Format("http://images.example-plugin.xxx/images/{0}.{1}",match.Groups[1],match.Groups[1])) });
				Page++;
			}
			//Return the "GalleryInfo" here
			return new GalleryInfo{Name = Title, Images = Images };
		}
	}
}

