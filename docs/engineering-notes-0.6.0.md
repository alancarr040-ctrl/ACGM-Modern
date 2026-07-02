# Engineering Notes – 0.6.0

## Legacy save mapping

The VB6 client saves character data through `SaveCharChanges` in `frmMain.frm` using `MSG_UPDATE_CHAR_INFO` (`106`).

The modern client preserves the same endpoint and POST field names.

Required request identity fields:

- `charname`
- `password`
- `charid`
- `updateid`
- `msgid = 106`

Supported Basic Info update fields:

- `NAME`
- `LEVEL`
- `CLASS`
- `RACE`
- `RANK`
- `SEX`
- `ISMULE`
- `MULEFOR`
- `LSAT`
- `TIEDTO`
- `BIO`
- `ISPK`
- `ISRESCUE`
- `ISPRIVATE`
- `CAN_SUMMON`
- `MAIN_CHAR`

## Architecture direction

The package starts a cleaner separation without disrupting the Visual Studio project:

- Forms remain in `Forms/`.
- Models remain in `Models/`.
- Protocol implementation remains in `Protocol/`.
- Logging starts in `Logging/`.
- Legacy-compatible transport remains `AcgmHttpClient`.

Future packages may split these folders into separate projects once the behavior stabilizes.
