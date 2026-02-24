# DXT Invariants — `.dx/invariants.json` Schema

**Purpose:** Templates publish intent as data consumed by analyzers.

```json
{
  "version": "1.0",
  "templates": {
    "<template-id>": {
      "projects": {
        "<project-name>": {
          "role": "Contracts|Domain|Application|Infrastructure|Host|Shared",
          "layer": "Kernel|Primitives|Annotations|Facts|...",
          "requiredReferences": ["Package.Id"],
          "forbiddenReferences": ["Package.Id"],
          "namespacePrefix": "Company.Area.Project",
          "directory": "src/Area/Project",
          "semanticCaps": { "emitsApiHost": true, "emitsObservability": true }
        }
      },
      "invariants": ["DXT-001","DXT-002","DXT-003"]
    }
  }
}
```

**Analyzer Consumption:** Cross‑validate role presence (DXK001), allowed/forbidden refs (DXK002/DXT002/DXT003), refine rule activation via `semanticCaps`.
