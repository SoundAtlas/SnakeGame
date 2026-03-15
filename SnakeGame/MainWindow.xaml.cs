using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SnakeGame
{
    /// <summary>
    /// Main game window
    /// </summary>
    public partial class MainWindow : Window
    {
        // Maps each grid value (Empty, Snake, Food) to its image
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food  },
        };

        // Maps direction to rotations for eyes on head
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 },
        };

        // Grid size
        private readonly int rows = 15, cols = 15;

        // Stores the Image controls for each grid cell
        private readonly Image[,] gridImages;

        // Holds the game logic/state
        private GameState gameState;

        // Prevents multiple games from running at once
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();

            // Create visual grid
            gridImages = SetupGrid();

            // Create new game state
            gameState = new GameState(rows, cols);
        }

        // Starts and runs a full game
        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();

            // Hide overlay before game starts
            Overlay.Visibility = Visibility.Hidden;

            // Run main game loop
            await GameLoop();

            // Show game over screen
            await ShowGameOver();

            // Reset game
            gameState = new GameState(rows, cols);
        }

        // Starts the game when a key is pressed
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Prevent input when overlay is visible
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            // Start game if not already running
            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        // Handles snake movement input
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.IsGameOver)
                return;

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;

                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;

                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;

                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        // Main game loop: moves the snake repeatedly
        private async Task GameLoop()
        {
            while (!gameState.IsGameOver)
            {
                await Task.Delay(100); // control game speed
                gameState.Move();
                Draw();
            }
        }

        // Creates the visual grid of Image controls
        private Image[,] SetupGrid()
        {
            Image[,] gridImages = new Image[rows, cols];

            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    gridImages[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return gridImages;
        }

        // Updates the game visuals
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }

        // Draws the grid based on the game state
        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridValue = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridValue];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        // Draws the snakehead on first position of the snake
        private void DrawSnakeHead()
        {
            GridPosition headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Column];
            image.Source = Images.Head;

            // rotates head based on current direction
            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<GridPosition> positions = new List<GridPosition>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                GridPosition pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Column].Source = source;
                await Task.Delay(30);
            }
        }

        // Shows countdown before game starts
        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        // Shows game over message
        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(500);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}