!ifndef APPNAME
	!define APPNAME "NHentai-DL"
!endif
!ifndef VERSION
	!define VERSION "0.1.0.0"
!endif
!ifndef DISTDIR
	!define DISTDIR "dist"
!endif
!ifndef BINDIR
	!define BINDIR "bin\Release"
!endif
!ifndef LICENSE_FILE
	!define LICENSE_FILE "LICENSE.md"
!endif

!include nsDialogs.nsh
!include LogicLib.nsh
!include StrFunc.nsh
${StrStr}
${UnStrStr}
${UnStrRep}

Name "${APPNAME} v${VERSION}"
OutFile "${DISTDIR}\${APPNAME}-${VERSION}-setup.exe"
SetCompressor "bzip2"
RequestExecutionLevel admin

SetOverwrite ifnewer
InstProgressFlags smooth
AutoCloseWindow false
ShowInstDetails show
ShowUninstDetails show
SilentInstall normal
SilentUnInstall normal

Var welcome_dialog
Var welcome_header
Var welcome_text

Var license_dialog
Var license_header
Var license_text

Page custom welcome
Page custom license
;Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

Function welcome
	nsDialogs::Create 1018
	Pop $welcome_dialog
	
	${NSD_CreateLabel} 0 0 100% 20u "${APPNAME} v${VERSION}"
	Pop $welcome_header
	CreateFont $0 "Arial" 18
	SendMessage $welcome_header ${WM_SETFONT} $0 1
	
	${NSD_CreateLabel} 0 20u 100% 100%-20u "This setup installs ${APPNAME} v${VERSION}"
	Pop $welcome_text
	
	nsDialogs::Show
FunctionEnd

Function license
	GetDlgItem $0 $HWNDPARENT 1
	SendMessage $0 ${WM_SETTEXT} 0 "STR:&Accept"
	nsDialogs::Create 1018
	Pop $license_dialog
	
	${NSD_CreateLabel} 0 0 100% 20u "License"
	Pop $license_header
	CreateFont $0 "Arial" 13
	SendMessage $license_header ${WM_SETFONT} $0 1
	
	nsDialogs::CreateControl EDIT "${__NSD_Text_STYLE}|${WS_VSCROLL}|${ES_MULTILINE}|${ES_WANTRETURN}" "${__NSD_Text_EXSTYLE}" 0 20u 100% 100%-20u ""
	Pop $license_text
	
	SetOutPath $TEMP
	File /oname=${APPNAME}-${VERSION}-${LICENSE_FILE} ${LICENSE_FILE}
	FileOpen $1 "$TEMP\${APPNAME}-${VERSION}-${LICENSE_FILE}" r
	loop:
		FileRead $1 $2
		SendMessage $license_text ${EM_REPLACESEL} 0 "STR:$2"
		IfErrors +1 loop
	FileClose $1
	Delete "$TEMP\${APPNAME}-${VERSION}-${LICENSE_FILE}"
	SendMessage $license_text ${EM_SETSEL} 0 0
	
	nsDialogs::Show
FunctionEnd

InstallDir $PROGRAMFILES\${APPNAME}

DirText "Select the directory where you want to install ${APPNAME} v${VERSION}."

Section "${APPNAME}"
	SetOutPath $INSTDIR
	File ${BINDIR}\*
	
	ReadRegStr $0 HKLM "System\CurrentControlSet\Control\Session Manager\Environment" "PATH"
	${If} $0 == ""
		WriteRegStr HKLM "System\CurrentControlSet\Control\Session Manager\Environment" "PATH" "$INSTDIR\"
	${Else}
		${StrStr} $1 "$0" "$INSTDIR\"
		${If} $1 == ""
			WriteRegStr HKLM "System\CurrentControlSet\Control\Session Manager\Environment" "PATH" "$0;$INSTDIR\"
		${EndIf}
	${EndIf}
	
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME} (remove only)"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\Uninstall-${APPNAME}.exe"
	WriteUninstaller "$INSTDIR\Uninstall-${APPNAME}.exe"
SectionEnd

Section "un.${APPNAME}"
	Delete "$INSTDIR\*"
	RmDir "$INSTDIR"
	
	ReadRegStr $0 HKLM "System\CurrentControlSet\Control\Session Manager\Environment" "PATH"
	${UnStrStr} $1 "$0" ";$INSTDIR\"
	${If} $1 != ""
		${UnStrRep} $0 "$0" ";$INSTDIR\" ""
		WriteRegStr HKLM "System\CurrentControlSet\Control\Session Manager\Environment" "PATH" "$0"
	${Else}
		${UnStrStr} $1 "$0" "$INSTDIR\"
		${If} $1 != ""
			${UnStrRep} $0 "$0" "$INSTDIR\" ""
			WriteRegStr HKLM "System\CurrentControlSet\Control\Session Manager\Environment" "PATH" "$0"
		${EndIf}
	${EndIf}
	
	DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\${APPNAME}"
	DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
SectionEnd
