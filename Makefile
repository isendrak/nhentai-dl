PACKAGE_NAME    = nhentai-dl
PACKAGE_VERSION = 0.1.0.0
CONFIGURATION   = Release
ARCH            = all

MSBUILD         = msbuild
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
	$(INSTALL) -d -o root -g root $(DESTDIR)/$(PREFIX)/share/nhentai-dl/plugins
	$(INSTALL) -d -o root -g root $(DESTDIR)/$(PREFIX)/share/man/man1
	$(INSTALL) -o root -g root bin/$(CONFIGURATION)/* $(DESTDIR)/$(PREFIX)/share/nhentai-dl/
	$(INSTALL) -d -o root -g root $(DESTDIR)/$(PREFIX)/bin
	$(INSTALL) -o root -g root nhentai-dl.1 $(DESTDIR)/$(PREFIX)/man/man1
	$(INSTALL) -o root -g root nhentai-dl $(DESTDIR)/$(PREFIX)/bin

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
