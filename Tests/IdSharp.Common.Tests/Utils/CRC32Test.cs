using System;
using System.IO;
using System.Text;
using IdSharp.Common.Utils;
using NUnit.Framework;

namespace IdSharp.Common.Tests.Utils;

[TestFixture]
public class CRC32Test
{
    // Test_1's intent is to have a leading 0 in the result
    private const string _testString_1 = "Hello World1";
    private readonly byte[] _testByteArray_1 = Encoding.ASCII.GetBytes(_testString_1);
    private const uint _crc32Result_1 = 0x019E0CC7;
    private const string _crc32StringResult_1 = "019E0CC7";

    // Test_2's intent is to have a result greather than 0x8000 0000
    private const string _testString_2 = "Hello World123";
    private readonly byte[] _testByteArray_2 = Encoding.ASCII.GetBytes(_testString_2);
    private const uint _crc32Result_2 = 0x9E742BA4;
    private const string _crc32StringResult_2 = "9E742BA4";

    [Test]
    public void Calculate_Set1_ByteArray()
    {
        var result = CRC32.Calculate(_testByteArray_1);
        Assert.That(_crc32StringResult_1, Is.EqualTo(result));
    }

    [Test]
    public void Calculate_Set1_Stream()
    {
        using (Stream stream = new MemoryStream(_testByteArray_1))
        {
            var result = CRC32.Calculate(stream);
            Assert.That(_crc32StringResult_1, Is.EqualTo(result));
        }
    }

    [Test]
    public void Calculate_Set1_Path()
    {
        var path = PathUtils.GetTempFileName(".txt");
        File.WriteAllText(path, _testString_1);
        try
        {
            var result = CRC32.Calculate(new FileInfo(path));
            Assert.That(_crc32StringResult_1, Is.EqualTo(result));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void CalculateInt32_Set1_ByteArray()
    {
        var result = CRC32.CalculateInt32(_testByteArray_1);
        Assert.That(_crc32Result_1, Is.EqualTo(result));
    }

    [Test]
    public void CalculateInt32_Set1_Stream()
    {
        using (Stream stream = new MemoryStream(_testByteArray_1))
        {
            var result = CRC32.CalculateInt32(stream);
            Assert.That(_crc32Result_1, Is.EqualTo(result));
        }
    }

    [Test]
    public void CalculateInt32_Set1_Path()
    {
        var path = PathUtils.GetTempFileName(".txt");
        File.WriteAllText(path, _testString_1);
        try
        {
            var result = CRC32.CalculateInt32(new FileInfo(path));
            Assert.That(_crc32Result_1, Is.EqualTo(result));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void Calculate_Set2_ByteArray()
    {
        var result = CRC32.Calculate(_testByteArray_2);
        Assert.That(_crc32StringResult_2, Is.EqualTo(result));
    }

    [Test]
    public void Calculate_Set2_Stream()
    {
        using (Stream stream = new MemoryStream(_testByteArray_2))
        {
            var result = CRC32.Calculate(stream);
            Assert.That(_crc32StringResult_2, Is.EqualTo(result));
        }
    }

    [Test]
    public void Calculate_Set2_Path()
    {
        var path = PathUtils.GetTempFileName(".txt");
        File.WriteAllText(path, _testString_2);
        try
        {
            var result = CRC32.Calculate(new FileInfo(path));
            Assert.That(_crc32StringResult_2, Is.EqualTo(result));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void CalculateInt32_Set2_ByteArray()
    {
        var result = CRC32.CalculateInt32(_testByteArray_2);
        Assert.That(_crc32Result_2, Is.EqualTo(result));
    }

    [Test]
    public void CalculateInt32_Set2_Stream()
    {
        using (Stream stream = new MemoryStream(_testByteArray_2))
        {
            var result = CRC32.CalculateInt32(stream);
            Assert.That(_crc32Result_2, Is.EqualTo(result));
        }
    }

    [Test]
    public void CalculateInt32_Set2_Path()
    {
        var path = PathUtils.GetTempFileName(".txt");
        File.WriteAllText(path, _testString_2);
        try
        {
            var result = CRC32.CalculateInt32(new FileInfo(path));
            Assert.That(_crc32Result_2, Is.EqualTo(result));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void Calculate_ByteArray_Null()
    {
        Assert.Throws<ArgumentNullException>(() => CRC32.Calculate((byte[])null));
    }

    [Test]
    public void Calculate_Stream_Null()
    {
        Assert.Throws<ArgumentNullException>(() => CRC32.Calculate((Stream)null));
    }

    [Test]
    public void Calculate_Path_Null()
    {
        Assert.Throws<ArgumentNullException>(() => CRC32.Calculate((FileInfo)null));
    }

    [Test]
    public void CalculateInt32_ByteArray_Null()
    {
        Assert.Throws<ArgumentNullException>(() => CRC32.CalculateInt32((byte[])null));
    }

    [Test]
    public void CalculateInt32_Stream_Null()
    {
        Assert.Throws<ArgumentNullException>(() => CRC32.CalculateInt32((Stream)null));
    }

    [Test]
    public void CalculateInt32_Path_Null()
    {
        Assert.Throws<ArgumentNullException>(() => CRC32.CalculateInt32((FileInfo)null));
    }
}
