
namespace SnakeGame
{
    public class GridPosition
    {
        public int Row { get; }
        public int Column { get; }

        public GridPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public GridPosition NewPosition(Direction direction)
        {
            return new GridPosition(Row + direction.RowOffset, Column + direction.ColumnOffset);
        }

        // Generated equality members and GetHashCode 
        public override bool Equals(object? obj)
        {
            return obj is GridPosition position &&
                   Row == position.Row &&
                   Column == position.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        public static bool operator ==(GridPosition? left, GridPosition? right)
        {
            return EqualityComparer<GridPosition>.Default.Equals(left, right);
        }

        public static bool operator !=(GridPosition? left, GridPosition? right)
        {
            return !(left == right);
        }
    }
}
