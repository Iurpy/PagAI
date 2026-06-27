#define MyAppName "PagAI"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "PagAI"
#define MyAppExeName "PagAI.UI.exe"

[Setup]
AppId={{B5C9B79C-7E49-4D1E-93C7-0C5E9F911A01}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=Installer
OutputBaseFilename=PagAISetup
SetupIconFile=PagAI.UI\Assets\pagai.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na Área de Trabalho"; GroupDescription: "Opções adicionais:"; Flags: unchecked

[Files]
Source: "PagAI.UI\bin\Release\net10.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\PagAI"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Assets\pagai.ico"
Name: "{autodesktop}\PagAI"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; IconFilename: "{app}\Assets\pagai.ico"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir o PagAI"; Flags: nowait postinstall skipifsilent