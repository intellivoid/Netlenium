; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Netlenium"
#define MyAppVersion "1.0.0.2"
#define MyAppPublisher "Intellivoid"
#define MyAppURL "https://netlenium.intellivoid.info/"
#define MyAppExeName "netlenium.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{3EA0C823-1A26-4C6D-9A66-E4448144559C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=Build\Windows Setup
OutputBaseFilename=netlenium_v1.0.0.2_setup_x86
SetupIconFile=Graphics\Icons\netlenium_package_cat.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; OnlyBelowVersion: 0,6.1

[Files]
Source: "Build\Netlenium\windows_release_x86\netlenium.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\getAttribute.js"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\isDisplayed.js"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\Mono.Options.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\Netlenium.Driver.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\Netlenium.Logging.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\Netlenium.so"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\Netlenium.WebAPI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\webdriver.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\WebResources\favicon.ico"; DestDir: "{app}\WebResources"; Flags: ignoreversion
Source: "Build\Netlenium\windows_release_x86\extensions\*"; DestDir: "{app}\extensions"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\netlenium.exe"; Parameters: "--update --driver-logging-level 3"; Flags: waituntilterminated; StatusMsg: "Installing Drivers"
Filename: "{app}\{#MyAppExeName}"; Flags: nowait postinstall skipifsilent unchecked; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"

[UninstallRun]
Filename: "{app}\netlenium.exe"; Parameters: "--clear-cache"; Flags: waituntilterminated;

[UninstallDelete]
Type: filesandordirs; Name: "{userappdata}\Netlenium"

[Dirs]
Name: "{app}\WebResources"
Name: "{app}\extensions"
