using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SnakeGame;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DrawWelcomeScreen();
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
