using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NHentaiDL{
	public interface ISitePlugin{
		//Name of the Plugin (should/has to be unique)
		string Name{ get; }
		//Example URI (shown in the help message)
		string ExampleURL{ get; }
		//Regular expression for recognizing URIs associated to this plugin
		Regex URLRegex{ get; }
		//Get infos about the gallery (title, images)
		GalleryInfo GetGallery(string GalleryUrl);
	}

	public struct GalleryInfo{
		//Name/Title of the gallery
		public string Name{ get; set; }
		//See "ImageInfo"...
		public List<ImageInfo> Images{ get; set; }
	}

	public struct ImageInfo{
		//Local filename, relative to the gallery's download directory
		public string Filename{ get; set; }
		//The image's URI
		public Uri URL{ get; set; }
		public Uri HttpReferer{ get; set; }
	}
}
