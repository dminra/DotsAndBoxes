using System;
using System.Drawing;
using System.Windows.Forms;

namespace DotsAndBoxes.UI
{
    public partial class MainForm : Form
    {
        private const int DotSize = 8;
        private const int CellSize = 40;
        private const int Margin = 40;
        
        private int gridSize = 5;
        private GameBoard gameBoard;
        private Brush player1Brush = Brushes.Blue;
        private Brush player2Brush = Brushes.Red;
        private Pen linePen = new Pen(Color.Black, 2);
        private Pen selectedLinePen = new Pen(Color.Green, 3);
        
        private int player1Score = 0;
        private int player2Score = 0;
        private bool isPlayer1Turn = true;

        public MainForm()
        {
            InitializeComponent();
            InitializeGame();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Dots and Boxes";
            this.Size = new Size(800, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.Paint += MainForm_Paint;
            this.MouseClick += MainForm_MouseClick;
            
            CreateMenuBar();
            CreateStatusBar();
        }

        private void CreateMenuBar()
        {
            MenuStrip menuStrip = new MenuStrip();
            
            // File menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("&File");
            ToolStripMenuItem newGameItem = new ToolStripMenuItem("&New Game", null, NewGame_Click);
            ToolStripMenuItem exitItem = new ToolStripMenuItem("E&xit", null, Exit_Click);
            fileMenu.DropDownItems.AddRange(new ToolStripMenuItem[] { newGameItem, exitItem });
            
            // Game menu
            ToolStripMenuItem gameMenu = new ToolStripMenuItem("&Game");
            ToolStripMenuItem gridSizeItem = new ToolStripMenuItem("&Grid Size");
            for (int i = 3; i <= 8; i++)
            {
                int size = i;
                ToolStripMenuItem sizeItem = new ToolStripMenuItem($"{i}x{i}", null, (s, e) => SetGridSize(size));
                gridSizeItem.DropDownItems.Add(sizeItem);
            }
            gameMenu.DropDownItems.Add(gridSizeItem);
            
            menuStrip.Items.AddRange(new ToolStripMenuItem[] { fileMenu, gameMenu });
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private void CreateStatusBar()
        {
            StatusStrip statusStrip = new StatusStrip();
            ToolStripStatusLabel statusLabel = new ToolStripStatusLabel("Player 1 (Blue): 0 | Player 2 (Red): 0 | Current: Player 1");
            statusLabel.Name = "StatusLabel";
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        private void InitializeGame()
        {
            gameBoard = new GameBoard(gridSize);
            player1Score = 0;
            player2Score = 0;
            isPlayer1Turn = true;
        }

        private void SetGridSize(int size)
        {
            gridSize = size;
            InitializeGame();
            this.Invalidate();
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            InitializeGame();
            this.Invalidate();
            UpdateStatusBar();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            DrawBoard(e.Graphics);
            DrawDots(e.Graphics);
            DrawLines(e.Graphics);
            DrawScores(e.Graphics);
        }

        private void DrawBoard(Graphics g)
        {
            int boardWidth = gridSize * CellSize;
            int boardHeight = gridSize * CellSize;
            
            Rectangle boardRect = new Rectangle(Margin, Margin + 30, boardWidth, boardHeight);
            g.DrawRectangle(Pens.Black, boardRect);
        }

        private void DrawDots(Graphics g)
        {
            for (int i = 0; i <= gridSize; i++)
            {
                for (int j = 0; j <= gridSize; j++)
                {
                    int x = Margin + j * CellSize;
                    int y = Margin + 30 + i * CellSize;
                    
                    g.FillEllipse(Brushes.Black, x - DotSize / 2, y - DotSize / 2, DotSize, DotSize);
                }
            }
        }

        private void DrawLines(Graphics g)
        {
            // Draw horizontal lines
            for (int i = 0; i <= gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    int x1 = Margin + j * CellSize;
                    int y = Margin + 30 + i * CellSize;
                    int x2 = Margin + (j + 1) * CellSize;
                    
                    Pen pen = linePen;
                    if (gameBoard.IsHorizontalLineOwned(i, j))
                    {
                        pen = gameBoard.GetHorizontalLineOwner(i, j) == 1 ? new Pen(Color.Blue, 3) : new Pen(Color.Red, 3);
                    }
                    
                    g.DrawLine(pen, x1, y, x2, y);
                }
            }
            
            // Draw vertical lines
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j <= gridSize; j++)
                {
                    int x = Margin + j * CellSize;
                    int y1 = Margin + 30 + i * CellSize;
                    int y2 = Margin + 30 + (i + 1) * CellSize;
                    
                    Pen pen = linePen;
                    if (gameBoard.IsVerticalLineOwned(i, j))
                    {
                        pen = gameBoard.GetVerticalLineOwner(i, j) == 1 ? new Pen(Color.Blue, 3) : new Pen(Color.Red, 3);
                    }
                    
                    g.DrawLine(pen, x, y1, x, y2);
                }
            }
        }

        private void DrawScores(Graphics g)
        {
            Font font = new Font("Arial", 14, FontStyle.Bold);
            int baseY = Margin + 30 + gridSize * CellSize + 30;
            
            g.DrawString($"Player 1 (Blue): {player1Score}", font, player1Brush, new PointF(Margin, baseY));
            g.DrawString($"Player 2 (Red): {player2Score}", font, player2Brush, new PointF(Margin + 300, baseY));
            
            string currentPlayer = isPlayer1Turn ? "Player 1's Turn" : "Player 2's Turn";
            g.DrawString(currentPlayer, font, isPlayer1Turn ? player1Brush : player2Brush, new PointF(Margin, baseY + 40));
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X - Margin;
            int y = e.Y - Margin - 30;
            
            // Check if click is on a horizontal line
            for (int i = 0; i <= gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    int lineX1 = j * CellSize;
                    int lineY = i * CellSize;
                    int lineX2 = (j + 1) * CellSize;
                    
                    if (IsNearHorizontalLine(x, y, lineX1, lineY, lineX2) && !gameBoard.IsHorizontalLineOwned(i, j))
                    {
                        PlaceHorizontalLine(i, j);
                        return;
                    }
                }
            }
            
            // Check if click is on a vertical line
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j <= gridSize; j++)
                {
                    int lineX = j * CellSize;
                    int lineY1 = i * CellSize;
                    int lineY2 = (i + 1) * CellSize;
                    
                    if (IsNearVerticalLine(x, y, lineX, lineY1, lineY2) && !gameBoard.IsVerticalLineOwned(i, j))
                    {
                        PlaceVerticalLine(i, j);
                        return;
                    }
                }
            }
        }

        private bool IsNearHorizontalLine(int x, int y, int x1, int lineY, int x2)
        {
            return x >= x1 - 5 && x <= x2 + 5 && y >= lineY - 5 && y <= lineY + 5;
        }

        private bool IsNearVerticalLine(int x, int y, int lineX, int y1, int y2)
        {
            return x >= lineX - 5 && x <= lineX + 5 && y >= y1 - 5 && y <= y2 + 5;
        }

        private void PlaceHorizontalLine(int row, int col)
        {
            gameBoard.PlaceHorizontalLine(row, col, isPlayer1Turn ? 1 : 2);
            
            int boxesCompleted = CheckCompletedBoxes();
            if (boxesCompleted == 0)
            {
                isPlayer1Turn = !isPlayer1Turn;
            }
            
            UpdateStatusBar();
            this.Invalidate();
        }

        private void PlaceVerticalLine(int row, int col)
        {
            gameBoard.PlaceVerticalLine(row, col, isPlayer1Turn ? 1 : 2);
            
            int boxesCompleted = CheckCompletedBoxes();
            if (boxesCompleted == 0)
            {
                isPlayer1Turn = !isPlayer1Turn;
            }
            
            UpdateStatusBar();
            this.Invalidate();
        }

        private int CheckCompletedBoxes()
        {
            int boxesCompleted = 0;
            int currentPlayer = isPlayer1Turn ? 1 : 2;
            
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (gameBoard.IsBoxComplete(i, j) && gameBoard.GetBoxOwner(i, j) == 0)
                    {
                        gameBoard.SetBoxOwner(i, j, currentPlayer);
                        boxesCompleted++;
                        
                        if (currentPlayer == 1)
                            player1Score++;
                        else
                            player2Score++;
                    }
                }
            }
            
            return boxesCompleted;
        }

        private void UpdateStatusBar()
        {
            ToolStripStatusLabel label = this.FindForm()?.Controls.Find("StatusLabel", true).Length > 0 
                ? (ToolStripStatusLabel)this.Controls.Find("StatusLabel", true)[0] 
                : null;
            
            if (label != null)
            {
                string currentPlayer = isPlayer1Turn ? "Player 1" : "Player 2";
                label.Text = $"Player 1 (Blue): {player1Score} | Player 2 (Red): {player2Score} | Current: {currentPlayer}";
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }

    public class GameBoard
    {
        private int gridSize;
        private bool[,] horizontalLines;
        private bool[,] verticalLines;
        private int[,] boxOwners;
        private int[] horizontalLineOwners;
        private int[] verticalLineOwners;

        public GameBoard(int size)
        {
            gridSize = size;
            horizontalLines = new bool[(size + 1) * size, 2];
            verticalLines = new bool[size * (size + 1), 2];
            boxOwners = new int[size, size];
            horizontalLineOwners = new int[(size + 1) * size];
            verticalLineOwners = new int[size * (size + 1)];
        }

        public void PlaceHorizontalLine(int row, int col, int player)
        {
            horizontalLines[row * gridSize + col, 0] = true;
            horizontalLineOwners[row * gridSize + col] = player;
        }

        public void PlaceVerticalLine(int row, int col, int player)
        {
            verticalLines[row * gridSize + col, 0] = true;
            verticalLineOwners[row * gridSize + col] = player;
        }

        public bool IsHorizontalLineOwned(int row, int col)
        {
            return horizontalLines[row * gridSize + col, 0];
        }

        public bool IsVerticalLineOwned(int row, int col)
        {
            return verticalLines[row * gridSize + col, 0];
        }

        public int GetHorizontalLineOwner(int row, int col)
        {
            return horizontalLineOwners[row * gridSize + col];
        }

        public int GetVerticalLineOwner(int row, int col)
        {
            return verticalLineOwners[row * gridSize + col];
        }

        public bool IsBoxComplete(int row, int col)
        {
            return IsHorizontalLineOwned(row, col) && 
                   IsHorizontalLineOwned(row + 1, col) && 
                   IsVerticalLineOwned(row, col) && 
                   IsVerticalLineOwned(row, col + 1);
        }

        public void SetBoxOwner(int row, int col, int player)
        {
            boxOwners[row, col] = player;
        }

        public int GetBoxOwner(int row, int col)
        {
            return boxOwners[row, col];
        }
    }
}
