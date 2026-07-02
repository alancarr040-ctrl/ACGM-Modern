# Legacy Character Information Map

The 0.5.0 parser maps the `MSG_GET_CHAR_INFO` / `msgid=107` response according to the VB6 `frmMain.frm` usage of `strData()`.

The response payload is split by `|` after legacy decoding.

| Index | Field |
|---:|---|
| 0 | Character ID |
| 1 | Name |
| 2 | Level |
| 3 | Rank |
| 6 | Number of followers |
| 8 | Class |
| 9 | Race |
| 10 | Sex |
| 11 | Is Mule |
| 12 | Mule For |
| 13 | Lifestoned At |
| 14 | Tied To |
| 15 | Bio |
| 16 | Skills |
| 17 | Real Name |
| 18 | City/State |
| 19 | Misc Real Life |
| 20 | Email |
| 21 | ICQ |
| 23 | Last Modified |
| 24 | Patron |
| 25 | Path To Monarch |
| 26 | PK |
| 27 | Regs_Rescue Member |
| 28 | Hide Info on Web |
| 29 | Can Summon |
| 30 | Main Character Name |
| 31 | Last Modified By Character ID |
| 32 | Awards |

Fields not yet displayed are retained for later tabs.
