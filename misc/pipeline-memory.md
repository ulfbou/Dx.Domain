# Pipeline Memory: Zentient.Metadata

## Goals
- Ensure `package-and-publish` job completes and publishes NuGet packages on tag push. This is the main target.
- Ensure `preview-packages` job works for PRs and branch pushes.
- Prevent duplicate/parallel pipelines for the same event.
- Avoid repeating failed fixes and keep a log of all attempted solutions.
- Use .NET 9.0.100 for deterministic builds (setup-dotnet ensures SDK availability).
- Only run-ci-checks should use the container; publish/preview jobs should run natively.

## Critical Instructions
- **ALWAYS commit and push all changes before creating and pushing a tag.**
- **Do everything required to ensure NuGet publish completes successfully.**
- If NuGet publish fails, immediately investigate logs, debug, and retry with fixes.

## Failed Fixes / Attempts
- [x] Removing container from `package-and-publish` (did not resolve publish failure due to SDK mismatch).
- [x] Updating `global.json` to 9.0.304 (fixed SDK issue locally, but not on GitHub Actions runner).
- [x] Adding smarter triggers with `github.ref_type` and job-level `if:` (to prevent duplicate pipelines).
- [x] Ensured build step before pack/publish (to guarantee artifacts are built).
- [x] Confirmed only one pipeline runs for tag push (still need to verify publish success).
- [x] Set `global.json` to 9.0.304, but GitHub Actions runner for package-and-publish only has 8.0.119 (Linux), not 9.0.304 (Windows).
- [x] Changed package-and-publish to use windows-latest runner (should ensure .NET 9.0.304 SDK is available).
- [x] Remove tags trigger from top-level push to avoid duplicate pipelines.
- [x] Remove container from package-and-publish and preview-packages jobs.
- [x] Clean artifacts folder step failed if directory did not exist (fixed to not fail on missing directory).
- [x] Used custom Docker image for all jobs (not needed for publish/preview jobs).
- [x] Use setup-dotnet to ensure .NET 9.0.x is available for publish/preview jobs.
- [ ] Awaiting next step: verify NuGet publish completes and address any errors immediately.

## Lessons Learned / Context
- GitHub Actions Windows runners have .NET SDK 9.0.304, but Ubuntu runners may not.
- Custom Docker image provides .NET 9.0.100 and 8.0.x on Ubuntu.
- Only run-ci-checks should use the container; publish/preview jobs should run natively.
- Remove `tags: ['v*']` from workflow trigger to avoid duplicate pipelines.
- Use job-level `if:` for tag-based jobs.
- Ensure correct job dependencies and directory creation.
- Confirm SDK version matches your container or is set up with setup-dotnet.
- **Always commit and push before tagging.**

## Next Steps
- Commit and push all changes before creating and pushing a tag.
- Ensure NuGet publish completes; if not, debug and fix immediately.
- Monitor and update this file with new issues, fixes, and lessons learned.
