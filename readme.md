(generated readme)

# Logic Puzzle CSP Engine

_A C# framework for modeling and solving Zebra / Einstein puzzles, self-reference quizzes, and related logic puzzles using Generalized Arc Consistency (GAC)._

---

## 📁 Project Structure

```
src/
  Csp/
  Generator/
  Zebra/
test/
  CspTests/
  GeneratorTests/
```

### **`Csp` — Core CSP Engine**

This is the heart of the project. It contains:

- **Generalized Arc Consistency (GAC)** implementation
- **Variable, Domain, and Constraint abstractions**
- A set of **CSP constraint classes** tailored for:

  - Zebra / Einstein puzzles
  - Self-reference quizzes (self-ref consistency rules, meta-constraints, etc.)

- **Builder classes** for ergonomically constructing puzzles:

  - `ZebraBuilder` for standard Zebra categories/attributes
  - `QuizBuilder` for recursive self-referential question sets

This layer is puzzle-agnostic: it doesn't know about clue pools, generation, or I/O — just pure CSP machinery and fluent puzzle builders.

---

### **`Generator` — Solution + Clue Pool Generation**

Logic for generating:

- **Random valid puzzle solutions** for Zebra (fixed-size domains)
- **Complete clue pools** from a single solution:

  - position-equals clues
  - equality clues
  - before / immediately-before
  - adjacency
  - (future) composite and high-order clues

- Eventually: **self-reference puzzle clue generation**

This module is used for puzzle design, generation, and solver stress-testing.

---

### **`src/Zebra` — Legacy Console App**

A simple console runner that currently:

- Contains the **older “matrix grid” CSP approach** originally used early in development
- Will eventually be replaced or removed
- Still useful as a reference and quick sanity-check tool

The modern system runs through `Csp` + `Generator`.

---

## 🧪 Unit Tests

The `test/` directory includes:

- **`CspTests/`**
  Tests for:

  - constraint semantics
  - clue equivalence / implication / intrinsic contradiction logic
  - builder behavior
  - domain pruning and consistency checks

- **`GeneratorTests/`**
  Tests for:

  - solution generator correctness
  - clue pool generation
  - redundancy and contradiction checks in clue sets
  - puzzle validity/integrity helpers

Tests serve both as correctness verification and as examples of how to use the engine.

---

## 🚀 Goals & Roadmap

- Expand Zebra generation into **full puzzle synthesis**:

  - clue pool → subset selection → CSP uniqueness check

- Expand Self-Ref Quiz engine:

  - meta-level constraints
  - generative question types

- Ditch the old console app solver
- Experiment with evolutionary puzzle generation: genetic algorithms and feasibility heuristics

---

## 🛠 Technologies

- **C# / .NET**
- **GAC (Generalized Arc Consistency)** for constraint propagation
- **TDD** with xUnit

---

## 📚 Overview

This project aims to be a **fully general logic-puzzle synthesizer and solver**:

- Define categories and attributes
- Add structural CSP constraints (all-different, bijection, adjacency, etc.)
- Add clue objects with well-defined:

  - equivalence
  - implication
  - intrinsic contradiction

- Generate solutions
- Generate maximal/minimal clue sets
- Test puzzle uniqueness
- Output playable puzzle instances

It is designed to be extensible, ergonomic, and semantically precise.
