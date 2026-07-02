# Legacy Login Map

The VB6 screenshot corresponds to `frmPlayerSetup.frm`, not `frmLogin.frm`.

## VB6 form

`frmPlayerSetup`

Caption:

```text
Player Login
```

Frames:

```text
Server Address
Character Setup
```

Controls:

- `cmbServerAddress`
- `cmbCharName`
- `txtPassword`
- `cmdOK`
- `cmdCancel`

## Behavior preserved in 0.2

- User enters an ACGM `server.cgi` address.
- User enters/selects character name.
- Password is required.
- Password length must be between 6 and 10 characters.
- Last server and last player are saved.
- Server/player history is saved in an `acgm.ini`-style file.
- `http://` and `https://` are stripped for storage, but HTTPS is used for connection by default.

## Intentional modernization

- The old VB6 client only handled plain HTTP over Winsock.
- The modern client stores the address similarly to VB6 but reconnects using HTTPS by default.
- Passwords are not saved.
