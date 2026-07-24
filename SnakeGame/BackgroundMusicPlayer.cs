using System.IO;
using System.Windows.Media;

namespace SnakeGame;

public class BackgroundMusicPlayer
{
    private readonly MediaPlayer mediaPlayer = new();
    private readonly string musicFilePath;

    public BackgroundMusicPlayer(string gameDataFolder)
    {
        musicFilePath = Path.Combine(gameDataFolder, "background_music.wav");
        mediaPlayer.Volume = 0.25;
        mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
    }

    public void Play()
    {
        CreateMusicFileIfNeeded();
        mediaPlayer.Open(new Uri(musicFilePath));
        mediaPlayer.Play();
    }

    public void Stop()
    {
        mediaPlayer.Stop();
        mediaPlayer.Close();
    }

    private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
    {
        // Đưa bài nhạc về đầu để phát lặp lại.
        mediaPlayer.Position = TimeSpan.Zero;
        mediaPlayer.Play();
    }

    private void CreateMusicFileIfNeeded()
    {
        if (File.Exists(musicFilePath))
        {
            return;
        }

        string? folderPath = Path.GetDirectoryName(musicFilePath);

        if (folderPath is not null)
        {
            Directory.CreateDirectory(folderPath);
        }

        const int sampleRate = 22050;
        const double noteDurationInSeconds = 0.28;

        double[] melodyFrequencies =
        {
            261.63, 329.63, 392.00, 523.25,
            392.00, 329.63, 293.66, 392.00
        };

        int samplesPerNote = (int)(sampleRate * noteDurationInSeconds);
        int totalNumberOfSamples = samplesPerNote * melodyFrequencies.Length;
        int dataSize = totalNumberOfSamples * sizeof(short);

        using FileStream fileStream = File.Create(musicFilePath);
        using BinaryWriter writer = new(fileStream);

        WriteWaveFileHeader(writer, sampleRate, dataSize);

        foreach (double frequency in melodyFrequencies)
        {
            WriteNote(writer, frequency, sampleRate, samplesPerNote);
        }
    }

    private static void WriteWaveFileHeader(
        BinaryWriter writer,
        int sampleRate,
        int dataSize)
    {
        writer.Write("RIFF"u8);
        writer.Write(36 + dataSize);
        writer.Write("WAVE"u8);
        writer.Write("fmt "u8);
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(sampleRate);
        writer.Write(sampleRate * sizeof(short));
        writer.Write((short)sizeof(short));
        writer.Write((short)16);
        writer.Write("data"u8);
        writer.Write(dataSize);
    }

    private static void WriteNote(
        BinaryWriter writer,
        double frequency,
        int sampleRate,
        int numberOfSamples)
    {
        const double volume = 0.20;

        for (int sampleIndex = 0; sampleIndex < numberOfSamples; sampleIndex++)
        {
            double time = (double)sampleIndex / sampleRate;

            // Giảm âm lượng dần ở cuối mỗi nốt để âm thanh bớt gắt.
            double fadeOut = 1.0 - (double)sampleIndex / numberOfSamples;
            double wave = Math.Sin(2 * Math.PI * frequency * time);
            short sampleValue = (short)(wave * short.MaxValue * volume * fadeOut);

            writer.Write(sampleValue);
        }
    }
}
