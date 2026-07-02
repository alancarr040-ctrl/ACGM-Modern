# Engineering Notes - 0.11.2

## Rescue Tree Icon Priority

The legacy VB6 client chooses tree icons in `frmMain.frm` `AddNode` using this precedence:

1. Mule
2. Rescue Squad
3. PK
4. Normal

The modern client previously checked PK before Rescue Squad. If a tree payload had multiple flags set, a Rescue Squad member could appear with the PK icon. This package updates `GetTreeImageKey()` to match the VB6 ordering.

No server protocol changes were made.
