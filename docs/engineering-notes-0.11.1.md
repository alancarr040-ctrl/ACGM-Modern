# Engineering Notes - 0.11.1

## Awards persistence

Legacy VB6 source in `frmMain.frm` saves Awards with:

```vb
Me.CheckTextBoxUpdate Me.txtAwards, "ADMIN_ONLY"
```

The modern 0.11.0 implementation incorrectly posted changed awards data as `AWARDS`. This patch changes the protocol field to `ADMIN_ONLY` while keeping the same `MSG_UPDATE_CHAR_INFO` request.

No server-side format changes are introduced.
