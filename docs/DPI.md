# Dx.Domain Design Pressure Index

## How Every Change Is Judged

The Design Pressure Index (DPI) translates the Manifesto and Non-Goals into daily, mechanical decision-making.

Every pull request applies pressure to the system.  
The DPI determines whether that pressure strengthens the kernel or fractures it.

## Core Admission Criteria

A change may enter `Dx.Domain` only if all of the following are true:

- **Invariant Enforcement**  
  The change enforces a domain invariant that cannot be reliably enforced elsewhere.

- **Reduction of Accidental Complexity**  
  The change removes an existing source of boilerplate, ambiguity, or misuse.

- **Compiler-Assisted Correctness**  
  The change increases what the compiler can prove.

- **Teaches Through Friction**  
  The change makes the wrong thing harder or impossible.

- **Manifesto Alignment**  
  The change strengthens at least one explicit demand in the Manifesto.

- **Non-Goal Compliance**  
  The change violates none of the Non-Goals.

## Edge Placement Rules

If a change fails Core admission, relocate it:

- **Adapters**: persistence, transport, and framework-specific code.
- **Generators**: reduce repetition without altering semantics.
- **Analyzers**: static enforcement without runtime impact.
- **Outside Dx.Domain**: if it does not enforce invariants or increase correctness.

## Decision Rule

When in doubt, the change does not go into Core.  
Rejection is not failure; misplacement is.
