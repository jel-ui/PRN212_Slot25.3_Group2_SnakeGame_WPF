using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Ellipse = System.Windows.Shapes.Ellipse;
using Rectangle = System.Windows.Shapes.Rectangle;

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
    private int highScore;
    private bool isGameRunning;

    private readonly string highScoreFilePath;
    private readonly BackgroundMusicPlayer backgroundMusicPlayer;

    public MainWindow()
    {
        InitializeComponent();

        string applicationDataFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string gameDataFolder = Path.Combine(applicationDataFolder, "SimpleSnakeGame");
        highScoreFilePath = Path.Combine(gameDataFolder, "highscore.txt");
        backgroundMusicPlayer = new BackgroundMusicPlayer(gameDataFolder);

        gameTimer.Interval = TimeSpan.FromMilliseconds(130);
        gameTimer.Tick += GameTimer_Tick;

        highScore = LoadHighScore();
        UpdateScoreText();
        DrawWelcomeScreen();
    }

    private void EnterGameButton_Click(object sender, RoutedEventArgs e)
    {
        ApplySelectedSpeed();

        MenuPanel.Visibility = Visibility.Collapsed;
        GamePanel.Visibility = Visibility.Visible;

        if (MusicCheckBox.IsChecked == true)
        {
            backgroundMusicPlayer.Play();
        }

        StartNewGame();
        Keyboard.Focus(this);
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StartNewGame();
        Keyboard.Focus(this);
    }

    private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
    {
        gameTimer.Stop();
        backgroundMusicPlayer.Stop();
        isGameRunning = false;

        GamePanel.Visibility = Visibility.Collapsed;
        MenuPanel.Visibility = Visibility.Visible;
    }

    private void ApplySelectedSpeed()
    {
        if (SpeedComboBox.SelectedItem is not ComboBoxItem selectedItem)
        {
            return;
        }

        string speedName = selectedItem.Content.ToString() ?? "Bình thường";
        string timerIntervalText = selectedItem.Tag.ToString() ?? "130";

        if (int.TryParse(timerIntervalText, out int timerInterval))
        {
            gameTimer.Interval = TimeSpan.FromMilliseconds(timerInterval);
        }

        SpeedText.Text = $"Tốc độ: {speedName}";
    }

    private void StartNewGame()
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
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        currentDirection = nextDirection;

        Point currentHead = snake[0];
        Point newHead = GetNewHeadPosition(currentHead);

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
        return currentDirection switch
        {
            Direction.Up => new Point(currentHead.X, currentHead.Y - 1),
            Direction.Down => new Point(currentHead.X, currentHead.Y + 1),
            Direction.Left => new Point(currentHead.X - 1, currentHead.Y),
            Direction.Right => new Point(currentHead.X + 1, currentHead.Y),
            _ => currentHead
        };
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

        // Khi rắn không ăn, đuôi sẽ di chuyển đi nên được phép bước vào ô đuôi cũ.
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
        Rectangle rectangle = new()
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
        GameCanvas.Children.Clear();

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

        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
            MessageText.Text = $"Kỷ lục mới! Bạn đạt {score} điểm.";
        }
        else
        {
            MessageText.Text = $"Kết thúc! Bạn đạt {score} điểm.";
        }

        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        ScoreText.Text = $"Điểm: {score}";
        HighScoreText.Text = $"Điểm cao: {highScore}";
    }

    private int LoadHighScore()
    {
        try
        {
            if (!File.Exists(highScoreFilePath))
            {
                return 0;
            }

            string savedText = File.ReadAllText(highScoreFilePath);

            if (int.TryParse(savedText, out int savedHighScore))
            {
                return savedHighScore;
            }
        }
        catch
        {
            // Nếu file lỗi hoặc không đọc được, game vẫn chạy với điểm cao bằng 0.
        }

        return 0;
    }

    private void SaveHighScore()
    {
        try
        {
            string? folderPath = Path.GetDirectoryName(highScoreFilePath);

            if (folderPath is not null)
            {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllText(highScoreFilePath, highScore.ToString());
        }
        catch
        {
            MessageText.Text = "Đạt kỷ lục mới nhưng không thể lưu điểm cao.";
        }
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
        backgroundMusicPlayer.Stop();
    }
}
