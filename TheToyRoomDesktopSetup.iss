#define MyAppName "The Toy Room Desktop"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Personal"
#define MyAppExeName "TheToyRoomDesktop.exe"

[Setup]
AppId={{C7D8E9F1-4A5B-6C7D-8E9F-0A1B2C3D4E5F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
; Use {autopf} which will default to Program Files on 64-bit systems
; Falls back to user directory if no admin privileges
DefaultDirName={autopf}\TheToyRoomDesktop
DefaultGroupName={#MyAppName}
; Allow user to change installation directory
DisableDirPage=no
DisableProgramGroupPage=yes
UsePreviousAppDir=yes
OutputDir=.
OutputBaseFilename=TheToyRoomDesktopSetup_v{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
; Request admin privileges to install to Program Files
; Change to 'lowest' if you want user-level installation
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesInstallIn64BitMode=x64
MinVersion=10.0
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\net8.0-windows\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
