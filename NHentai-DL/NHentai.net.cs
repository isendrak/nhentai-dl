using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace NHentaiDL{
	public class NHentaiNet:ISitePlugin{
		public string Name{get{return "NHentai.net";}}
		public string ExampleURL{get{return "nhentai.net/g/123456";}}
		public Regex URLRegex{get{return _URLRegex;}}
		private Regex _URLRegex = new Regex("nhentai\\.net/g/[0-9]+");
		
		[DataContract]
		public struct GalleryData{
			[DataContract]
			public struct GalleryTitle{
				[DataMember(Name="japanese")]
				public string Japanese{get;set;}
				[DataMember(Name="pretty")]
				public string Pretty{get;set;}
				[DataMember(Name="english")]
				public string English{get;set;}
			}
			
			[DataContract]
			public struct GalleryImages{
				[DataMember(Name="cover")]
				public GalleryImage Cover{get;set;}
				[DataMember(Name="thumbnail")]
				public GalleryImage Thumbnail{get;set;}
				[DataMember(Name="pages")]
				public List<GalleryImage> Pages{get;set;}
			}
		
			[DataContract]
			public struct GalleryImage{
				[DataMember(Name="h")]
				public int Height{get;set;}
				[DataMember(Name="w")]
				public int Width{get;set;}
				[DataMember(Name="t")]
				public string Type{get;set;}
			}
			
			[DataMember(Name="upload_date")]
			public int UploadDate{get;set;}
			[DataMember(Name="num_favorites")]
			public int Favorites{get;set;}
			[DataMember(Name="title")]
			public GalleryTitle Title{get;set;}
			[DataMember(Name="media_id")]
			public string MediaId{get;set;}
			[DataMember(Name="scanlator")]
			public string Scanlator{get;set;}
			[DataMember(Name="id")]
			public int Id{get;set;}
			[DataMember(Name="images")]
			public GalleryImages Images{get;set;}
		}
		
		public GalleryInfo GetGallery(string GalleryUrl){
			/*List<ImageInfo> Images = new List<ImageInfo>();
			WebClient wc = new WebClient();
			if(!GalleryUrl.ToLower().StartsWith("https://", StringComparison.InvariantCulture))
				GalleryUrl = "https://"+GalleryUrl;
			string Body = wc.DownloadString(GalleryUrl);
			string Title = Regex.Match(Body, "<h1>([^<]+)</h1>").Groups[1].Value;
			int Page = 1;
			foreach(Match match in Regex.Matches(Body, "[ \\t]src=\"http[s]{0,1}://t.nhentai.net/galleries/([0-9]+)/([0-9]+)t\\.(jpg|png|bmp|jpeg|gif)")){
				if(match.Groups.Count != 4) continue;
				Images.Add(new ImageInfo{ Filename=string.Format("{0:0000}.{1}",Page,match.Groups[3].Value), URL=new Uri(string.Format("https://i.nhentai.net/galleries/{0}/{1}.{2}",match.Groups[1].Value,match.Groups[2].Value,match.Groups[3].Value)) });
				Page++;
			}
			return new GalleryInfo{Name = Title, Images = Images };*/
			
			List<ImageInfo> Images = new List<ImageInfo>();
			string galleryId = Regex.Match(GalleryUrl, "nhentai.net/g/([0-9]+)").Groups[1].Value;
			WebClient wc = new WebClient();
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GalleryData));
			GalleryData data = (GalleryData)ser.ReadObject(wc.OpenRead(string.Format("https://nhentai.net/api/gallery/{0}", galleryId)));
			for(int i = 0; i < data.Images.Pages.Count; ++i){
				string ext = "";
				switch(data.Images.Pages[i].Type.ToLower()){
					case "j":
						ext = "jpg";
						break;
					case "g":
						ext = "gif";
						break;
					case "p":
						ext = "png";
						break;
				}
				Images.Add(new ImageInfo{ Filename = string.Format("{0:0000}.{1}", i + 1, ext), URL = new Uri(string.Format("https://i.nhentai.net/galleries/{0}/{1}.{2}", data.MediaId, i + 1, ext)) } );
			}
			return new GalleryInfo{ Name = data.Title.English, Images = Images };
		}
	}
}
