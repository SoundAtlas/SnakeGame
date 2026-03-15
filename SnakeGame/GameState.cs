namespace SnakeGame
{
    public class GameState
    {
        // Grid size
        public int Rows { get; }
        public int Columns { get; }

        // 2D array representing the game board
        public GridValue[,] Grid { get; }


        public Direction Dir { get; private set; }


        public int Score { get; private set; }

        public bool IsGameOver { get; private set; }
        // Stores upcoming direction changes from player input
        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();

        // Stores the positions of the snake body
        private readonly LinkedList<GridPosition> snakePositions = new LinkedList<GridPosition>();

        private readonly Random random = new Random();

        // Constructor: sets up the grid and starts the game
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
            // Start snake in the middle row
            int r = Rows / 2;

            // Create snake with starting length of 3
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new GridPosition(r, c));
            }
        }

        // Find all empty cells in the grid
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

        // Place food on a random empty tile
        private void AddFood()
        {
            List<GridPosition> emptyPositions = new List<GridPosition>(EmptyPositions());

            if (emptyPositions.Count == 0)
            {
                // No space left -> player wins
                IsGameOver = true;
                return;
            }

            GridPosition position = emptyPositions[random.Next(emptyPositions.Count)];
            Grid[position.Row, position.Column] = GridValue.Food;
        }

        // Get current snake head
        public GridPosition HeadPosition()
        {
            return snakePositions.First.Value;
        }

        // Get current snake tail
        public GridPosition TailPosition()
        {
            return snakePositions.Last.Value;
        }

        // Return all snake body positions
        public IEnumerable<GridPosition> SnakePositions()
        {
            return snakePositions;
        }

        // Add a new head when the snake moves
        private void AddHead(GridPosition newHead)
        {
            snakePositions.AddFirst(newHead);
            Grid[newHead.Row, newHead.Column] = GridValue.Snake;
        }

        // Remove the tail when the snake moves without eating
        private void RemoveTail()
        {
            GridPosition tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        // Get the most recent direction change
        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }

            return dirChanges.Last.Value;
        }

        // Check if the snake is allowed to change direction
        private bool CanChangeDirection(Direction newdirection)
        {
            // Limit queued direction changes
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();

            // Prevent reversing directly into itself
            return newdirection != lastDir && newdirection != lastDir.Opposite();
        }

        // Called when the player presses a movement key
        public void ChangeDirection(Direction direction)
        {
            if (CanChangeDirection(direction))
            {
                dirChanges.AddLast(direction);
            }
        }

        // Check if position is outside the grid
        private bool OutsideGrid(GridPosition position)
        {
            return position.Row < 0 || position.Row >= Rows || position.Column < 0 || position.Column >= Columns;
        }

        // Determine what the snake will collide with next
        private GridValue WillHit(GridPosition newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }

            // Moving into the tail is allowed (it moves away)
            if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Column];
        }

        // Moves the snake one step forward
        public void Move()
        {
            // Apply queued direction change
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            // Calculate next head position
            GridPosition newHeadPos = HeadPosition().NewPosition(Dir);

            // Check what we will hit
            GridValue hitValue = WillHit(newHeadPos);

            if (hitValue == GridValue.Outside || hitValue == GridValue.Snake)
            {
                // Hit wall or body -> game over
                IsGameOver = true;
            }
            else if (hitValue == GridValue.Empty)
            {
                // Normal move
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hitValue == GridValue.Food)
            {
                // Eat food and grow
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}