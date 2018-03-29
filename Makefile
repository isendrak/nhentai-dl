PACKAGE_NAME    = nhentai-dl
PACKAGE_VERSION = 0.1.0.0
CONFIGURATION   = Release
ARCH            = all

MSBUILD         = msbuild
MSBUILDFLAGS    = /p:Configuration=$(CONFIGURATION)
NSIS            = makensis
RMDIR           = rmdir
RMTREE          = rm -rfv
MKDIR           = mkdir -p
ZIP             = zip -v9r
PREFIX          = /usr/local
CP              = cp
CPTREE          = $(CP) -r

.PHONY: all dist/bin dist/src dist/nsis dist/pacman dist clean install

all:
	$(MSBUILD) $(MSBUILDFLAGS) NHentai-DL.sln

clean:
	$(MSBUILD) $(MSBUILDFLAGS) /t:clean
	$(RMTREE) bin obj dist

install: all
	$(MKDIR) $(DESTDIR)/$(PREFIX)/share/nhentai-dl/plugins
	$(MKDIR) $(DESTDIR)/$(PREFIX)/share/man/man1
	$(CPTREE) bin/$(CONFIGURATION)/* $(DESTDIR)/$(PREFIX)/share/nhentai-dl/
	$(MKDIR) $(DESTDIR)/$(PREFIX)/bin
	$(CP) nhentai-dl.1 $(DESTDIR)/$(PREFIX)/share/man/man1
	sed "s|@PREFIX@|$(PREFIX)|g" < nhentai-dl > $(DESTDIR)/$(PREFIX)/bin/nhentai-dl
	chmod +x $(DESTDIR)/$(PREFIX)/bin/nhentai-dl

dist/bin: all
	$(MKDIR) dist
	$(ZIP) "dist/$(PACKAGE_NAME)_$(PACKAGE_VERSION)-$(CONFIGURATION).zip" bin/$(CONFIGURATION)/*

dist/src:
	$(MKDIR) dist
	$(ZIP) -xdist/\* -x\*/bin\* -xbin/\* -x\*/obj/\* -xobj/\* -x\*.userprefs "dist/$(PACKAGE_NAME)_$(PACKAGE_VERSION)-Source.zip" *

dist/nsis: all
	$(MKDIR) dist
	$(NSIS) NHentai-DL.nsi

dist/pacman:
	$(MKDIR) dist
	$(MKDIR) makepkg.tmp
	$(CP) NHentai-DL.pkgbuild makepkg.tmp/PKGBUILD
	cd makepkg.tmp;PKGDEST="$$PWD/../dist" makepkg
	$(RMTREE) makepkg.tmp

dist: dist/bin dist/src
