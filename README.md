# Dots and Boxes

A strategic pen-and-paper game implementation featuring AI opponents, multiplayer support, and interactive gameplay.

## Table of Contents

- [About](#about)
- [Features](#features)
- [Architecture](#architecture)
- [Setup Instructions](#setup-instructions)
- [Gameplay Rules](#gameplay-rules)
- [How to Play](#how-to-play)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## About

Dots and Boxes is a classic pencil-and-paper game that combines strategy, pattern recognition, and tactical decision-making. This project brings the traditional game to the digital realm with enhanced features including AI opponents with varying difficulty levels, multiplayer capabilities, and an intuitive user interface.

## Features

### Core Gameplay
- **Two-player gameplay**: Classic 1v1 matches
- **Single-player with AI**: Challenge computer opponents with multiple difficulty levels
  - Easy: Random move selection
  - Medium: Basic strategic heuristics
  - Hard: Advanced minimax algorithm with alpha-beta pruning
- **Interactive game board**: Dynamic grid sizing (3x3 to 10x10)
- **Real-time score tracking**: Live player scores and statistics
- **Move validation**: Automatic detection of completed boxes and score updates

### User Interface
- **Responsive design**: Works on desktop and mobile devices
- **Visual feedback**: Clear indication of valid moves and completed boxes
- **Game history**: Replay previous games and view move sequences
- **Statistics tracking**: Player performance metrics and win/loss records

### Game Management
- **Multiple game modes**: Quick play, tournament, training
- **Customizable settings**: Board size, AI difficulty, time limits
- **Save/Load functionality**: Persist game states for later resumption
- **Undo/Redo moves**: Revert actions up to game start

## Architecture

### System Design

The project follows a modular, layered architecture:

```
┌─────────────────────────────────────┐
│      Presentation Layer             │
│  (UI Components, Game Display)      │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Game Logic Layer               │
│  (Rules Engine, Move Validator)     │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      AI & Strategy Layer            │
│  (AI Players, Algorithms)           │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Data Layer                     │
│  (Game State, Persistence)          │
└─────────────────────────────────────┘
```

### Key Components

#### 1. **Game Board Manager**
- Manages the game grid state
- Tracks all placed edges and completed boxes
- Validates move legality
- Calculates scores

#### 2. **Move Validator**
- Checks move legality
- Detects box completions
- Identifies chain reactions
- Validates game state transitions

#### 3. **AI Engine**
- **Random AI**: Generates random valid moves
- **Minimax AI**: Evaluates move sequences up to depth limit
  - Uses alpha-beta pruning for optimization
  - Evaluates board state and potential future boxes
  - Considers opponent threat assessment

#### 4. **Game State Manager**
- Maintains current game state
- Handles game history and replay functionality
- Manages player turns and game flow
- Saves/loads game configurations

#### 5. **UI Controller**
- Handles user input (mouse/touch)
- Renders game board and components
- Updates display based on game events
- Manages game transitions

## Setup Instructions

### Prerequisites

- Python 3.8+ (for Python implementation)
- Node.js 14+ (for JavaScript implementation)
- Git
- pip or npm (package managers)

### Installation

#### Python Version

1. **Clone the repository**
   ```bash
   git clone https://github.com/dminra/DotsAndBoxes.git
   cd DotsAndBoxes
   ```

2. **Create a virtual environment**
   ```bash
   python -m venv venv
   source venv/bin/activate  # On Windows: venv\Scripts\activate
   ```

3. **Install dependencies**
   ```bash
   pip install -r requirements.txt
   ```

4. **Run the game**
   ```bash
   python main.py
   ```

#### JavaScript/Web Version

1. **Clone the repository**
   ```bash
   git clone https://github.com/dminra/DotsAndBoxes.git
   cd DotsAndBoxes
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Run the development server**
   ```bash
   npm run dev
   ```

4. **Build for production**
   ```bash
   npm run build
   ```

### Configuration

Create a `config.json` file in the project root:

```json
{
  "board_size": 5,
  "ai_difficulty": "medium",
  "game_mode": "vs_ai",
  "ui_theme": "dark",
  "enable_animations": true,
  "sound_enabled": true
}
```

## Gameplay Rules

### Objective

The player who completes the most boxes wins the game. A box is completed when all four of its sides have been drawn.

### Game Setup

1. The game board consists of a grid of dots arranged in rows and columns
2. Players take turns drawing a single line (edge) between two adjacent dots
3. Standard board sizes: 3x3 (6 dots per side) to 10x10 (11 dots per side)

### Turn Structure

**On each turn, a player:**
1. Draws one line connecting two adjacent dots
2. If their line completes a box, they:
   - Earn 1 point
   - Place their mark in the completed box
   - Get an additional turn
3. If their line does NOT complete a box, their turn ends and play passes to opponent

### Scoring

- **Box completion**: +1 point per completed box
- **Bonus**: Extra turn when completing a box
- **Chain completion**: Multiple boxes can be completed in a single turn if connected

### Winning Condition

The game ends when all possible lines have been drawn. The player with the highest score wins.

**Special case**: In case of a tie, the game is recorded as a draw.

### Key Strategy Elements

- **Sacrificial moves**: Avoid leaving opportunities for opponent to complete multiple boxes
- **Chain control**: Try to set up scenarios where completing one box enables completing others
- **Box counting**: Track open boxes and prioritize based on completion potential
- **Threat assessment**: Monitor positions where opponent could gain multiple points

## How to Play

### Starting a Game

1. **Launch the application**
2. **Select game mode**:
   - Single Player (vs AI)
   - Two Player (local)
   - Online Multiplayer

3. **Configure settings**:
   - Choose board size (3x3 to 10x10)
   - Select AI difficulty (if applicable)
   - Set time limits (optional)

### During Gameplay

1. **View the board**: See all dots, edges, and completed boxes
2. **Make a move**: Click or tap on an edge between two dots
3. **Watch for feedback**:
   - Valid move: Edge turns to your color
   - Box completion: Box fills with your color, score updates
   - Extra turn: Turn indicator shows you play again

4. **Switch turns**: When you pass, opponent makes their move

### Game State Indicators

- **Player scores**: Displayed prominently at top of screen
- **Current turn**: Highlighted player name
- **Available moves**: Clickable edges are highlighted on hover
- **Completed boxes**: Marked with player color and number

### Advanced Features

#### Undo/Redo
- Press `Ctrl+Z` (or `Cmd+Z` on Mac) to undo last move
- Press `Ctrl+Y` (or `Cmd+Y` on Mac) to redo

#### Game Menu
- **New Game**: Start fresh game
- **Save Game**: Store current game state
- **Load Game**: Resume saved game
- **Settings**: Adjust preferences
- **Statistics**: View performance history
- **Help**: Display rules and tips

## Project Structure

```
DotsAndBoxes/
├── README.md                 # This file
├── requirements.txt          # Python dependencies
├── package.json             # JavaScript dependencies
├── config.json              # Default configuration
├── main.py                  # Python entry point
├── index.html               # Web entry point
│
├── src/
│   ├── game/
│   │   ├── board.py        # Game board logic
│   │   ├── player.py       # Player management
│   │   ├── rules.py        # Game rules engine
│   │   └── game.py         # Main game controller
│   │
│   ├── ai/
│   │   ├── ai_player.py    # AI player base class
│   │   ├── random_ai.py    # Random strategy AI
│   │   ├── minimax_ai.py   # Minimax algorithm implementation
│   │   └── evaluator.py    # Board state evaluation
│   │
│   ├── ui/
│   │   ├── game_ui.py      # Main UI controller
│   │   ├── components.py   # UI components
│   │   └── renderer.py     # Board rendering
│   │
│   ├── utils/
│   │   ├── config.py       # Configuration management
│   │   ├── logger.py       # Logging utilities
│   │   └── helpers.py      # Helper functions
│   │
│   └── data/
│       ├── game_state.py   # Game state management
│       └── persistence.py  # Save/load functionality
│
├── tests/
│   ├── test_board.py       # Board logic tests
│   ├── test_rules.py       # Rules engine tests
│   ├── test_ai.py          # AI strategy tests
│   └── test_game.py        # Game flow tests
│
├── docs/
│   ├── architecture.md     # Detailed architecture docs
│   ├── api.md              # API documentation
│   └── ai_strategies.md    # AI algorithm documentation
│
└── assets/
    ├── images/            # Game images and icons
    ├── sounds/            # Audio files
    └── styles/            # CSS stylesheets
```

## Contributing

We welcome contributions! Please follow these steps:

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make your changes** with clear commit messages
4. **Add tests** for new functionality
5. **Submit a pull request** with a detailed description

### Code Standards

- Follow PEP 8 (Python) or ESLint (JavaScript) guidelines
- Write clear, documented code with docstrings
- Include unit tests for new features
- Keep commits atomic and well-described

### Testing

Run tests before submitting:

```bash
# Python
python -m pytest tests/ -v

# JavaScript
npm test
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Classic Dots and Boxes game concept
- Contributors and testers
- Community feedback and suggestions

## Support

For issues, questions, or suggestions:
- **GitHub Issues**: Open an issue on the repository
- **Discussions**: Participate in project discussions
- **Email**: Contact the project maintainer

---

**Last Updated**: January 4, 2026

Enjoy the game and happy coding!
