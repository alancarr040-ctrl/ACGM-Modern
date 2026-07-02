# Main Window Map – 0.4

0.4 aligns the modern client with the legacy `frmMain.frm` shell.

## Main regions

| Region | Legacy role | Modern implementation |
|---|---|---|
| Menu | `frmMain` menu bar | `MenuStrip` |
| Top tabs | Allegiance Tree, Flat Listing, rescue/search lists | Main `TabControl` |
| Left pane | Allegiance Tree | `SplitContainer.Panel1` |
| Right pane | Character details | `SplitContainer.Panel2` |
| Status bar | Download/connection/bytes messages | `StatusStrip` |

## Split behavior

The main `SplitContainer` now defaults to a 25% left panel and 75% right panel.  This better matches the legacy client while giving the character detail area more room.

The user may drag the splitter.  The chosen distance is saved to `acgm.ini` as `MainSplitterDistance` and restored on the next launch.

## Intentional non-goals

0.4 does not change server protocol behavior.  It only improves UI fidelity and layout stability.
