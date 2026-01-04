# Development Guidelines for Dots and Boxes

This document provides comprehensive guidelines for developers working on the Dots and Boxes project. It covers project setup, development workflow, architecture, building, testing, and contribution guidelines.

## Table of Contents

1. [Project Setup](#project-setup)
2. [Development Workflow](#development-workflow)
3. [Code Structure](#code-structure)
4. [Building and Testing](#building-and-testing)
5. [Architecture Decisions](#architecture-decisions)
6. [Debugging](#debugging)
7. [Performance Optimization](#performance-optimization)
8. [Contributing Guidelines](#contributing-guidelines)

---

## Project Setup

### Prerequisites

- **.NET SDK 6.0 or later** - Download from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
- **Visual Studio 2022** or **Visual Studio Code** with C# and F# extensions
- **Git** - Version control system

### Initial Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/dminra/DotsAndBoxes.git
   cd DotsAndBoxes
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Verify Setup**
   ```bash
   dotnet build
   dotnet test
   ```

### IDE Configuration

#### Visual Studio 2022
- Install workloads: ".NET desktop development" and "F# language support"
- Open `DotsAndBoxes.sln`
- Visual Studio will automatically restore NuGet packages

#### Visual Studio Code
- Install extensions:
  - C# (powered by OmniSharp)
  - Ionide for F#
  - .NET Core Tools
- Open the project root folder
- Follow prompts to install recommended extensions

---

## Development Workflow

### Branch Strategy

We follow a **Git Flow** branching model:

- **main** - Production-ready code, stable releases
- **develop** - Integration branch for features
- **feature/*** - Feature branches (e.g., `feature/game-ai`)
- **bugfix/*** - Bug fix branches (e.g., `bugfix/scoring-issue`)
- **hotfix/*** - Critical production fixes

### Creating a Feature Branch

1. Ensure you're on the `develop` branch:
   ```bash
   git checkout develop
   git pull origin develop
   ```

2. Create a feature branch:
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. Work on your feature with regular commits:
   ```bash
   git add .
   git commit -m "feat: add descriptive message"
   ```

4. Keep your branch updated:
   ```bash
   git fetch origin
   git rebase origin/develop
   ```

### Commit Message Convention

Follow the **Conventional Commits** specification:

```
<type>(<scope>): <subject>

<body>

<footer>
```

- **type**: feat, fix, docs, style, refactor, perf, test, chore
- **scope**: Component or file affected (optional)
- **subject**: Brief description (imperative mood, lowercase)
- **body**: Detailed explanation (optional)
- **footer**: Issue references and breaking changes (optional)

**Examples:**
```
feat(ai): implement minimax algorithm for player moves
fix(game): correct scoring calculation for box completion
docs: update setup instructions in DEVELOPMENT.md
refactor(board): simplify board state representation
```

### Pull Request Process

1. Push your feature branch:
   ```bash
   git push origin feature/your-feature-name
   ```

2. Create a Pull Request on GitHub with:
   - Clear title and description
   - Reference to related issues
   - Checklist of changes

3. Wait for code review and CI checks to pass

4. Address review comments and request re-review

5. Squash and merge commits when approved

---

## Code Structure

### Project Organization

```
DotsAndBoxes/
├── src/
│   ├── Core/                    # F# Core game logic
│   │   ├── Types.fs             # Domain types and models
│   │   ├── Board.fs             # Board representation and operations
│   │   ├── Game.fs              # Game state and rules
│   │   ├── AI/                  # AI algorithms
│   │   │   ├── Minimax.fs       # Minimax implementation
│   │   │   └── Heuristics.fs    # Evaluation functions
│   │   └── Core.fsproj
│   │
│   ├── UI/                      # C# UI and Application
│   │   ├── Models/
│   │   ├── ViewModels/
│   │   ├── Views/
│   │   ├── Services/
│   │   └── UI.csproj
│   │
│   └── Common/                  # Shared utilities
│       ├── Logging.fs
│       ├── Configuration.cs
│       └── Common.fsproj
│
├── tests/
│   ├── Core.Tests/              # F# unit tests
│   │   ├── BoardTests.fs
│   │   ├── GameTests.fs
│   │   ├── AITests.fs
│   │   └── Core.Tests.fsproj
│   │
│   └── UI.Tests/                # C# integration tests
│       ├── GameViewModelTests.cs
│       └── UI.Tests.csproj
│
├── docs/                        # Documentation
├── DotsAndBoxes.sln             # Solution file
└── DEVELOPMENT.md               # This file
```

### F# Core Layer

The core game logic is implemented in F# for:
- **Type Safety**: Leverages F# type system for domain modeling
- **Immutability**: Functional approach ensures thread-safe game state
- **Pattern Matching**: Elegant handling of game states
- **Performance**: Compiled to efficient IL

**Key Modules:**

#### Types.fs
Defines domain-specific types:
```fsharp
type Coordinate = int * int
type Box = { TopLeft: Coordinate; Size: int }
type Move = { From: Coordinate; To: Coordinate }
type GameState = { Board: Board; CurrentPlayer: Player; Score: Map<Player, int> }
```

#### Board.fs
Board representation and operations:
- `initBoard: int -> int -> Board` - Initialize empty board
- `makeMove: Board -> Move -> Board` - Apply move to board
- `getAvailableMoves: Board -> Move list` - Get valid moves
- `isGameOver: Board -> bool` - Check win condition

#### Game.fs
Game state management:
- Game initialization
- State transitions
- Turn management
- Score calculation
- Win/loss determination

#### AI/Minimax.fs
AI decision-making:
- Minimax algorithm with alpha-beta pruning
- Configurable search depth
- Evaluation functions for board states

### C# UI Layer

The user interface and application layer in C#:
- **MVVM Pattern**: Clean separation of concerns
- **Binding Integration**: WPF/WinForms data binding
- **Interop**: Seamless F# and C# integration

**Key Components:**

#### Models/
- Game models for UI representation
- Data structures for binding

#### ViewModels/
- `GameViewModel` - Main game logic coordination
- `BoardViewModel` - Board display state
- `PlayerViewModel` - Player information and stats

#### Views/
- XAML definitions for game UI
- Event handlers for user input
- Visual state management

#### Services/
- `GameService` - Orchestrates F# core with UI
- `AIService` - Manages AI player moves
- `FileService` - Save/load game functionality
- `ConfigurationService` - Application settings

### F# to C# Interoperability

**Best Practices:**

1. **Expose F# Functions in C#-friendly ways:**
   ```fsharp
   // F# Core
   module GameEngine =
       let makeMove (board: Board) (move: Move) : Board = ...
       
   // C# wrapper (if needed)
   [<CompiledName("MakeMove")>]
   let makeMove board move = GameEngine.makeMove board move
   ```

2. **Use Compatible Types:**
   - Prefer `list<'a>` over `seq<'a>` for C# consumption
   - Return tuples only when necessary; consider records
   - Use BCL types (`int`, `string`) over F#-specific types

3. **Document Public APIs:**
   ```fsharp
   /// <summary>
   /// Evaluates board state using heuristic evaluation
   /// </summary>
   /// <param name="board">Current board state</param>
   /// <returns>Evaluation score</returns>
   let evaluateBoard (board: Board) : int = ...
   ```

---

## Building and Testing

### Build Commands

```bash
# Build solution
dotnet build

# Build specific project
dotnet build src/Core/Core.fsproj

# Release build
dotnet build -c Release

# Verbose output
dotnet build -v diagnostic
```

### Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Core.Tests/Core.Tests.fsproj

# Run tests with verbose output
dotnet test -v normal

# Run tests matching pattern
dotnet test --filter "Category=AI"

# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Test Structure

**Unit Tests (F#):**
```fsharp
[<TestFixture>]
type BoardTests() =
    [<Test>]
    member this.``Init board creates correct dimensions`` () =
        let board = Board.initBoard 6 6
        Assert.AreEqual(6, Board.width board)
        Assert.AreEqual(6, Board.height board)
```

**Integration Tests (C#):**
```csharp
[TestFixture]
public class GameViewModelTests
{
    [Test]
    public void MakeMove_UpdatesBoardViewModel()
    {
        // Arrange
        var viewModel = new GameViewModel();
        
        // Act
        viewModel.MakeMove(move);
        
        // Assert
        Assert.That(viewModel.BoardState, Is.Not.Null);
    }
}
```

### Continuous Integration

Tests run automatically on:
- Pull request creation/updates
- Commits to `main` and `develop`
- Manual trigger via GitHub Actions

---

## Architecture Decisions

### Functional Core, Imperative Shell

**Decision**: Core game logic in F# (pure functions), UI in C# (imperative)

**Rationale**:
- F# ensures game state correctness through immutability
- C# provides practical UI development
- Clear separation enables independent testing

**Impact**:
- Easier to test core logic
- Simpler reasoning about game state changes
- Clear boundaries between layers

### Immutable Game State

**Decision**: Game state represented as immutable data structures

**Rationale**:
- Prevents bugs from unexpected mutations
- Simplifies AI move lookahead
- Enables efficient undo/replay functionality

**Impact**:
- Small memory overhead from copying
- Excellent for concurrent operations
- Easy to implement game history

### Minimax with Alpha-Beta Pruning

**Decision**: AI uses minimax algorithm with alpha-beta pruning

**Rationale**:
- Optimal for two-player zero-sum games
- Alpha-beta pruning reduces search space exponentially
- Deterministic and easily tunable

**Impact**:
- Configurable difficulty via search depth
- Strong AI performance without machine learning complexity
- Predictable computation time

### Dependency Injection

**Decision**: Services injected via constructor/container

**Rationale**:
- Improves testability
- Decouples components
- Simplifies configuration

**Implementation**:
```csharp
var services = new ServiceCollection();
services.AddSingleton<IGameService, GameService>();
services.AddSingleton<IFileService, FileService>();
```

---

## Debugging

### Visual Studio Debugging

1. **Set Breakpoints**: Click on line number in editor
2. **Start Debugging**: Press `F5` or `Debug > Start Debugging`
3. **Step Commands**:
   - `F10` - Step over
   - `F11` - Step into
   - `Shift+F11` - Step out
4. **Watch Variables**: Add to Watch window
5. **Locals Window**: View all local variables

### F# Debugging Tips

- **Pattern Matching**: Hover over patterns to see matched values
- **Pipe Chains**: Break complex pipes into intermediate variables
- **Recursion**: Use call stack to trace recursive calls
- **Type Inference**: Check inferred types in tooltip

### Logging

```fsharp
// In F# modules
let logger = Logger.create "GameEngine"

let makeMove board move =
    logger.Debug $"Making move from {move.From} to {move.To}"
    // ... implementation
    logger.Info "Move completed successfully"
```

```csharp
// In C# classes
private readonly ILogger<GameViewModel> _logger;

public void MakeMove(Move move)
{
    _logger.LogDebug("Making move: {move}", move);
    // ... implementation
}
```

### Performance Profiling

1. **Enable Profiler**: Debug menu > Performance Profiler
2. **Collect Data**: Run specific operations
3. **Analyze**: Identify hotspots and allocations
4. **Focus**: Optimize worst offenders first

---

## Performance Optimization

### Key Performance Areas

#### 1. AI Move Calculation
- **Metric**: Seconds per move decision
- **Current**: <1s for 8-ply search
- **Target**: Responsive play (< 2s)

**Optimization Strategies**:
```fsharp
// Use memoization for repeated evaluations
let mutable memo = Map.empty

let evaluateMemoized board =
    match Map.tryFind board memo with
    | Some value -> value
    | None ->
        let value = evaluateBoard board
        memo <- Map.add board value memo
        value
```

#### 2. Board State Representation
- **Current**: Record with nested lists
- **Consider**: Bit manipulation for large boards
- **Trade-off**: Memory vs. cache efficiency

#### 3. Move Generation
- **Cache** available moves when possible
- **Validate** moves early to fail fast
- **Avoid** generating impossible moves

### Memory Usage

```fsharp
// Prefer immutable structures with structural sharing
let optimizedUpdate board move =
    { board with 
        cells = Array.copy board.cells
    } // Array.copy shares data with parent

// Avoid unnecessary allocations
let inline isEmpty (cell: Cell) = cell = Cell.Empty
```

### Benchmarking

Use BenchmarkDotNet for performance testing:

```csharp
[MemoryDiagnoser]
public class GameEngineBenchmarks
{
    [Benchmark]
    public void EvaluateBoard()
    {
        var board = CreateTestBoard();
        var result = GameEngine.EvaluateBoard(board);
    }
}
```

Run: `dotnet run -c Release -- --job short`

### Profiling Checklist

- [ ] Identify bottleneck operations
- [ ] Measure current performance baseline
- [ ] Apply optimization strategy
- [ ] Re-measure performance
- [ ] Document changes and rationale
- [ ] Ensure functionality remains correct

---

## Contributing Guidelines

### Before You Start

1. **Check Issues**: Look for existing issues or open a new one
2. **Discuss Approach**: Comment on issue with your planned approach
3. **Get Approval**: Ensure maintainers approve before major work

### Code Quality Standards

#### C# Code Style
```csharp
// Use consistent naming
public class GameViewModel : ViewModelBase
{
    private readonly IGameService _gameService;
    
    // Property naming
    public Board CurrentBoard { get; set; }
    
    // Method naming
    public void MakeMove(Move move)
    {
        _gameService.ExecuteMove(move);
    }
}
```

#### F# Code Style
```fsharp
// Use meaningful names
let evaluateBoard (board: Board) : int = ...

// Pattern matching over conditionals
match gameState with
| GameOver winner -> printfn "Winner: %A" winner
| InProgress -> executeNextTurn ()

// Use module organization
module Board =
    let width board = board.Width
    let height board = board.Height
```

### Testing Requirements

- **Unit Tests**: Required for all core logic
- **Coverage Goal**: >80% for F# core
- **Integration Tests**: For UI components
- **Test Naming**: Descriptive names explaining test purpose

### Documentation Requirements

- **XML Comments**: For public APIs
- **README Sections**: Update if user-facing changes
- **DEVELOPMENT.md**: Update if process changes

### Pull Request Checklist

Before submitting a PR, ensure:

- [ ] Code follows style guidelines
- [ ] All tests pass locally (`dotnet test`)
- [ ] New tests added for new functionality
- [ ] No console warnings or errors
- [ ] Commits follow convention
- [ ] PR description is clear and complete
- [ ] Related issues are referenced
- [ ] Documentation is updated
- [ ] No breaking changes (or clearly documented)

### Code Review Process

**Reviewers will check**:
1. **Correctness**: Does it solve the issue?
2. **Design**: Is architecture sound?
3. **Tests**: Are tests adequate?
4. **Style**: Does it follow guidelines?
5. **Performance**: Any obvious regressions?
6. **Documentation**: Is it clear?

**Expectations**:
- Respond to reviews promptly
- Ask clarifying questions if feedback unclear
- Engage respectfully and constructively
- Push back on feedback you disagree with (with reasoning)

### Commit Best Practices

```bash
# Keep commits atomic and logical
git commit -m "feat(ai): add alpha-beta pruning"

# Don't mix refactoring with functionality
git commit -m "refactor: extract MoveValidator class"

# Rewrite history if needed before merging
git rebase -i origin/develop
```

### Release Process

1. **Update Version**: Bump version in .csproj files
2. **Update CHANGELOG**: Document new features/fixes
3. **Create Release Branch**: `release/v1.0.0`
4. **Create Release Tag**: `git tag v1.0.0`
5. **Merge to main**: PR from release branch
6. **Create Release Notes**: On GitHub releases page

---

## Additional Resources

### Documentation
- [F# Language Reference](https://docs.microsoft.com/en-us/dotnet/fsharp/)
- [C# Language Reference](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)

### Learning Resources
- Minimax Algorithm: [Wikipedia](https://en.wikipedia.org/wiki/Minimax)
- Dots and Boxes Strategy: [Game Theory Analysis](https://en.wikipedia.org/wiki/Dots_and_Boxes)

### Tools
- [ReSharper](https://www.jetbrains.com/resharper/) - Enhanced C# analysis
- [F# Formatter](https://github.com/fsprojects/fantomas) - F# code formatting
- [BenchmarkDotNet](https://benchmarkdotnet.org/) - Micro-benchmarking

---

## Getting Help

- **Questions**: Open a GitHub Discussion
- **Bugs**: File an issue with steps to reproduce
- **Suggestions**: Open an issue with detailed proposal
- **Direct Help**: Contact maintainers @dminra

---

**Last Updated**: 2026-01-04

**Document Version**: 1.0

For feedback or updates to this guide, please open an issue or submit a pull request.
