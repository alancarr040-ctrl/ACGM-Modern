# Engineering Notes - 0.9.0 Skills Tab Recreation

## Principle
Recreation first. Modernization underneath.

The legacy VB6 client exposed Skills as a top-level character tab containing a child tab strip with two pages:

- Training
- Skill Values

The Training page organized skills into four list boxes:

- Specialized Skills
- Trained Skills
- Untrained Skills
- Unusable Skills

The Skill Values page used a two-column grid:

- Skill
- Skill Value

## Legacy Format
The server stores character skill data as a single encoded field in the character-info response.

The observed legacy format is:

```text
skill_id,training_level,skill_value!skill_id,training_level,skill_value
```

Training levels:

```text
0 = Unusable
1 = Untrained
2 = Trained
3 = Specialized
```

## Skill Definitions
This package recreates the original VB6 `AddSkills()` list and preserves the original spelling, including `Missle Defense` as it appeared in the legacy source.

## Implementation
- `Models/SkillInfo.cs` adds the skill model.
- `Protocol/LegacySkillParser.cs` parses and serializes the legacy skill string.
- `MainForm.cs` builds the Skills tab UI and keeps protocol parsing separate from the visual controls.

## Save Behavior
When skills are changed, Save Changes now includes the serialized `SKILLS` field through the existing legacy character update call. No server protocol changes were introduced.

## Caution
Live save behavior should be tested carefully against a test character/server because legacy server-side handling determines which fields are accepted and persisted.
