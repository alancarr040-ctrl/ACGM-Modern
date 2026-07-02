# Engineering Notes - 0.10.0

The requested field list included several modern/historical contact services, but the inspected VB6 `frmMain.frm` Real Life & Contact Info tab only defines five fields: Real Name, City/State/Country, Misc. Real Life Info, Email Address, and ICQ Number.

To preserve the project principle of recreation first, 0.10.0 implements only fields confirmed in the VB6 source. Additional fields should not be introduced unless discovered elsewhere in the legacy source or intentionally added as a post-1.0 modernization.

The implementation uses existing `CharacterDetails` properties already populated by `LegacyCharacterParser` and adds UI controls to `MainForm`. The save workflow remains centralized through `SaveCurrentCharacterAsync`, with protocol field names added to `AcgmHttpClient.UpdateCharacterInfoAsync`.
