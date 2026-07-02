# Engineering Notes – 0.9.1 Skills Display Refinement

## Goal

Improve readability of the Skill Values tab while preserving the legacy ACGM presentation model.

## Display Ordering

The Skill Values list is now ordered in two groups:

1. Skills with numeric values greater than zero.
2. Skills with no value or a zero value.

Inside each group, the original legacy skill order is preserved by sorting on the legacy skill id.

## Training Status

The legacy skill parser continues to map training state as follows:

- `3` = Specialized
- `2` = Trained
- `1` = Untrained
- `0` / missing / unknown = Unusable

The Training tab groups skills by status. The Skill Values tab exposes the same status through a dedicated Training column.

## Compatibility

No server protocol changes were made in this package.
