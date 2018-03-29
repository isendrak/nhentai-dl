# nhentai-dl v0.1.0.0 #

## Description: ##

nhentai-dl is a rather simple commandline downloader for nhentai.net and several other gallery sites (primarily Hentai/Porn ;)

## Features: ##
* Download entire galleries with a single command
* Add not implemented galleries by simply making a plugin
* Use a http proxy if needed (or wished)

## Currently supported galleries: ##
* NHentai.net - nhentai.net/g/123456
* Doujins.com - doujins.com/123abc
* EHentai.org - g.e-hentai.org/g/12345/1a2b3c4d5e/
* Hentaifox.com - hentaifox.com/gallery/123456
* Hitomi.la - hitomi.la/galleries/123456.html
* MangaFap.com - mangafap.com/gallery-name
* Hentai4manga.com - hentai4manga.com/hentai_manga/gallery-name
* HBrowse.com - hbrowse.com/123456
* Hentai2read.com - hentai2read.com/gallery_name/
* HMangaSearcher.com - hmangasearcher.com/m/Gallery+Name
* Pururin.io - pururin.io/gallery/12345/gallery-name
* AsmHentai.com - asmhentai.com/g/123456

## Plugin Directories: ##
* ### Linux ###
    * $HOME/.config/nhentai-dl/plugins/
    * /usr/share/nhentai-dl/plugins/
    * $PWD/plugins/

* ### Windows ###
    * %APPDATA%\nhentai-dl\plugins\
    * %ALLUSERSPROFILE%\Application Data\nhentai-dl\plugins\
    * %CD%\plugins\

## Configuration: ##
On startup the program searches for a file named "nhentai-dl.conf" in 3 locations, overriding options if they're set in 2 or more files, in this order:

1. %Path to nhentai-dl.exe%\nhentai-dl.conf
2. %Path to user profile directory%\nhentai-dl.conf (e.g. C:\Users\FooBar\nhentai-dl.conf)
3. %Current working directory%\nhentai-dl.conf

For more information about the configuration options see the default file included in this package.

## License: ##
Attribution 4.0 International (CC BY 4.0) - https://creativecommons.org/licenses/by/4.0/

## Requirements: ##
* .NET Framework/Mono >= 3

## ToDo: ##
* Some more options (socks-proxy, etc...)
* Add support for downloading booru-galleries

## Additional Information: ##
Though there's a Makefile, it may not work on every system's configuration, if you wish to "make", you may have to tweak the Makefile to work for you.
