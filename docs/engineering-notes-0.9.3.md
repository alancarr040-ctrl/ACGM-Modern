# Engineering Notes – 0.9.3 Legacy Feature Inventory & Gap Matrix

## Purpose

Package 0.9.3 intentionally avoids major functionality changes. Its purpose is to ground the rebuild in the actual VB6 source before continuing with additional tabs and workflows.

## Key Finding

The modern client is far enough along that feature work should now be driven by a legacy feature matrix, not by visual guesswork. Several tabs previously existed only as placeholders in the modern client but have concrete controls, functions, and message IDs in the VB6 source.

## Important Correction

A previous proposed package described verifying all tabs and menus. That was too broad because many legacy tabs and menus are not implemented yet. This package corrects the approach by separating:

1. **Inventory** — what exists in VB6.
2. **Matrix** — what exists in modern ACGM.
3. **Verification** — only for features implemented and tested.
4. **Roadmap** — what should be built next.

## Lowest-Risk Next Feature

The recommended next feature is the Real Life & Contact Info tab because its data is already included in the current character info response:

- index 17: Real Name
- index 18: City/State/Country
- index 19: Misc real-life information
- index 20: Email
- index 21: ICQ

Save keys are also already known from `SaveCharChanges`:

- `REALNAME`
- `CITYSTATE`
- `MISCINFO`
- `EMAIL`
- `ICQ`

## Higher-Risk Features

The following should be implemented carefully and preferably tested against disposable data or a non-production server:

- Add/remove/change patron workflows.
- Administrator functions.
- Backup database.
- Password reset.
- Security level changes.

## Protocol Safety Principle

When a request format is uncertain, do not guess destructively. Document the uncertainty, implement safe display-only behavior first, then add write behavior once request/response handling is confirmed.

