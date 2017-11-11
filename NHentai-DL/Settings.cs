using System;
namespace NHentaiDL {
	public static class Settings {
		public static string UserAgent { get; set; }
		static Settings() {
			UserAgent = "NHentai-DL/0.1";
		}
	}
}
