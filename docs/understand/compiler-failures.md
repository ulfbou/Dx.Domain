# From compiler output to corrective action

A DXA diagnostic indicates a statically observable pattern that violates declared architectural constraints. It does not prove program correctness or rule out similar violations via other code paths.

## Workflow

When you see a diagnostic (e.g., `DXA020: Result ignored`):

1. **Record the diagnostic ID and location**
   Note the exact rule (DXA020), the file, line, and message from build output.

2. **Open the diagnostic reference**
   Go to [Diagnostics](../reference/diagnostics/index.md) and find the matching rule.

3. **Confirm analyzer configuration**
   Verify that your project includes a published Dx.Domain package and that `.editorconfig` contains any required scope mappings (`dx.scope.map`, `dx.facade.root`, etc.).

4. **Understand the guarantee**
   Read the rule's **Guarantee** and **Limits** sections. Not all violations are detected, and some violations require intentional code suppression.

5. **Review remediation guidance**
   Follow the specific steps for your rule. For example:
   - DXA010: Ensure construction uses the configured facade or factory.
   - DXA020: Return, transform, or explicitly handle the Result.
   - DXA065: Verify XML doc references point to valid members or URIs.

6. **Apply and test**
   Make the correction and add a focused regression test that covers both the reported pattern and the corrected form.

7. **Rebuild and confirm**
   Run `dotnet build` and verify the exact diagnostic count decreases.

## When to suppress

Suppression is allowed **only** when:
- the code genuinely violates the rule's intent,
- business context justifies the exception,
- a comment documents the reason and scope,
- a test exists that would fail if suppression were removed.

Future rule **DXA090** will audit suppression discipline.

## Understanding limits

Each rule has explicit **Gaps** and **Non‑Guarantees**:
- Reflection and serialization bypass all static analyzers.
- Transitive correctness is not guaranteed (a returned Result may still go unhandled downstream).
- Cross‑assembly violations are not detected.

See [Enforcement Specification](enforcement-specification.md) for formal guarantee boundaries.

## Additional resources

- [Respond to diagnostics](../use/guides/respond-to-diagnostics.md) — consumer task workflow
- [Enforcement Model](enforcement-model.md) — how analyzer strength is classified
- [Enforcement Specification](enforcement-specification.md) — formal guarantees for each domain
