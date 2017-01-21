using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace NHentaiDL{
	partial class NHentaiDL{
		private static List<ISitePlugin> Plugins = new List<ISitePlugin>();

		private static void LoadPlugins(Assembly assembly){
			foreach(Type type in assembly.GetTypes()){
				foreach(Type iface in type.GetInterfaces()){
					if(typeof(ISitePlugin).Equals(iface)){
						ConstructorInfo constructor = type.GetConstructor(new Type[0]);
						ISitePlugin plugin = (ISitePlugin)constructor.Invoke(new object[0]);
						if(Plugins.Where(p => p.Name.Equals(plugin.Name)).Count() == 0){
							Plugins.Add(plugin);
						}
						break;
					}
				}
			}
		}

		private static void LoadPlugins(DirectoryInfo plugin_directory){
			if(plugin_directory.Exists){
				foreach(FileInfo plugin_file in plugin_directory.GetFiles()){
					Assembly plugin_assembly;
					try{
						plugin_assembly = Assembly.LoadFile(plugin_file.FullName);
						LoadPlugins(plugin_assembly);
					}
					catch(BadImageFormatException){}
				}
			}
		}
	}
}
