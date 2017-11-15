using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace NHentaiDL{
	partial class NHentaiDL{
		private static void Usage(){
			Console.WriteLine("usage: {0} [options] [gallery-url, ...]",Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
			Console.WriteLine("Options:");
			Console.WriteLine("\t--help / -h\t-\tShows this message.");
			Console.WriteLine("Supported Galleries:");
			foreach(ISitePlugin Plugin in Plugins){
				Console.WriteLine("\t{0}\t-\t{1}", Plugin.Name, Plugin.ExampleURL);
			}
		}

		public static void Main(string[] args){
			Console.CancelKeyPress += delegate{
				Console.WriteLine("");
				Environment.Exit(0);
			};

			LoadPlugins(Assembly.GetEntryAssembly());
			LoadPlugins(new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nhentai-dl" + Path.DirectorySeparatorChar + "plugins")));
			LoadPlugins(new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "nhentai-dl" + Path.DirectorySeparatorChar + "plugins")));
			LoadPlugins(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "plugins")));

			Console.WriteLine("nhentai-dl v{0}", Assembly.GetEntryAssembly().GetName().Version);

			if(args.Length < 1){
				Usage();
				return;
			}
			else if(args[0].Equals("/?") || args[0].Equals("/h") || args[0].Equals("/help") || args[0].Equals("-?") || args[0].Equals("-h") || args[0].Equals("--help")){
				Usage();
				return;
			}

			foreach(ISitePlugin Site in Plugins){
				foreach(Match match in Site.URLRegex.Matches(string.Join(" ", args))){
					if(match.Groups.Count < 1 || match.Length <= 0) continue;
					DownloadGallery(Site, match.Value);
				}
			}
		}
	}
}
