#define MyAppName "しごとログ"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Kumiko Arai"
#define MyAppExeName "ShigotoLog.exe"

[Setup]
AppId={{a4f42a98-e5cf-4796-8eb9-2a2bdfce204c}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

DefaultDirName={autopf}\ShigotoLog
DefaultGroupName={#MyAppName}

SetupIconFile=..\Assets\ShigotoLog.ico

OutputDir=Output
OutputBaseFilename=ShigotoLog_Setup_v1.0.0
Compression=lzma2
SolidCompression=yes

SetupArchitecture=x64

UninstallDisplayName={#MyAppName}

[Files]
Source: "C:\DailyReportMemoApp\DailyReportMemoApp\DailyReportMemoApp\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "デスクトップにショートカットを作成"; GroupDescription: "追加のショートカット:"; Flags: unchecked

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{#MyAppName}を起動"; Flags: nowait postinstall skipifsilent