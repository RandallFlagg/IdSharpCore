using IdSharp.AudioInfo.Mpeg.Inspection;
using IdSharp.AudioInfo.Mpeg.Mpeg.Inspection;

using NUnit.Framework;

namespace IdSharp.Mpeg;

[TestFixture]
internal static class MpegTests
{
    private static readonly string MP3TestFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "file_example_MP3_700KB.mp3");

    [Test]
    public static void LoadMp3File()
    {
        Assert.That(File.Exists(MP3TestFilePath), Is.True);

        var mp3Bytes = File.ReadAllBytes(MP3TestFilePath);
        Assert.That(mp3Bytes.Length, Is.GreaterThan(0));
    }

    [Test]
    public static void BasicLameTagReader_Errors()
    {
        _ = Assert.Throws<FileNotFoundException>(static () => new BasicLameTagReader("./JUST_MADE_UP_FOLDER_NAME_TEST_FILES"));
        _ = Assert.Throws<ArgumentNullException>(static () => new BasicLameTagReader(null));
        _ = Assert.Throws<ArgumentException>(static () => new BasicLameTagReader("       "));
        _ = Assert.Throws<ArgumentException>(static () => new BasicLameTagReader(string.Empty));
    }

    [Test]
    public static void BasicLameTagReader()
    {
        var reader = new BasicLameTagReader(MP3TestFilePath);
        Assert.That(reader.IsLameTagFound, Is.True, "IsLameTagFound should be true");
        Assert.That(reader.EncodingMethod, Is.EqualTo(4), "EncodingMethod should be 4");
        Assert.That(reader.Bitrate, Is.EqualTo(32), "Bitrate should be 32");
        Assert.That(reader.IsPresetGuessNonBitrate, Is.EqualTo(false), "IsPresetGuessNonBitrate should be false");
        Assert.That(reader.PresetGuess, Is.EqualTo(LamePreset.Unknown), "PresetGuess should be Unknown");
        Assert.That(reader.Preset, Is.EqualTo(480), "Preset should be 480");
        Assert.That(reader.VersionString, Is.EqualTo("3.10"), "VersionString should be '3.10'");
        Assert.That(reader.VersionStringNonLameTag, Is.EqualTo("ME3.100\u0004?\0\0\0\0\0\0\0"), @"VersionStringNonLameTag should be 'ME3.100\u0004?\0\0\0\0\0\0\0'");
    }
}