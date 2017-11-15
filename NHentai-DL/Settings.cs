using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace NHentaiDL {
	public static class Settings {
		public static string UserAgent { get; set; }
		public static int MaxRetries { get; set; }
		public static Uri Proxy { get; set; }
		static Settings() {
			UserAgent = string.Format("NHentai-DL/{0}", Assembly.GetEntryAssembly().GetName().Version);
			MaxRetries = 5;
			Proxy = null;

			string[] configfiles = new string[]{
				Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "nhentai-dl.conf"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "nhentai-dl.conf"),
				Path.Combine(Environment.CurrentDirectory, "nhentai-dl.conf")
			};

			foreach(string filename in configfiles) {
				if(!File.Exists(filename)) continue;
				Dictionary<string, Dictionary<string, string>> settings = null;
				try { settings = IniFile.Load(filename); }
				catch(AccessViolationException) { continue; }
				catch(IOException) { continue; }

				foreach(string section in settings.Keys) {
					if(section.ToUpper().Equals("HTTP")) {
						foreach(string key in settings[section].Keys) {
							if(key.ToUpper().Equals("USERAGENT")) {
								UserAgent = settings[section][key]
									.Replace("$VERSION$", Assembly.GetEntryAssembly().GetName().Version.ToString());
							}
							else if(key.ToUpper().Equals("PROXY")) {
								if(settings[section][key].Equals(string.Empty)) {
									Proxy = null;
								}
								else {
									try { Proxy = new Uri(settings[section][key]); }
									catch(UriFormatException) { }
								}
							}
						}
					}
					else if(section.ToUpper().Equals("DOWNLOADS")) {
						foreach(string key in settings[section].Keys) {
							if(key.ToUpper().Equals("MAXRETRIES")) {
								int value;
								if(int.TryParse(settings[section][key], out value)) {
									MaxRetries = value;
								}
							}
						}
					}
				}
			}
		}
	}
}
