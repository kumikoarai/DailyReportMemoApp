#define MyAppName "しごとログ"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Kumiko Arai"
#define MyAppExeName "ShigotoLog.exe"

[Setup]
AppId={{A8C67D5F-9F83-4E58-9B29-7CA27588F001}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

DefaultDirName={autopf}\ShigotoLog
DefaultGroupName={#MyAppName}

OutputDir=Output
OutputBaseFilename=ShigotoLogSetup
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