# ACGM Modern Client 0.11.10 - Security-Level UI Enforcement

0.11.10 restores two legacy security behaviors from the VB6 client.

## Administrator menu visibility

VB6 `DisplayLoginInfo()` splits the MSG_LOGIN payload on `|`, reads `txtRetData(3)` into `iCurSecurityLevel`, and only displays `mnuAdministrator` when that value is `3`. The modern client now performs the same check after initial login and after File -> Login account switching.

## Legacy result 972

When the server returns legacy result `972` during character save, the modern client now shows the VB6-style message:

`There was an error updating the character info.  You attempted to edit a field that only admins are allowed to edit.`

The raw exception is still written to the diagnostic log, but the user sees the legacy-friendly dialog.

## Preserved behavior

No parser, tree, awards, save serialization, admin action, or server setup protocol behavior was changed.
