# Engineering Notes - 0.8.0

## Scope

0.8.0 adds the first allegiance management workflows to the modern client while preserving the legacy server protocol.

## Legacy operations mapped from VB6

The original `frmMain.frm` uses these operations:

- `MSG_ADD_VASSAL = 101`
- `MSG_REMOVE_VASSAL = 102`
- `MSG_CHANGE_PATRON = 103`

### Add Vassal

Legacy fields:

- `msgid`
- `charid`
- `addtoid`
- `vassalname`
- `vassallist`
- `patronid`

### Remove Vassal

Legacy fields:

- `msgid`
- `charid`
- `patronid`
- `vassalid`
- `vassalname`
- `vassallist`

### Change Patron

Legacy fields:

- `msgid`
- `moveid`
- `charid`
- `oldpatronid`
- `oldpatronname`
- `oldpatronvassallist`
- `newpatronid`
- `newpatronname`
- `newpatronvassallist`

## Safety behavior

The UI stages changes locally. The server is contacted only when **Save Changes** is clicked.

Reset Changes discards pending local allegiance changes and reloads the current displayed data without contacting the server.

## Known future work

- Match the original security-level and re-authentication behavior more exactly.
- Replace typed-name dialogs with fuller legacy-compatible find/select dialogs.
- Improve server error-code friendly messages for add/remove/change patron failures.
