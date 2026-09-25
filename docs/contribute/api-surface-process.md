# API Surface Process

1. Identify the invariant or correctness gap.
2. Show why the change cannot live in an analyzer, generator, adapter, or consumer project.
3. Complete DPI review.
4. Update the appropriate public API baseline before implementation review closes.
5. Build every target framework and inspect the produced package.
6. Update package documentation, API reference, stability classification, and release notes.
7. Treat an unapproved baseline difference as a release blocker.

An empty or missing baseline is not evidence that the surface is frozen.
