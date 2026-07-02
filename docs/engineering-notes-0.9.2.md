# Engineering Notes – 0.9.2 Skills Grouping & Ordering

## Guiding Principle

Recreation first. Modernization underneath.

## Implementation

The Skill Values list now builds grouped display rows from the parsed `SkillInfo` collection. The UI remains a classic WinForms `ListView`, but the ordering logic is kept separate from parsing.

Sorting order:

1. Specialized
2. Trained
3. Untrained
4. Unusable

Within each status group:

1. Skills with values
2. Skills without values
3. Legacy skill order

## Compatibility

No protocol changes were made. The legacy server response is parsed into the existing `SkillInfo` model, and all display ordering is handled client-side.
