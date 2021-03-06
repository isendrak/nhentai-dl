using System;
using System.IO;
using System.Net;

namespace NHentaiDL{
	partial class NHentaiDL{
		private static void DownloadGallery(ISitePlugin Site, string GalleryUrl){
			Console.WriteLine("Fetching gallery info for \"{0}\"...", GalleryUrl);

			GalleryInfo gallery;
			try{
				gallery = Site.GetGallery(GalleryUrl);
			}
			catch(WebException ex){
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("{0}", ex.Message);
				Console.ResetColor();
				return;
			}
			Console.Write("Downloading gallery \"{0}\": ", gallery.Name);
			if(gallery.Images.Count == 0){
				Console.WriteLine("Empty gallery.");
				return;
			}
			int n = 1;
			int CurX = Console.CursorLeft;
			int Retries;
			foreach(ImageInfo image in gallery.Images){
				Retries = 0;
				while(Retries <= Settings.MaxRetries){
					try{
						string DownloadDir = (gallery.Name.Length <= 255 ? gallery.Name : gallery.Name.Substring(0, 255))
							.Replace("/", "-")
							.Replace("\\", "-")
							.Replace(":", "-")
							.Trim()
							.Trim('-');
						string DownloadFilename = Path.Combine(DownloadDir, image.Filename);
						if(!Directory.Exists(DownloadDir))
							Directory.CreateDirectory(DownloadDir);
						HttpWebRequest headrequest = (HttpWebRequest)WebRequest.Create(image.URL);
						headrequest.Method = "HEAD";
						if(image.HttpReferer != null)
							headrequest.Referer = image.HttpReferer.ToString();
						headrequest.UserAgent = Settings.UserAgent;
						if(Settings.Proxy != null) headrequest.Proxy = new WebProxy(Settings.Proxy);
						HttpWebResponse headresponse = (HttpWebResponse)headrequest.GetResponse();
						HttpWebRequest request = (HttpWebRequest)WebRequest.Create(image.URL);
						if(image.HttpReferer != null)
							request.Referer = image.HttpReferer.ToString();
						if(File.Exists(DownloadFilename)){
							if(new FileInfo(DownloadFilename).Length < headresponse.ContentLength){
								request.AddRange("bytes", new FileInfo(DownloadFilename).Length, headresponse.ContentLength);
							}
							else{
								Console.CursorLeft = CurX;
								Console.Write("[{0:###0}/{1:###0}]", n++, gallery.Images.Count);
								break;//continue;
							}
						}
						request.UserAgent = Settings.UserAgent;
						if(Settings.Proxy != null) request.Proxy = new WebProxy(Settings.Proxy);
						HttpWebResponse response = (HttpWebResponse)request.GetResponse();
						Stream istream = response.GetResponseStream();
						Stream ostream = new FileStream(DownloadFilename, FileMode.Append, FileAccess.Write);
						byte[] buffer = new byte[4096];
						int c;
						while((c = istream.Read(buffer, 0, buffer.Length)) > 0)
							ostream.Write(buffer, 0, c);
						ostream.Close();
						response.Close();
						Console.CursorLeft = CurX;
						Console.Write("[{0:###0}/{1:###0}]", n++, gallery.Images.Count);
						break;
					}catch(WebException ex){
						Retries++;
						if(Retries > Settings.MaxRetries){
							Console.CursorLeft = CurX;
							Console.Write("[{0:###0}/{1:###0}] ", n, gallery.Images.Count);
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Write(ex.Message);
							Console.ResetColor();
							Console.WriteLine("");
							return;
						}
					}
				}
			}
			Console.WriteLine("");
		}
	}
}
