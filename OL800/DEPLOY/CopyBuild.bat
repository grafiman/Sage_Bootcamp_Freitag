DEL "%cd%\FILES\ADDIN\*.*" /Q
DEL "%cd%\FILES\Shared\*.*" /Q
DEL "%cd%\FILES\DBADMIN\*.*" /Q
DEL "%cd%\FILES\METADATA\*.*" /Q
DEL "%cd%\FILES\CAS\*.*" /Q
DEL "%cd%\FILES\CONFIGUPDATE\*.*" /Q

REM XCopy "..\ADDIN\OLAbfSeminarverwaltung71.MDA" "%cd%\FILES\ADDIN\" /K/H/V/C/Q/R
REM XCopy "..\ADDIN\OLAbfSeminarverwaltung71.MDE" "%cd%\FILES\ADDIN\" /K/H/V/C/Q/R

REM "C:\Programme\Microsoft Office\Office15\msaccess.exe" "%cd%\FILES\ADDIN\OLAbfSeminarverwaltung70.MDA" /decompile

XCopy "..\CAS\PSDSeminarverwaltung.OLKey" "%cd%\FILES\CAS\" /K/H/V/C/Q/R

XCopy "..\SKRIPTE\PSDSeminarverwaltung80.upd" "%cd%\FILES\CAS\" /K/H/V/C/Q/R
XCopy "..\CONFIGUPDATE\PSDSeminarverwaltung.configupdate" "%cd%\FILES\CONFIGUPDATE\" /K/H/V/C/Q/R
XCopy "..\METADATA\100096740.Academy.802.metadata" "%cd%\FILES\METADATA\" /K/H/V/C/Q/R
XCopy "..\METADATA\100096740.AcademyReporting.802.metadata" "%cd%\FILES\METADATA\" /K/H/V/C/Q/R

XCopy "..\SKRIPTE\PSDSeminarverwaltung80.upd" "%cd%\FILES\DBADMIN\" /K/H/V/C/Q/R

XCopy "C:\Program Files (x86)\Sage\Sage 100\8.0\Shared\PSDev.OfficeLine.Academy.BusinessLogic.dll" "%cd%\FILES\Shared\" /K/H/V/C/Q/R
XCopy "C:\Program Files (x86)\Sage\Sage 100\8.0\Shared\PSDev.OfficeLine.Academy.DataAccess.dll" "%cd%\FILES\Shared\" /K/H/V/C/Q/R
XCopy "C:\Program Files (x86)\Sage\Sage 100\8.0\Shared\PSDev.OfficeLine.Academy.RealTimeData.dll" "%cd%\FILES\Shared\" /K/H/V/C/Q/R
XCopy "C:\Program Files (x86)\Sage\Sage 100\8.0\Shared\PSDev.OfficeLine.Academy.DCM.dll" "%cd%\FILES\Shared\" /K/H/V/C/Q/R


"C:\Program Files (x86)\Inno Setup 5\compil32.exe" /cc "%cd%\SeminarverwaltungOLClient.iss"

PAUSE
