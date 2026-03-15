namespace SnakeGame
{
    public class GameState
    {
        public int Rows { get; }
        public int Columns { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }

        private readonly LinkedList<GridPosition> snakePositions = new LinkedList<GridPosition>();
        private readonly Random random = new Random();

        //constructor
        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[rows, columns];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            // Start snake in the middle of the grid
            int r = Rows / 2;

            // Start with a length of 3
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new GridPosition(r, c));
            }
        }

        // Get all empty positions on the grid
        private IEnumerable<GridPosition> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new GridPosition(r, c);
                    }
                }
            }
        }

        // Add food to a random empty position
        private void AddFood()
        {
            List<GridPosition> emptyPositions = new List<GridPosition>(EmptyPositions());

            if (emptyPositions.Count == 0)
            {
                // No empty space left, player wins
                IsGameOver = true;
                return;
            }
            // Select a random empty position for the food
            GridPosition position = emptyPositions[random.Next(emptyPositions.Count)];
            Grid[position.Row, position.Column] = GridValue.Food;
        }
    }
}
