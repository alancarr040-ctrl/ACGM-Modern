Package

0.11.12

Title

Legacy Character Save Serialization Certification

Goal

Reproduce VB6 MSG_UPDATE_CHAR_INFO serialization exactly.

Scope

Outgoing character save serializer only.

Do Not Modify

See AI_CERTIFIED_SUBSYSTEMS.md

Known Regression

Awards removed after save.

PK not preserved.

Serializer differs from VB6.

Objectives

✓ Preserve all fields

✓ Preserve trailing empty fields

✓ Preserve awards

✓ Preserve PK

✓ Preserve Security

✓ Preserve Main Character

✓ Preserve Mule

✓ Preserve Rescue

✓ Preserve Monarch

Diagnostics

character-save-diagnostics.log

Deliverables

Updated VS Solution

Documentation

ZIP