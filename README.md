# Game rắn săn mồi bằng WPF

Project được viết theo hướng đơn giản, dễ đọc, phù hợp để học các kiến thức WPF cơ bản.

## Chức năng

- Điều khiển rắn bằng phím mũi tên hoặc W, A, S, D.
- Ăn thức ăn để tăng điểm và độ dài.
- Kết thúc khi đụng tường hoặc đụng vào thân.
- Có nút bắt đầu/chơi lại.
- Có màn hình menu trước khi vào game.
- Cho phép chọn ba tốc độ: Chậm, Bình thường và Nhanh.
- Có nhạc nền phát lặp và tùy chọn bật/tắt nhạc.
- Tự động lưu điểm cao sau khi kết thúc game.

Điểm cao được lưu tại:

```text
%LocalAppData%\SimpleSnakeGame\highscore.txt
```

## Cách chạy

Mở `SnakeGame.slnx` bằng Visual Studio 2026 hoặc chạy:

```powershell
dotnet run --project .\SnakeGame\SnakeGame.csproj
```

## Các file chính

- `MainWindow.xaml`: phần giao diện.
- `MainWindow.xaml.cs`: toàn bộ luật chơi và xử lý bàn phím.
- `Direction.cs`: bốn hướng di chuyển của rắn.
- `BackgroundMusicPlayer.cs`: tạo và phát nhạc nền đơn giản.
