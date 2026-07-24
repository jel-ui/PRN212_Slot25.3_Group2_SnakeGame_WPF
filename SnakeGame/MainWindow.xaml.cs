using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SnakeGame;

public partial class MainWindow : Window
{
    private const int CellSize = 20;
    private const int NumberOfColumns = 30;
    private const int NumberOfRows = 25;

    private readonly List<Point> snake = new();
    private readonly DispatcherTimer gameTimer = new();

    private Direction currentDirection = Direction.Right;
    private Direction nextDirection = Direction.Right;
    private bool isGameRunning;

    public MainWindow()
    {
        InitializeComponent();

        gameTimer.Interval = TimeSpan.FromMilliseconds(130);
        gameTimer.Tick += GameTimer_Tick;

        DrawWelcomeScreen();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        gameTimer.Stop();

        snake.Clear();
        snake.Add(new Point(10, 12));
        snake.Add(new Point(9, 12));
        snake.Add(new Point(8, 12));

        currentDirection = Direction.Right;
        nextDirection = Direction.Right;
        isGameRunning = true;

        MessageText.Text = "Điều khiển bằng phím mũi tên hoặc W, A, S, D.";
        DrawSnake();

        gameTimer.Start();
        Keyboard.Focus(this);
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        currentDirection = nextDirection;

        Point newHead = GetNewHeadPosition(snake[0]);
        snake.Insert(0, newHead);
        snake.RemoveAt(snake.Count - 1);

        DrawSnake();
    }

    private Point GetNewHeadPosition(Point currentHead)
    {
        double newX = currentHead.X;
        double newY = currentHead.Y;

        if (currentDirection == Direction.Up)
        {
            newY--;
        }
        else if (currentDirection == Direction.Down)
        {
            newY++;
        }
        else if (currentDirection == Direction.Left)
        {
            newX--;
        }
        else if (currentDirection == Direction.Right)
        {
            newX++;
        }

        // Ở phiên bản chuyển động đầu tiên, rắn đi xuyên qua mép bàn.
        if (newX < 0)
        {
            newX = NumberOfColumns - 1;
        }
        else if (newX >= NumberOfColumns)
        {
            newX = 0;
        }

        if (newY < 0)
        {
            newY = NumberOfRows - 1;
        }
        else if (newY >= NumberOfRows)
        {
            newY = 0;
        }

        return new Point(newX, newY);
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

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (!isGameRunning)
        {
            return;
        }

        if ((e.Key == Key.Up || e.Key == Key.W)
            && currentDirection != Direction.Down)
        {
            nextDirection = Direction.Up;
        }
        else if ((e.Key == Key.Down || e.Key == Key.S)
                 && currentDirection != Direction.Up)
        {
            nextDirection = Direction.Down;
        }
        else if ((e.Key == Key.Left || e.Key == Key.A)
                 && currentDirection != Direction.Right)
        {
            nextDirection = Direction.Left;
        }
        else if ((e.Key == Key.Right || e.Key == Key.D)
                 && currentDirection != Direction.Left)
        {
            nextDirection = Direction.Right;
        }

        e.Handled = true;
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        gameTimer.Stop();
    }
}
