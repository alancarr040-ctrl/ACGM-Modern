# ACGM Modern Client

Project Philosophy

Recreation first. Modernization underneath.

Primary Goal

Recreate the legacy VB6 ACGM client while modernizing the underlying implementation without altering observable behavior.

Project Rules

• Legacy behavior always wins.
• Do not guess protocol behavior.
• Use legacy VB6 as source of truth.
• Modernize internally only.
• Preserve CGI compatibility.
• Preserve existing UI behavior unless the package specifically changes it.

Coding Standards

• Small focused commits.
• No unrelated refactoring.
• No formatting-only changes.
• Preserve comments where practical.
• Minimize scope.

Testing Philosophy

Every package must include:

• Manual test plan
• Regression checks
• Documentation updates
• Release Notes

Deliverables

• Visual Studio Solution
• Source
• Documentation
• Release Notes
• ZIP Package