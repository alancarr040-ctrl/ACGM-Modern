# Engineering Notes - 0.11.10a

## Security Level Dialog Default Fix

0.11.10a corrects the Administrator -> Change Character's Security Level dialog so the combo box initializes from the known logged-in character security level when the selected target is the current login character.

Legacy values remain unchanged:

- 1 = Normal User
- 3 = Administrator

The legacy login response exposes the current login security level. The modern client now uses that value when the administrator changes their own security level, instead of always defaulting the dialog to Normal User.

For other selected characters, the dialog keeps the safe legacy default unless a verified server-provided value is available in a future protocol pass.

No protocol, parser, tree, awards, or save behavior was changed.
