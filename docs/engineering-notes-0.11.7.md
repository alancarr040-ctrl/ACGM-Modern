# Engineering Notes - 0.11.7

## Scope

0.11.7 restores the first server-backed Administrator functions using the original VB6 source as protocol authority.

## Restored Admin Actions

- Backup Database
- Reset Character's Password
- Change Character's Security Level

## Legacy Message IDs

- `MSG_RESET_PASSWORD = 110`
- `MSG_BACKUP_DB = 111`
- `MSG_CHANGE_SEC_LEVEL = 112`

## POST Field Mapping

Backup Database:
- `charid` = logged-in character ID

Reset Password:
- `charid` = logged-in character ID
- `resetid` = selected character ID

Change Security Level:
- `charid` = logged-in character ID
- `changeid` = selected character ID
- `newlevel` = legacy numeric security level

## Security Levels

- Normal User = 1
- Administrator = 3

## Deferred

Server Setup remains deferred because the legacy form includes additional server-wide configuration fields. It should be restored separately to reduce risk.

## Compatibility

No tree parser, character parser, award parser, save protocol, or existing message format behavior was changed.
