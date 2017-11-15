using System;
using System.IO;
using System.Collections.Generic;

namespace NHentaiDL {
	internal static class IniFile {
		public static Dictionary<string,Dictionary<string,string>> Load(string filename) {
			FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			Dictionary<string, Dictionary<string, string>> sections = Load(stream);
			stream.Close();
			return sections;
		}
		public static Dictionary<string, Dictionary<string, string>> Load(Stream stream) {
			TextReader reader = new StreamReader(stream);
			Dictionary<string, Dictionary<string, string>> sections = Load(reader);
			reader.Close();
			return sections;
		}
		public static Dictionary<string, Dictionary<string, string>> Load(TextReader reader) {
			string line;
			string section_name = "";
			Dictionary<string, string> section = new Dictionary<string, string>();
			Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();
			while((line = reader.ReadLine()) != null) {
				line = line.Trim();
				if(line.IndexOf(';') >= 0) line = line.Substring(0, line.IndexOf(';'));
				if(line.Equals(string.Empty)) continue;
				if(line.StartsWith("[", StringComparison.InvariantCulture) && line.EndsWith("]", StringComparison.InvariantCulture)) {
					sections[section_name] = section;
					section_name = line.Substring(1, line.Length - 2);
					section = new Dictionary<string, string>();
					continue;
				}
				string[] entry = line.Split(new char[] { '=' }, 2);
				section[entry[0]] = entry.Length == 1 ? "" : entry[1];
			}
			return sections;
		}
	}
}
