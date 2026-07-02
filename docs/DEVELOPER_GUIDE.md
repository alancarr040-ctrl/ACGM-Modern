# ACGM Modern Client Developer Guide

## Purpose

This guide explains how to work on ACGM Modern Client without accidentally breaking certified legacy behavior.

## Project Philosophy

> Recreation first. Modernization underneath.

0.11 is the Legacy Certification phase. It recreates the original VB6 client behavior. 0.12 and later may add modern enhancements on top of the certified baseline.

## Documentation Map

| Document | Purpose |
|---|---|
| `AI_PROJECT_SPEC.md` | Project philosophy and engineering rules |
| `AI_CERTIFIED_SUBSYSTEMS.md` | Frozen certified subsystems and current open work |
| `LEGACY_DISCOVERIES.md` | Facts learned about VB6 behavior during certification |
| `LEGACY_PROTOCOL.md` | Protocol and data-format reference |
| `CERTIFICATION_REPORT.md` | Gold certification status summary |
| `AI_MODERNIZATION_BACKLOG.md` | Ideas deferred until 0.12+ |
| `CHANGELOG.md` | Versioned change history |
| `RELEASE_NOTES.md` | Release-facing package notes |
| `docs/ENGINEERING_NOTES.md` | Engineering notes index |

## Working Rules

Before changing code:

1. Read `AI_PROJECT_SPEC.md`.
2. Read `AI_CERTIFIED_SUBSYSTEMS.md`.
3. Confirm whether the requested work is legacy certification or modernization.
4. If the work touches a Gold-certified subsystem, stop unless the package explicitly reopens it.

## Certification vs Modernization

Ask:

> Did the VB6 client do this?

If yes, it belongs in 0.11 legacy certification.

If no, record it in `AI_MODERNIZATION_BACKLOG.md` unless the user explicitly opens a modernization package.

## Protocol Rules

- Do not invent new CGI fields during legacy certification.
- Do not change `msgid` values.
- Do not replace legacy response parsing.
- Preserve custom legacy encoding for `|`, `!;`, and `&`.
- Preserve `ADMIN_ONLY` awards behavior in character saves.

## Package Workflow

Each package should provide:

- Source updates, if any.
- Documentation updates.
- Release Notes.
- Manual test checklist.
- ZIP package.

Documentation-only packages may make no code changes.

## End-of-Package Checklist

- Code complete, if code changed.
- Manual testing complete.
- README updated.
- CHANGELOG updated.
- RELEASE_NOTES updated.
- Engineering Notes updated.
- LEGACY_DISCOVERIES updated, if new legacy behavior was learned.
- AI_CERTIFIED_SUBSYSTEMS updated, if certification status changed.
- AI_MODERNIZATION_BACKLOG updated, if enhancement ideas were discovered.
- ZIP created.
- Package certified or explicitly marked for follow-up.
