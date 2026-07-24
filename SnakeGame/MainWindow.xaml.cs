using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Ellipse = System.Windows.Shapes.Ellipse;

namespace SnakeGame;

public partial class MainWindow : Window
{
    private const int CellSize = 20;
    private const int NumberOfColumns = 30;
    private const int NumberOfRows = 25;

    private readonly List<Point> snake = new();
    private readonly Random random = new();
    private readonly DispatcherTimer gameTimer = new();

    private Point foodPosition;
    private Direction currentDirection = Direction.Right;
    private Direction nextDirection = Direction.Right;
    private int score;
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
        score = 0;
        isGameRunning = true;

        CreateFood();
        UpdateScoreText();

        MessageText.Text = "Ăn thức ăn màu đỏ và đừng va vào tường!";
        StartButton.Content = "Chơi lại";
        DrawGame();

        gameTimer.Start();
        Keyboard.Focus(this);
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        currentDirection = nextDirection;

        Point newHead = GetNewHeadPosition(snake[0]);
        bool willEatFood = newHead == foodPosition;

        if (HasHitWall(newHead) || HasHitSnake(newHead, willEatFood))
        {
            EndGame();
            return;
        }

        snake.Insert(0, newHead);

        if (willEatFood)
        {
            score++;
            CreateFood();
            UpdateScoreText();
        }
        else
        {
            snake.RemoveAt(snake.Count - 1);
        }

        DrawGame();
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

        return new Point(newX, newY);
    }

    private bool HasHitWall(Point head)
    {
        return head.X < 0
               || head.X >= NumberOfColumns
               || head.Y < 0
               || head.Y >= NumberOfRows;
    }

    private bool HasHitSnake(Point head, bool willEatFood)
    {
        int numberOfBodyPartsToCheck = snake.Count;

        // Khi không ăn, ô đuôi sẽ rời đi nên đầu được phép bước vào ô đuôi cũ.
        if (!willEatFood)
        {
            numberOfBodyPartsToCheck--;
        }

        for (int index = 0; index < numberOfBodyPartsToCheck; index++)
        {
            if (snake[index] == head)
            {
                return true;
            }
        }

        return false;
    }

    private void CreateFood()
    {
        if (snake.Count == NumberOfColumns * NumberOfRows)
        {
            EndGame();
            return;
        }

        do
        {
            int column = random.Next(0, NumberOfColumns);
            int row = random.Next(0, NumberOfRows);
            foodPosition = new Point(column, row);
        }
        while (snake.Contains(foodPosition));
    }

    private void DrawGame()
    {
        GameCanvas.Children.Clear();
        DrawFood();

        for (int index = 0; index < snake.Count; index++)
        {
            Point snakePart = snake[index];
            Brush color = index == 0 ? Brushes.LimeGreen : Brushes.ForestGreen;
            DrawCell(snakePart, color);
        }
    }

    private void DrawFood()
    {
        Ellipse food = new()
        {
            Width = CellSize - 4,
            Height = CellSize - 4,
            Fill = Brushes.OrangeRed
        };

        Canvas.SetLeft(food, foodPosition.X * CellSize + 2);
        Canvas.SetTop(food, foodPosition.Y * CellSize + 2);
        GameCanvas.Children.Add(food);
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

    private void EndGame()
    {
        gameTimer.Stop();
        isGameRunning = false;
        MessageText.Text = $"Kết thúc! Bạn đạt {score} điểm.";
    }

    private void UpdateScoreText()
    {
        ScoreText.Text = $"Điểm: {score}";
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
