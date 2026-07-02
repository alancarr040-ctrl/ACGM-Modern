# Engineering Notes - 0.11.14

## Legacy Current Player Functions Menu Restoration

The legacy VB6 Current Player Functions menu was verified from `frmMain.frm` as:

- Add Vassal
- Change Patron
- Change Password

The modern client had incorrectly exposed Character Dialog tab names as menu actions. In this package, the Current Player Functions menu was restored to the legacy structure while preserving Refresh Character Info and Save Character Info.

The Character Dialog tabs remain available inside the dialog and were not removed.

## Protocol Notes

The menu actions use the existing legacy protocol paths:

- `MSG_ADD_VASSAL`
- `MSG_CHANGE_PATRON`
- `MSG_CHANGE_PASSWORD`

No character-save serialization changes were made.
