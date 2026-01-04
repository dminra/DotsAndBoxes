namespace DotsAndBoxes.Logic

/// Represents a player in the game
type Player =
    | Human
    | AI

/// Represents the state of a cell (box) in the grid
type CellState = {
    TopEdge: bool
    RightEdge: bool
    BottomEdge: bool
    LeftEdge: bool
    Owner: Player option
}

/// Represents the complete game state
type GameState = {
    Grid: CellState[,]
    Rows: int
    Cols: int
    HumanScore: int
    AIScore: int
    CurrentPlayer: Player
    GameOver: bool
}

/// Represents a move (edge placement)
type Move = {
    Row: int
    Col: int
    EdgeType: string // "top", "right", "bottom", "left"
}

module GameLogic =

    /// Creates an initial game state with given dimensions
    let createGame (rows: int) (cols: int) : GameState =
        let grid = Array2D.init rows cols (fun _ _ ->
            {
                TopEdge = false
                RightEdge = false
                BottomEdge = false
                LeftEdge = false
                Owner = None
            })
        {
            Grid = grid
            Rows = rows
            Cols = cols
            HumanScore = 0
            AIScore = 0
            CurrentPlayer = Human
            GameOver = false
        }

    /// Validates if a move is legal (edge hasn't been placed yet)
    let isValidMove (state: GameState) (move: Move) : bool =
        let { Row = row; Col = col; EdgeType = edgeType } = move
        if row < 0 || row >= state.Rows || col < 0 || col >= state.Cols then
            false
        else
            let cell = state.Grid.[row, col]
            match edgeType with
            | "top" -> not cell.TopEdge
            | "right" -> not cell.RightEdge
            | "bottom" -> not cell.BottomEdge
            | "left" -> not cell.LeftEdge
            | _ -> false

    /// Places an edge on the grid
    let private placeEdge (grid: CellState[,]) (move: Move) : unit =
        let { Row = row; Col = col; EdgeType = edgeType } = move
        let cell = grid.[row, col]
        match edgeType with
        | "top" -> grid.[row, col] <- { cell with TopEdge = true }
        | "right" -> grid.[row, col] <- { cell with RightEdge = true }
        | "bottom" -> grid.[row, col] <- { cell with BottomEdge = true }
        | "left" -> grid.[row, col] <- { cell with LeftEdge = true }
        | _ -> ()

        // Also update the adjacent cell's corresponding edge
        match edgeType with
        | "top" when row > 0 ->
            let adjacentCell = grid.[row - 1, col]
            grid.[row - 1, col] <- { adjacentCell with BottomEdge = true }
        | "bottom" when row < grid.GetLength(0) - 1 ->
            let adjacentCell = grid.[row + 1, col]
            grid.[row + 1, col] <- { adjacentCell with TopEdge = true }
        | "left" when col > 0 ->
            let adjacentCell = grid.[row, col - 1]
            grid.[row, col - 1] <- { adjacentCell with RightEdge = true }
        | "right" when col < grid.GetLength(1) - 1 ->
            let adjacentCell = grid.[row, col + 1]
            grid.[row, col + 1] <- { adjacentCell with LeftEdge = true }
        | _ -> ()

    /// Checks if a cell is completely surrounded and fills it if necessary
    let private isCellComplete (grid: CellState[,]) (row: int) (col: int) : bool =
        let cell = grid.[row, col]
        cell.TopEdge && cell.RightEdge && cell.BottomEdge && cell.LeftEdge

    /// Checks and fills squares after a move, returns the player who gets points
    let checkAndFillSquares (grid: CellState[,]) (player: Player) : int =
        let rows = grid.GetLength(0)
        let cols = grid.GetLength(1)
        let mutable pointsScored = 0

        for row in 0 .. rows - 1 do
            for col in 0 .. cols - 1 do
                if isCellComplete grid row col && grid.[row, col].Owner.IsNone then
                    grid.[row, col] <- { grid.[row, col] with Owner = Some player }
                    pointsScored <- pointsScored + 1

        pointsScored

    /// Makes a move and updates the game state
    let makeMove (state: GameState) (move: Move) : GameState =
        if not (isValidMove state move) then
            state
        else
            let newGrid = Array2D.copy state.Grid
            placeEdge newGrid move

            let pointsScored = checkAndFillSquares newGrid state.CurrentPlayer

            let newHumanScore, newAIScore =
                match state.CurrentPlayer with
                | Human -> state.HumanScore + pointsScored, state.AIScore
                | AI -> state.HumanScore, state.AIScore + pointsScored

            // If no points were scored, switch player
            let newCurrentPlayer =
                if pointsScored > 0 then state.CurrentPlayer else switchPlayer state.CurrentPlayer

            let gameOver = isGameComplete newGrid state.Rows state.Cols

            {
                Grid = newGrid
                Rows = state.Rows
                Cols = state.Cols
                HumanScore = newHumanScore
                AIScore = newAIScore
                CurrentPlayer = newCurrentPlayer
                GameOver = gameOver
            }

    /// Gets all possible moves from the current game state
    let getAllPossibleMoves (state: GameState) : Move list =
        let moves = System.Collections.Generic.List<Move>()

        for row in 0 .. state.Rows - 1 do
            for col in 0 .. state.Cols - 1 do
                let cell = state.Grid.[row, col]

                if not cell.TopEdge then
                    moves.Add({ Row = row; Col = col; EdgeType = "top" })

                if not cell.RightEdge then
                    moves.Add({ Row = row; Col = col; EdgeType = "right" })

                if not cell.BottomEdge then
                    moves.Add({ Row = row; Col = col; EdgeType = "bottom" })

                if not cell.LeftEdge then
                    moves.Add({ Row = row; Col = col; EdgeType = "left" })

        List.ofSeq moves

    /// Counts how many potential squares would be completed by a move
    let countPotentialSquares (state: GameState) (move: Move) : int =
        let testState = makeMove state move
        let pointsEarned =
            match state.CurrentPlayer with
            | Human -> testState.HumanScore - state.HumanScore
            | AI -> testState.AIScore - state.AIScore

        pointsEarned

    /// Evaluates a move for AI decision making
    let private evaluateMove (state: GameState) (move: Move) : int =
        let pointsFromMove = countPotentialSquares state move
        let testState = makeMove state move

        // Prioritize moves that complete squares
        let moveValue = pointsFromMove * 10

        // Penalize moves that give opponent opportunities
        let opponentPotentialMoves = getAllPossibleMoves testState
        let opponentOpportunities =
            opponentPotentialMoves
            |> List.fold (fun acc m -> acc + (countPotentialSquares testState m)) 0

        moveValue - (opponentOpportunities / 2)

    /// Gets the best move for the AI using a simple greedy strategy
    let getAIMove (state: GameState) : Move option =
        let possibleMoves = getAllPossibleMoves state

        if List.isEmpty possibleMoves then
            None
        else
            let bestMove =
                possibleMoves
                |> List.maxBy (fun move -> evaluateMove state move)

            Some bestMove

    /// Switches the current player
    let switchPlayer (player: Player) : Player =
        match player with
        | Human -> AI
        | AI -> Human

    /// Checks if the game is complete (all edges placed)
    let isGameComplete (grid: CellState[,]) (rows: int) (cols: int) : bool =
        let mutable allEdgesPlaced = true

        for row in 0 .. rows - 1 do
            for col in 0 .. cols - 1 do
                let cell = grid.[row, col]
                if not (cell.TopEdge && cell.RightEdge && cell.BottomEdge && cell.LeftEdge) then
                    allEdgesPlaced <- false

        allEdgesPlaced

    /// Gets the final scores when game is complete
    let getFinalScores (state: GameState) : int * int =
        state.HumanScore, state.AIScore

    /// Gets the game result
    let getGameResult (state: GameState) : string =
        if state.HumanScore > state.AIScore then
            "Human wins!"
        elif state.AIScore > state.HumanScore then
            "AI wins!"
        else
            "It's a tie!"
