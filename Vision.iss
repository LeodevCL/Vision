[Setup]
ChangesAssociations=yes
AppName="Vision"
AppVersion=1
DefaultDirName="E:\Visual Studio\2020\Vision\Vision\bin\Release"

[Registry]
Root: HKCR; Subkey: ".jfif"; ValueType: string; ValueName: ""; ValueData: "Vision"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "Vision"; ValueType: string; ValueName: ""; ValueData: "Vision Image Viewer"; Flags: uninsdeletekey
Root: HKCR; Subkey: "Vision\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Vision.exe,0"
Root: HKCR; Subkey: "Vision\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Vision.exe"" ""%1"""