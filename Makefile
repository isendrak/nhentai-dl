PACKAGE_NAME    = nhentai-dl
PACKAGE_VERSION = 0.1.0.0
CONFIGURATION   = Release

MSBUILD         = xbuild
MSBUILDFLAGS    = /p:Configuration=$(CONFIGURATION)
NSIS            = makensis
RMDIR           = rm -rfv
MKDIR           = mkdir -p
RSYNC           = rsync -va
CP              = cp -v
MV              = mv -v
PREFIX          = /usr/local
INSTALL         = install

.PHONY: all dist/deb dist/bin dist/src dist/nsis dist clean install

all:
	$(MSBUILD) $(MSBUILDFLAGS) NHentai-DL.sln

clean:
	$(MSBUILD) $(MSBUILDFLAGS) /t:clean
	$(RMDIR) bin obj dist

install: all
	$(INSTALL) -d -o root -g root $(DESTDIR)/$(PREFIX)/share/nhentai-dl
	$(INSTALL) -o root -g root bin/$(CONFIGURATION)/* $(DESTDIR)/$(PREFIX)/share/nhentai-dl/
	$(RSYNC) -og --chown=root:root debian/usr/* $(DESTDIR)/$(PREFIX)/

dist/deb: all
	$(MKDIR) dist
	$(RMDIR) "$(PACKAGE_NAME)_$(PACKAGE_VERSION)" "dist/$(PACKAGE_NAME)_$(PACKAGE_VERSION).deb"
	$(MKDIR) "$(PACKAGE_NAME)_$(PACKAGE_VERSION)"
	$(RSYNC) debian/* "$(PACKAGE_NAME)_$(PACKAGE_VERSION)/"
	$(RSYNC) bin/$(CONFIGURATION)/* "$(PACKAGE_NAME)_$(PACKAGE_VERSION)/usr/share/$(PACKAGE_NAME)/"
	md5sum `find "$(PACKAGE_NAME)_$(PACKAGE_VERSION)" -type f | grep -vE "^$(PACKAGE_NAME)_$(PACKAGE_VERSION)/DEBIAN"` | \
	    sed "s|^\([0-9a-fA-F]\{1,\}\)[ \t]\{1,\}$(PACKAGE_NAME)_$(PACKAGE_VERSION)\(/.*\)$$|\1  \2|" > \
	    "$(PACKAGE_NAME)_$(PACKAGE_VERSION)/DEBIAN/md5sums"
	fakeroot dpkg-deb --build "$(PACKAGE_NAME)_$(PACKAGE_VERSION)"
	$(MV) "$(PACKAGE_NAME)_$(PACKAGE_VERSION).deb" dist/
	$(RMDIR) "$(PACKAGE_NAME)_$(PACKAGE_VERSION)"

dist/bin: all
	$(MKDIR) dist
	zip -v9r "dist/$(PACKAGE_NAME)_$(PACKAGE_VERSION)-$(CONFIGURATION).zip" bin/$(CONFIGURATION)/*

dist/src:
	$(MKDIR) dist
	zip -v9r -xdist/\* -x\*/bin\* -xbin/\* -x\*/obj/\* -xobj/\* -x\*.userprefs "dist/$(PACKAGE_NAME)_$(PACKAGE_VERSION)-Source.zip" *

dist/nsis:
	$(MKDIR) dist
	$(NSIS) NHentai-DL.nsi

dist: dist/bin dist/src
