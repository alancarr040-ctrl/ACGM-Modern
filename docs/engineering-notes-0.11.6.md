# Engineering Notes - ACGM Modern Client 0.11.6

## Package

ACGM Modern Client 0.11.6 - Administrator Functions Restoration

## Philosophy

Recreation first. Modernization underneath.

## Scope

Version 0.11.6 builds on the verified 0.11.5 release and restores the Administrator -> Administrator Functions entry so it opens a working legacy-style administrator dialog instead of a placeholder.

## Preserved Behavior

No parser or protocol behavior was changed in this release:

- Legacy tree `!;` splitting remains unchanged.
- Tree field mapping remains unchanged.
- Character detail parsing remains unchanged.
- Awards final-field parsing remains unchanged.
- Save protocol and msgid values remain unchanged.

## Administrator Dialog

The restored Administrator Functions dialog lists the currently restored administrative-adjacent workflows and pending server-side functions.

Restored entries use existing verified client workflows:

- Refresh Tree
- Find Character
- Save Current Character
- Edit Character Flags
- Awards
- Rescue Squad List
- Portal List
- Trade Skills List

Pending server-side administration remains disabled until the exact VB6 behavior and CGI protocol are confirmed.

## Safety

This package intentionally avoids inventing server-side administrator operations. Unknown legacy actions display a clear message rather than sending unverified protocol requests.

### 0.11.6a correction

The legacy VB6 Administrator menu contains four direct actions: Backup Database, Change Character's Security Level, Reset Character's Password, and Server Setup. 0.11.6a corrects the modern menu to use those direct entries instead of a generic Administrator Functions dialog. The actions remain non-destructive placeholders until the legacy CGI protocol is confirmed.
