---
name: fix-bug
description: Fix a bug following the standard workflow — reproduce, locate, fix, regression-test, commit.
disable-model-invocation: true
---

Fix the bug described by the user (symptom: $ARGUMENTS) by following this workflow:

1. First, infer the pages and flows involved from the symptom, and confirm your understanding of the symptom with the user.
2. Trace down from the Controller into the Service and Repository to locate the root cause; after explaining the root cause, **wait for the user's confirmation** before making any changes.
3. Fix it with the smallest possible change — do not opportunistically refactor unrelated code.
4. Use the code-reviewer to verify the change.
5. Add a regression test (first confirm the logic would fail before the fix), then use the test-runner to run `dotnet test` and confirm everything is green.
6. Prompt the user to go back to the page and test it live; once confirmed, write the commit message in the "symptom → root cause → fix" format and commit.
