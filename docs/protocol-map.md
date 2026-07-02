# Legacy ACGM Server Protocol Map

## Transport in the VB6 client

Legacy source: `HTTPRequest.cls`

The old request is manually built as:

```text
POST <uri> HTTP/1.1
Host: <hostname>
Connection: close
User-Agent: ACGM 0.3
Content-length: <length>

<post-data>
```

Then VB6 connects with:

```vb
wskData.Connect strHostName, 80
```

This hard-coded port 80 Winsock transport is the reason HTTPS does not work.

## Modern replacement

Package 0.1 replaces this with `System.Net.Http.HttpClient`. `HttpClient` handles:

- TLS negotiation for `https://`
- Default port 443 for HTTPS
- Default port 80 for HTTP
- Host header
- Content-Length
- Connection handling

## Legacy post encoding

Legacy source: `HTTPRequest.cls`, `AddPostData`

Before adding field values to the POST body, the old client performs these replacements:

| Original | Replacement |
|---|---|
| `|` | `(pipe)` |
| `!;` | `(end)` |
| `&` | `(amp)` |

The modern `LegacyPostEncoder` preserves this behavior instead of switching to normal URL encoding. This is intentional for server compatibility.

## Shared fields

Legacy source: `frmMain.frm`, `PrepareConnection`

Most requests begin with:

| Field | Meaning |
|---|---|
| `charname` | current/login character name |
| `password` | current/login password |

Then the operation-specific `msgid` and additional fields are added.

## Message IDs

Legacy source: `constants.bas`

| Message | ID |
|---|---:|
| `MSG_GET_TREE` | 100 |
| `MSG_ADD_VASSAL` | 101 |
| `MSG_REMOVE_VASSAL` | 102 |
| `MSG_CHANGE_PATRON` | 103 |
| `MSG_LOGIN` | 104 |
| `MSG_INIT_NEW_SERVER` | 105 |
| `MSG_UPDATE_CHAR_INFO` | 106 |
| `MSG_GET_CHAR_INFO` | 107 |
| `MSG_CHANGE_PASSWORD` | 108 |
| `MSG_CHANGE_SERVER_SETUP` | 109 |
| `MSG_RESET_PASSWORD` | 110 |
| `MSG_BACKUP_DB` | 111 |
| `MSG_CHANGE_SEC_LEVEL` | 112 |
| `MSG_GET_RESCUE_SQUAD` | 113 |
| `MSG_GET_PORTAL_LIST` | 114 |
| `MSG_GET_TRADE_SKILL_LIST` | 115 |
| `MSG_SEARCH` | 116 |

## Response format

Legacy source: `HTTPRequest.cls`, close/data parsing logic

The client looks for:

```text
<START_HERE><result-code>;<payload><STOP_HERE>
```

A result code from 800 through 899 is treated as success. Other result codes are treated as server/application errors.

Known success codes:

| Code | Meaning |
|---:|---|
| 800 | OK |
| 801 | New server |

## Fields identified by operation

The following are the key operation fields identified in `frmMain.frm`.

### Login - `msgid=104`

Base fields only:

- `charname`
- `password`
- `msgid`

### Initialize new server - `msgid=105`

- `guildname`
- `defaultpw`
- `use_rescue_squad`
- `rescue_squad_name`

### Get tree - `msgid=100`

- `tree_type`
- `rootid`
- `disp_mules`
- `disp_sec_level`
- `disp_pks`
- `disp_rescue_squad`

### Get character info - `msgid=107`

- `getcharid`

### Add vassal - `msgid=101`

- `charid`
- `addtoid`
- `vassalname`
- `vassallist`
- `patronid`

### Change password - `msgid=108`

- `charid`
- `newpw`
- `confirmpw`

### Change patron - `msgid=103`

- `moveid`
- `charid`
- `oldpatronid`
- `oldpatronname`
- `oldpatronvassallist`
- `newpatronid`
- `newpatronname`
- `newpatronvassallist`

### Remove vassal - `msgid=102`

- `charid`
- `patronid`
- `vassalid`
- `vassalname`
- `vassallist`

### Update character info - `msgid=106`

- `charid`
- `updateid`
- `ISMULE`
- Additional character stat/skill keys are added dynamically from the form state.

### Change server setup - `msgid=109`

- `guildname`
- `defaultpw`
- `use_rescue_squad`
- `rescue_squad_name`
- `resetpw`

### Reset password - `msgid=110`

- `charid`
- `resetid`

### Backup database - `msgid=111`

- `charid`

### Change security level - `msgid=112`

- `charid`
- `changeid`
- `newlevel`

### Search - `msgid=116`

- `race`
- `rank`
- `skillsearch`

## Tree Record Mapping Confirmed for 0.11.4

`MSG_GET_TREE` returns records separated by `!;`. Each record has 9 pipe-delimited fields:

```text
id|name|level|rank|patron_id|vassal_ids|is_mule|is_pk|is_rescue_squad_member
```

VB6 passes fields 6, 7, and 8 directly into `Characters.Add` as `iIsMule`, `iIsPK`, and `iIsRescueSquadMember`.

Awards are not parsed from tree records. Awards are stored in the final `MSG_GET_CHAR_INFO` character-detail field.
