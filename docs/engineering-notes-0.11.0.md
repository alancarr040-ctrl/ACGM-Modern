# Engineering Notes - 0.11.0

0.11.0 consolidates several previously planned small releases into one Character Utilities phase.

The utility-list tabs are intentionally implemented as read/display workflows first. The VB6 source clearly identifies the retrieval msgid values and grid layouts, but update behavior for these lists is not implemented unless the server protocol can be confirmed safely.

New code added:

- `Models/UtilityRecords.cs`
- `Protocol/LegacyUtilityListParser.cs`
- Utility retrieval methods in `AcgmHttpClient`
- Utility retrieval methods in `AcgmProtocolService`
- Utility tab builders and refresh handlers in `MainForm`

The UI remains classic WinForms and preserves the recreation-first approach.
