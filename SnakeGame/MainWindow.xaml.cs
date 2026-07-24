using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SnakeGame;

public partial class MainWindow : Window
{
    private const int CellSize = 20;
    private readonly List<Point> snake = new();

    public MainWindow()
    {
        InitializeComponent();
        DrawWelcomeScreen();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        snake.Clear();
        snake.Add(new Point(10, 12));
        snake.Add(new Point(9, 12));
        snake.Add(new Point(8, 12));

        MessageText.Text = "Đã tạo con rắn gồm 3 ô.";
        DrawSnake();
    }

    private void DrawSnake()
    {
        GameCanvas.Children.Clear();

        for (int index = 0; index < snake.Count; index++)
        {
            Point snakePart = snake[index];
            Brush color = index == 0 ? Brushes.LimeGreen : Brushes.ForestGreen;
            DrawCell(snakePart, color);
        }
    }

    private void DrawCell(Point position, Brush color)
    {
        System.Windows.Shapes.Rectangle rectangle = new()
        {
            Width = CellSize - 2,
            Height = CellSize - 2,
            Fill = color,
            RadiusX = 3,
            RadiusY = 3
        };

        Canvas.SetLeft(rectangle, position.X * CellSize + 1);
        Canvas.SetTop(rectangle, position.Y * CellSize + 1);
        GameCanvas.Children.Add(rectangle);
    }

    private void DrawWelcomeScreen()
    {
        TextBlock welcomeText = new()
        {
            Text = "SNAKE",
            Foreground = Brushes.ForestGreen,
            FontSize = 72,
            FontWeight = FontWeights.Bold
        };

        Canvas.SetLeft(welcomeText, 175);
        Canvas.SetTop(welcomeText, 190);
        GameCanvas.Children.Add(welcomeText);
    }
}
