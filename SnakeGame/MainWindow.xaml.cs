using System.Windows;
using System.Windows.Controls;


namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
        }

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
                        Source = Images.Empty
                    };

                    gridImages[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return gridImages;
        }
    }
}