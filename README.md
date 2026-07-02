# ACGM Modern Client

**Project Philosophy**
Recreation first. Modernization underneath.

**Primary Goal**
Recreate the legacy VB6 ACGM client while modernizing the underlying implementation without altering observable behavior, with one exception, Must support SSL Protocol as that is required in today's world.

**Project Rules**
- Legacy behavior always wins.
- Do not guess protocol behavior.
- Use legacy VB6 as source of truth.
- Modernize internally only.
- Preserve CGI compatibility.
- Preserve existing UI behavior unless the package specifically changes it.

**Coding Standards**
- Small focused commits.
- No unrelated refactoring.
- No formatting-only changes.
- Preserve comments where practical.
- Minimize scope.

**Testing Philosophy**
Every package must include:
- Manual test plan
- Regression checks
- Documentation updates
- Release Notes

**Deliverables**
- Visual Studio Solution
- Source
- Documentation
- Release Notes
- ZIP Package

# Feature Matrix

| Area | Status | Notes |
|---|---|---|
| HTTPS connection | Complete | Existing compatibility preserved. |
| Login | Complete | Existing compatibility preserved. |
| Legacy server.cgi compatibility | Complete | No msgid/request/response changes in 0.11.4. |
| Allegiance Tree | Complete / Fixed | Flag mapping corrected. |
| Basic Info | Complete | Existing behavior preserved. |
| Allegiance Info | Complete | Existing behavior preserved. |
| Skills | Complete | Existing behavior preserved. |
| Real Life & Contact Info | Complete | Existing behavior preserved. |
| Character editing framework | Complete | Awards serialization preserves final-field mapping. |
| Awards | Fixed | Read from final field, split by `!;`, display one per line. |
| Rescue Squad | Fixed | Reads field 27. |
| Portal List | Complete | Message ID 114 preserved. |
| Trade Skills | Complete | Message ID 115 preserved. |
| Search | Complete | Message ID 116 preserved. |
| Protocol separation | Complete | Constants/parser remain in `ACGM.Protocol`. |
| Model separation | Complete | Parsed character state exposed by `ACGM.Models`. |
| UI separation | Complete | UI consumes model output only. |

## 0.11.4 Verification

| Feature | Status | Notes |
|---|---:|---|
| Legacy tree split on `!;` | Complete | Matches VB6 behavior. |
| Tree flag mapping | Complete | Field 6 Mule, field 7 PK, field 8 Rescue Squad. |
| Tree icon priority | Complete | Mule, Rescue Squad, PK, Normal. |
| Awards display | Complete | Final character-detail field, one award per line. |
| Protocol compatibility | Complete | No msgid/POST/request/response changes. |
| Diagnostics | Active | Kept for confirmation testing. |


## 0.11.6 Feature Status

| Feature | Status | Notes |
|---|---|---|
| Administrator menu entry | Restored | Opens Administrator Functions dialog |
| Administrator workflow launcher | Restored | Uses verified existing client workflows |
| Server-side administrator tools | Pending | Disabled until VB6/CGI behavior is confirmed |
| Tree/parser/protocol behavior | Preserved | No changes from verified 0.11.4/0.11.5 behavior |

## 0.11.9 Update

- File -> Login restored for in-application login/session switching.
