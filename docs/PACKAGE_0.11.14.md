# Package

0.11.14

# Title

Legacy Current Player Functions Menu Restoration

# Goal

Restore the Modern Client Current Player Functions menu to match the legacy VB6 ACGM menu structure.

# Scope

The legacy VB6 Current Player Functions menu contains:

- Add Vassal
- Change Patron
- Change Password

The modern client menu should keep:

- Refresh Character Info
- Save Character Info

The modern client should remove dialog-tab navigation entries from this menu:

- Basic Info
- Allegiance Info
- Skills
- Real Life & Contact Info
- Awards

Those sections remain available inside the Character Dialog/tab interface and are not removed from the application.

# Do Not Modify

See:

- AI_PROJECT_SPEC.md
- AI_CERTIFIED_SUBSYSTEMS.md

All certified subsystems remain frozen unless this package explicitly restores legacy menu behavior.

Do not modify:

- HTTPS
- Login
- Tree Parser
- Character Parser
- Tree Icons
- Character Save Serialization
- Character Save Certification Suite
- Awards storage/display behavior
- Administrator Functions
- Server Setup
- Security Level
- Find Character
- Unrelated protocol logic

# Implementation Requirements

- Rebuild the Current Player Functions menu to expose only legacy current-player actions plus Refresh/Save Character Info.
- Add Vassal must call the existing legacy `MSG_ADD_VASSAL` protocol path.
- Change Patron must call the existing legacy `MSG_CHANGE_PATRON` protocol path.
- Change Password must call the legacy `MSG_CHANGE_PASSWORD` protocol path.
- Character Dialog tabs must remain in the dialog only.
- Do not change character-save serialization.
- Do not change tree/icon behavior.

# Certification Requirements

Verify:

1. Current Player Functions menu shows:
   - Refresh Character Info
   - Save Character Info
   - Add Vassal
   - Change Patron
   - Change Password
2. Current Player Functions menu no longer shows:
   - Basic Info
   - Allegiance Info
   - Skills
   - Real Life & Contact Info
   - Awards
3. Character Dialog tabs still exist and work normally.
4. Add Vassal posts successfully and Refresh Tree reflects the result.
5. Change Patron posts successfully and Refresh Tree reflects the result.
6. Change Password posts successfully and the changed password can be used to log in.
7. No regression to character save behavior.
8. No regression to tree icons.

# Documentation

Update:

- README
- CHANGELOG
- RELEASE_NOTES
- LEGACY_PROTOCOL.md
- LEGACY_DISCOVERIES.md
- Engineering Notes

Document:

The Current Player Functions menu has been restored to the legacy VB6 structure.

# Deliverables

- Complete Visual Studio Solution
- Updated Documentation
- Release Notes
- ZIP Package

# Version

ACGM Modern Client 0.11.14 – Legacy Current Player Functions Menu Restoration
