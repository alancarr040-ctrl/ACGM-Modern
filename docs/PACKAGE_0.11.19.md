# Package

0.11.19

# Title

Legacy Protocol Inventory & Documentation

# Goal

Document for all legacy protocol and data structure implemented in this modernization

**Recreation first. Modernization underneath.**

No modern enhancements are included in this package.

# Background

The original VB6 code used specific protocol and data structure it is the intent of this project to attempt to recreate or reuse all of these
protocols and data structure to the best of our ability.  We will document every known client/server message

This package does not restore or enhance the package at all

# Scope

No new UI is introduced.  This package documents known server/client messages

There is nothing to restore as this is completely function testing

# Do Not Modify

See:

* AI_PROJECT_SPEC.md
* AI_CERTIFIED_SUBSYSTEMS.md

Certified subsystems remain frozen.

Do not modify:

* HTTPS
* Login
* Tree Parser
* Character Parser
* Character Editing
* Character Save Serialization
* Character Save Certification Suite
* Tree Icons
* Current Player Functions
* Character Dialog
* Legacy Protocols

Unless a verified defect requires a targeted correction.

# Tree Behavior Certification

General

This package certifies the documentation of legacy messages and modern usage.

This will cover:

- Document all legacy messages used
- Document any if all legacy messages unable to be used1

# Certification Requirements

Verify:

* Documents exist

* No major issues

# Diagnostics

Reuse existing certification diagnostics where appropriate.

Do not introduce additional diagnostics unless required.

# Documentation

Update:

* README
* CHANGELOG
* RELEASE_NOTES
* LEGACY_DISCOVERIES.md
* AI_MODERNIZATION_BACKLOG.md
* Engineering Notes

Document:

Message Name
Direction
  Client → Server
  Server → Client
Purpose
Parameters
Response
Legacy VB6 Notes
Certification Status
33 fields
Exact ordering
Trailing empty fields preserved
Awards final field
PK
Rescue
Mule
Security
Main Character
Compact format
Node layout
Parent/child
Icon state
Awards storage
Skill storage
Tree Icons
Search Workspace
Serializer
Refresh behavior
Session persistence

This becomes the permanent reference for:

# Deliverables

* Complete Visual Studio Solution
* Updated Documentation
* Release Notes
* ZIP Package

# Version

ACGM Modern Client 0.11.19 – Legacy Protocol Inventory & Documentation
