# Package

0.11.13

# Title

Legacy Character Save Certification Suite

# Goal

Create a permanent developer certification suite for validating legacy character-save behavior without modifying production functionality.

# Scope

Build a reusable certification framework for `MSG_UPDATE_CHAR_INFO` that allows future releases to verify character-save compatibility with the legacy VB6 client.

No production behavior should change.

# Do Not Modify

See:

* AI_PROJECT_SPEC.md
* AI_CERTIFIED_SUBSYSTEMS.md

All certified subsystems remain frozen.

# Objectives

* Build a developer-only character save diagnostics framework.
* Generate `character-save-diagnostics.log`.
* Record every outgoing field in order.
* Record field index.
* Record field name.
* Record serialized value.
* Record final serialized payload.
* Record outgoing POST payload.
* Record CGI server response.
* Record loaded character values after save (when available).
* Add optional field-by-field comparison against the expected legacy layout.
* Allow diagnostics to be enabled or disabled without recompiling.
* Ensure diagnostics have zero impact when disabled.

# Diagnostics

Create:

* character-save-diagnostics.log

Include:

* Timestamp
* Character Name
* Operation
* Field Index
* Field Name
* Field Value
* Final Serialized Payload
* POST Payload
* Server Response
* Reloaded Character Values (if available)

# Documentation

Update:

* README
* CHANGELOG
* RELEASE_NOTES
* LEGACY_PROTOCOL.md
* LEGACY_DISCOVERIES.md
* Engineering Notes

Document:

The Character Save Certification Suite provides a repeatable method for validating future changes to the legacy character-save subsystem without affecting production behavior.

# Testing

Verify:

1. Diagnostics disabled:

   * Normal operation unchanged.
   * No log file generated.
2. Diagnostics enabled:

   * Log file generated.
   * All fields recorded.
   * Payload recorded.
   * Server response recorded.
3. Save William Ohmsford.
4. Compare diagnostics against expected legacy field layout.
5. Confirm no behavioral regressions.

# Deliverables

* Complete Visual Studio Solution
* Updated Documentation
* Release Notes
* ZIP Package

# Version

ACGM Modern Client 0.11.13 – Legacy Character Save Certification Suite
