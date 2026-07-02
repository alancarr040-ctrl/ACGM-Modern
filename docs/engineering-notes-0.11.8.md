# Engineering Notes - 0.11.8 Server Setup Restoration

0.11.8 restores the legacy Administrator -> Server Setup workflow.

VB6 reference behavior:

- Form: frmNewServerSetup.frm
- Message: MSG_CHANGE_SERVER_SETUP = 109
- Fields: guildname, defaultpw, use_rescue_squad, rescue_squad_name, resetpw
- Password length validation follows PW_MIN=6 and PW_MAX=10 when a new default password is entered.
- defaultpw is omitted when blank, matching VB6 behavior so the existing default password is not overwritten.

Safety constraints:

- No tree parser changes.
- No character parser changes.
- No awards parser changes.
- No character save protocol changes.
