# Legacy VB6 Source Audit

## Project type

The uploaded source is a Visual Basic 6 desktop application.

Primary project file:

- `acgm.vbp`

Related project/workspace files:

- `acgm.vbg`
- `acgm.vbw`
- `acgm.PDM`
- `ACGMDecal.vbp`
- `ACGMDecal.vbw`

## External VB6 dependencies observed

From `acgm.vbp`:

- `MSWINSCK.OCX` - Microsoft Winsock Control 6.0
- `MSCOMCTL.OCX` - Microsoft Windows Common Controls
- `COMDLG32.OCX` - Common Dialog Control
- `MSFLXGRD.OCX` - Microsoft FlexGrid Control

The important HTTPS blocker is `MSWINSCK.OCX`. The old client manually constructs HTTP and connects to port 80.

## Forms

- `frmMain.frm` - main application window and most application logic
- `frmAddVassal.frm` - add vassal dialog
- `frmFindChar.frm` - character search dialog
- `frmPlayerSetup.frm` - player/server login setup
- `frmNewServerSetup.frm` - initialize/change server setup
- `frmLogin.frm` - login dialog
- `frmAbout.frm` - about dialog
- `frmChangePassword.frm` - password change dialog
- `frmSecLevel.frm` - security level dialog
- `frmOptions.frm` - options dialog

Associated binary form/resource files:

- `frmMain.frx`
- `frmPlayerSetup.frx`
- `frmNewServerSetup.frx`
- `frmOptions.frx`
- `frmAbout.frx`

## Classes

- `HTTPRequest.cls` - legacy HTTP transport using Winsock
- `Character.cls` - character model
- `Characters.cls` - character collection
- `PlayerInfo.cls` - player profile/settings model
- `Players.cls` - player collection
- `ServerInfo.cls` - server profile/settings model
- `Servers.cls` - server collection
- `Skill.cls` - skill model
- `Skills.cls` - skill collection
- `ACGMDecal.cls` - Decal/plugin integration path

## Modules

- `constants.bas` - constants, message ids, response codes, helper functions, password obfuscation helper

## Data/config/assets

- `ACGM.xml`
- `ACGM.xml.bak`
- `acgm.ini`
- `client.reg`
- `bugs.txt`
- `acgm.ico`
- `guy.bmp`
- `muleico.jpg`
- `pkico.jpg`
- `rescueico.jpg`

## Main modernization approach

The original application mixes UI, state, request construction, response processing, and tree manipulation heavily in `frmMain.frm`. The modern rewrite should split this into:

- Protocol layer
- Models
- Persistence/settings layer
- UI forms
- Tree/character services
- Optional Decal compatibility layer later
