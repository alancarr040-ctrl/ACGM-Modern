# Implementation Notes

## Why Package 0.1 is focused on HTTPS first

The old client cannot talk to HTTPS servers because it uses raw Winsock and manually writes HTTP text to port 80. Adding SSL to that design would be fragile and not worth preserving.

The rewrite starts with the protocol layer so we can prove compatibility with the existing `server.cgi` before recreating the full UI.

## Modern classes added

### `AcgmHttpClient`

Wraps `HttpClient` and exposes legacy server operations.

Currently implemented:

- `LoginAsync`
- `GetTreeAsync`
- `GetCharacterInfoAsync`
- generic `PostAsync`

### `LegacyPostEncoder`

Preserves the VB6 `AddPostData` field escaping behavior.

### `AcgmResponse`

Parses the legacy response envelope:

```text
<START_HERE>800;payload<STOP_HERE>
```

### `EndpointParser`

Accepts full URLs and defaults bare server/path values to `https://`.

## Recommended next package

`ACGM Modern Client 0.2 - Profiles & Full Request Surface`

Suggested scope:

- Add player/server profile storage.
- Recreate `acgm.ini` import support.
- Implement all remaining message-id request builders.
- Add response fixtures for testing against captured server output.
- Add a diagnostics panel showing exact POST fields without exposing passwords by default.

## Later packages

- `0.3` - Character/tree model parsing
- `0.4` - Main tree UI shell
- `0.5` - Character info and editing forms
- `0.6` - Vassal/patron operations
- `0.7` - Search/rescue/portal/trade skill views
- `1.0` - Installer-ready modern ACGM client
