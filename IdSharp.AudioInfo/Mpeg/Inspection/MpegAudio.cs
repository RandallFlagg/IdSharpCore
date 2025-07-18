using System.Text;

using IdSharp.AudioInfo.Inspection;

namespace IdSharp.AudioInfo.Mpeg.Inspection;

internal sealed class MpegAudio
{
    #region <<< Private Constants >>>

    // Limitation constants
    private const int MaxMpegFrameLength = 1729;

    private static readonly string[] MPEG_VERSION = ["MPEG 2.5", "MPEG ?", "MPEG 2", "MPEG 1"];
    private static readonly string[] MPEG_LAYER = ["Layer ?", "Layer III", "Layer II", "Layer I"];

    // Table for sample rates
    private static readonly ushort[][] MPEG_SAMPLE_RATE = [
        [11025, 12000, 8000, 0],
        [0, 0, 0, 0],
        [22050, 24000, 16000, 0],
        [44100, 48000, 32000, 0]
    ];

    private static readonly ushort[][][] BitrateTable = [
    [   // MPEG-2.5
        [8, 16, 24, 32, 40, 48,  56,  64,  80,  96, 112, 128, 144, 160], // Layer-3
        [8, 16, 24, 32, 40, 48,  56,  64,  80,  96, 112, 128, 144, 160], // Layer-2
        [32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256]  // Layer-1
    ],
    [
        [0,  0,  0,  0,  0,  0,   0,   0,   0,   0,   0,   0,   0,   0],
        [0,  0,  0,  0,  0,  0,   0,   0,   0,   0,   0,   0,   0,   0],
        [0,  0,  0,  0,  0,  0,   0,   0,   0,   0,   0,   0,   0,   0]
    ],
    [   // MPEG-2
        [8, 16, 24, 32, 40, 48,  56,  64,  80,  96, 112, 128, 144, 160], // Layer-3
        [8, 16, 24, 32, 40, 48,  56,  64,  80,  96, 112, 128, 144, 160], // Layer-2
        [32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256]  // Layer-1
    ],
    [   // MPEG-1
        [32, 40, 48, 56, 64, 80,  96, 112, 128, 160, 192, 224, 256, 320], // Layer-3
        [32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384], // Layer-2
        [32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448] // Layer-1
    ]
    ];

    #endregion <<< Private Constants >>>

    #region <<< Private Fields >>>

    private long m_FileLength;
    private string m_VendorID;
    private VBRData m_VBR;
    private FrameData m_Frame;

    #endregion <<< Private Fields >>>

    #region <<< Constructor >>>

    public MpegAudio(string path)
    {
        const int dataLength = MaxMpegFrameLength * 2;

        ResetData();

        using (var br = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
        {
            var startPos = ID3v2.GetTagSize(br.BaseStream);

            // Get file length
            m_FileLength = br.BaseStream.Length;

            // Seek past ID3v2 tag
            br.BaseStream.Seek(startPos, SeekOrigin.Begin);

            // Read first block of data and search for a frame
            var data = br.ReadBytes(dataLength);
            FindFrame(data, ref m_VBR);
            m_VendorID = FindVendorID(data);

            // Try to search in the middle of the file if no frame at the beginning found
            if (!m_Frame.Found)
            {
                br.BaseStream.Seek((m_FileLength - startPos) / 2, SeekOrigin.Begin);
                data = br.ReadBytes(dataLength);
                FindFrame(data, ref m_VBR);
            }

            // Search for vendor ID at the end if CBR encoded
            if (m_Frame.Found && string.IsNullOrEmpty(m_VendorID))
            {
                br.BaseStream.Seek(-(data.Length + ID3v1.GetTagSize(br.BaseStream)), SeekOrigin.End);

                data = br.ReadBytes(dataLength);
                FindFrame(data, ref m_VBR);
                m_VendorID = FindVendorID(data);
            }
        }

        if (m_Frame.Found == false)
        {
            ResetData();
        }
    }

    #endregion <<< Constructor >>>

    #region <<< Private Methods >>>

    /// <summary>
    /// Reset all variables
    /// </summary>
    private void ResetData()
    {
        m_FileLength = 0;
        m_VendorID = "";

        m_Frame.VersionID = MpegVersion.Unknown;
        m_Frame.SampleRateID = SampleRateLevel.Unknown;
        m_Frame.ModeID = MpegChannel.Unknown;
        m_Frame.ModeExtensionID = JointStereoExtensionMode.Unknown;
        m_Frame.EmphasisID = Emphasis.Unknown;
    }

    /// <summary>
    /// Find a valid MP3 frame in a byte array.  Sets "m_Frame.Found = true"
    /// </summary>
    /// <param name="data">Byte array of data from MP3 file</param>
    /// <param name="vbrHeader">Method sets this variable to a VBRData struct, if frame found</param>
    private void FindFrame(byte[] data, ref VBRData vbrHeader)
    {
        var headerData = new byte[4];

        // Search for valid frame
        Buffer.BlockCopy(data, 0, headerData, 0, 4);

        var size = data.Length - 4;
        for (var i = 0; i < size; i++)
        {
            // Decode data if frame header found
            if (IsFrameHeader(headerData))
            {
                DecodeHeader(headerData);
                var nextHeader = i + GetFrameLength(m_Frame);
                if (nextHeader < size)
                {
                    // Check for next frame and try to find VBR header
                    if (ValidFrameAt(nextHeader, data))
                    {
                        m_Frame.Found = true;
                        m_Frame.Position = i;
                        m_Frame.Size = GetFrameLength(m_Frame);
                        m_Frame.Xing = IsXing(i + headerData.Length, data);
                        vbrHeader = FindVBR(i + GetVBRFrameOffset(m_Frame), data);
                        break;
                    }
                }
            }

            // Prepare next data block
            headerData[0] = headerData[1];
            headerData[1] = headerData[2];
            headerData[2] = headerData[3];
            headerData[3] = data[4 + i];
        }
    }

    /// <summary>
    /// Returns true if valid frame header sent
    /// </summary>
    /// <param name="headerData">Header data to test</param>
    /// <returns>True if valid frame header sent</returns>
    private static bool IsFrameHeader(byte[] headerData)
    {
        // Check for valid frame header
        if ((headerData[0] & 0xFF) != 0xFF ||
            (headerData[1] & 0xE0) != 0xE0 ||
            (headerData[1] >> 3 & 3) == 1 ||
            (headerData[1] >> 1 & 3) == 0 ||
            (headerData[2] & 0xF0) == 0xF0 ||
            (headerData[2] & 0xF0) == 0 ||
            (headerData[2] >> 2 & 3) == 3 ||
            (headerData[3] & 3) == 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Populates the "m_Frame" struct with data from the header
    /// </summary>
    /// <param name="headerData">Header data byte array</param>
    private void DecodeHeader(byte[] headerData)
    {
        // Decode frame header data            
        m_Frame.Data = new byte[headerData.Length];
        Buffer.BlockCopy(headerData, 0, m_Frame.Data, 0, headerData.Length);

        m_Frame.VersionID = (MpegVersion)(headerData[1] >> 3 & 3);
        m_Frame.LayerID = (MpegLayer)(headerData[1] >> 1 & 3);
        m_Frame.ProtectionBit = (headerData[1] & 1) != 1;
        m_Frame.BitRateID = (byte)(headerData[2] >> 4); //TODO: why is this being cast to byte and not ushort?
        m_Frame.SampleRateID = (SampleRateLevel)(headerData[2] >> 2 & 3);
        m_Frame.PaddingBit = (headerData[2] >> 1 & 1) == 1;
        m_Frame.PrivateBit = (headerData[2] & 1) == 1;
        m_Frame.ModeID = (MpegChannel)(headerData[3] >> 6 & 3);
        m_Frame.ModeExtensionID = (JointStereoExtensionMode)(headerData[3] >> 4 & 3);
        m_Frame.CopyrightBit = (headerData[3] >> 3 & 1) == 1;
        m_Frame.OriginalBit = (headerData[3] >> 2 & 1) == 1;
        m_Frame.EmphasisID = (Emphasis)(headerData[3] & 3);
    }

    /// <summary>
    /// Returns true if valid frame found at Index
    /// </summary>
    /// <param name="index">Index</param>
    /// <param name="data">Data byte array</param>
    /// <returns>True if valid frame found at Index</returns>
    private static bool ValidFrameAt(int index, byte[] data)
    {
        var HeaderData = new byte[4];

        // Check for frame at given position
        HeaderData[0] = data[index];
        HeaderData[1] = data[index + 1];
        HeaderData[2] = data[index + 2];
        HeaderData[3] = data[index + 3];

        return IsFrameHeader(HeaderData);
    }

    /// <summary>
    /// Returns the length of a frame in bytes
    /// </summary>
    /// <param name="frame">Frame</param>
    /// <returns>Length of frame in bytes</returns>
    private static ushort GetFrameLength(FrameData frame)
    {
        // Calculate MPEG frame length
        ushort Coefficient = GetCoefficient(frame);
        var BitRate = GetBitRate(frame);
        var SampleRate = GetSampleRate(frame);
        ushort Padding = GetPadding(frame);

        var result = (ushort)(Coefficient * BitRate * 1000 / SampleRate + Padding);

        return result;
    }

    /// <summary>
    /// Returns true if Xing encoder
    /// </summary>
    /// <param name="index">Index</param>
    /// <param name="data">Byte array</param>
    /// <returns>True of Xing encoder</returns>
    private static bool IsXing(int index, byte[] data)
    {
        var result =
            data[index] == 0 &&
            data[index + 1] == 0 &&
            data[index + 2] == 0 &&
            data[index + 3] == 0 &&
            data[index + 4] == 0 &&
            data[index + 5] == 0;

        return result;
    }

    /// <summary>
    /// Returns a VBRData structure from an index in a byte array
    /// </summary>
    /// <param name="index">Index</param>
    /// <param name="data">Byte array</param>
    /// <returns>VBRData structure</returns>
    private static VBRData FindVBR(int index, byte[] data)
    {
        VBRData result;

        var id = $"{(char)data[index]}{(char)data[index + 1]}{(char)data[index + 2]}{(char)data[index + 3]}";

        // Check for VBR header at given position
        if (id == VBRHeaderID.Xing)
        {
            result = GetXingInfo(index, data);
        }
        else if (id == VBRHeaderID.FhG)
        {
            result = GetFhGInfo(index, data);
        }
        else
        {
            result = new VBRData();
        }

        return result;
    }

    /// <summary>
    /// Returns the offset of the VBRData structure from the start of a frame
    /// </summary>
    /// <param name="Frame">Populated FrameData structure</param>
    /// <returns>Offset of the VBRData structure</returns>
    private static byte GetVBRFrameOffset(FrameData Frame)
    {
        byte result;

        // Calculate VBR deviation
        if (Frame.VersionID == MpegVersion.Version1)
        {
            if (Frame.ModeID != MpegChannel.Mono)
            {
                result = 36;
            }
            else
            {
                result = 21;
            }
        }
        else
        {
            if (Frame.ModeID != MpegChannel.Mono)
            {
                result = 21;
            }
            else
            {
                result = 13;
            }
        }

        return result;
    }

    /// <summary>
    /// Returns frame size coefficient (used in calculating frame size)
    /// </summary>
    /// <param name="Frame">Populated FrameData structure</param>
    /// <returns>Frame size coefficient</returns>
    private static byte GetCoefficient(FrameData Frame)
    {
        byte result;

        // Get frame size coefficient
        if (Frame.VersionID == MpegVersion.Version1)
        {
            if (Frame.LayerID == MpegLayer.LayerI)
            {
                result = 48;
            }
            else
            {
                result = 144;
            }
        }
        else
        {
            if (Frame.LayerID == MpegLayer.LayerI)
            {
                result = 24;
            }
            else if (Frame.LayerID == MpegLayer.LayerII)
            {
                result = 144;
            }
            else
            {
                result = 72;
            }
        }

        return result;
    }

    /// <summary>
    /// Returns the bitrate of a frame
    /// </summary>
    /// <param name="Frame">Populated FrameData structure</param>
    /// <returns>Bitrate of the frame</returns>
    private static ushort GetBitRate(FrameData Frame)
    {
        // Get bit rate
        var result = BitrateTable[(int)Frame.VersionID][(int)Frame.LayerID - 1][Frame.BitRateID - 1];

        return result;
    }

    /// <summary>
    /// Returns the sample rate of a frame
    /// </summary>
    /// <param name="Frame">Populated FrameData structure</param>
    /// <returns>Sample rate of the frame</returns>
    private static ushort GetSampleRate(FrameData Frame)
    {
        // Get sample rate
        var result = MPEG_SAMPLE_RATE[(int)Frame.VersionID][(int)Frame.SampleRateID];

        return result;
    }

    /// <summary>
    /// Returns the padding size of a frame (used in calculating frame size)
    /// </summary>
    /// <param name="Frame">Populated FrameData structure</param>
    /// <returns>Padding size</returns>
    private static byte GetPadding(FrameData Frame)
    {
        byte result;

        // Get frame padding
        if (Frame.PaddingBit)
        {
            if (Frame.LayerID == MpegLayer.LayerI)
            {
                result = 4;
            }
            else
            {
                result = 1;
            }
        }
        else
        {
            result = 0;
        }

        return result;
    }

    /// <summary>
    /// Returns VBRData structure from a Xing info tag
    /// </summary>
    /// <param name="index">Index in array</param>
    /// <param name="data">Data byte array</param>
    /// <returns>VBRData structure</returns>
    private static VBRData GetXingInfo(int index, byte[] data)
    {
        VBRData result;

        // Extract Xing VBR info at given position

        result.Found = true;
        result.ID = Encoding.ASCII.GetBytes(VBRHeaderID.Xing);
        result.Frames =
            data[index + 8] * 0x1000000 +
            data[index + 9] * 0x10000 +
            data[index + 10] * 0x100 +
            data[index + 11];
        result.Bytes =
            data[index + 12] * 0x1000000 +
            data[index + 13] * 0x10000 +
            data[index + 14] * 0x100 +
            data[index + 15];
        result.Scale = data[index + 119];
        // Vendor ID can be not present
        result.VendorID = $"{(char)data[index + 120]}{(char)data[index + 121]}{(char)data[index + 122]}{(char)data[index + 123]}{(char)data[index + 124]}{(char)data[index + 125]}{(char)data[index + 126]}{(char)data[index + 127]}";

        return result;
    }

    /// <summary>
    /// Returns VBRData structure from an FhG info tag
    /// </summary>
    /// <param name="index">Index in array</param>
    /// <param name="data">Data byte array</param>
    /// <returns>VBRData structure</returns>
    private static VBRData GetFhGInfo(int index, byte[] data)
    {
        var result = new VBRData();

        // Extract FhG VBR info at given position

        result.Found = true;
        result.ID = Encoding.ASCII.GetBytes(VBRHeaderID.FhG);
        result.Scale = data[index + 9];
        result.Bytes =
            data[index + 10] * 0x1000000 +
            data[index + 11] * 0x10000 +
            data[index + 12] * 0x100 +
            data[index + 13];
        result.Frames =
            data[index + 14] * 0x1000000 +
            data[index + 15] * 0x10000 +
            data[index + 16] * 0x100 +
            data[index + 17];

        return result;
    }

    /// <summary>
    /// Find a VendorID in a byte array
    /// </summary>
    /// <param name="data">Byte array</param>
    /// <returns>VendorID</returns>
    private static string FindVendorID(byte[] data)
    {
        var result = "";

        // Search for vendor ID
        var size = data.Length;
        for (var i = 0; i <= size - 8; i++)
        {
            var VendorID = $"{(char)data[size - i - 8]}{(char)data[size - i - 7]}{(char)data[size - i - 6]}{(char)data[size - i - 5]}";

            if (VendorID == VBRVendorID.LAME)
            {
                result = VendorID +
                    $"{(char)data[size - i - 4]}{(char)data[size - i - 3]}{(char)data[size - i - 2]}{(char)data[size - i - 1]}";
                break;
            }

            if (VendorID == VBRVendorID.GoGoNew)
            {
                result = VendorID;
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Returns encoder
    /// </summary>
    /// <returns>Encoder</returns>
    private MpegEncoder GetEncoderID()
    {
        var result = MpegEncoder.Unknown;

        // Get guessed encoder ID
        if (m_Frame.Found)
        {
            if (m_VBR.Found)
            {
                result = GetVBREncoderID();
            }
            else
            {
                result = GetCBREncoderID();
            }
        }

        return result;
    }

    /// <summary>
    /// Returns encoder from a VBR info tag
    /// </summary>
    /// <returns>Encoder from a VBR info tag</returns>
    private MpegEncoder GetVBREncoderID()
    {
        var result = MpegEncoder.Unknown;

        var vbrVendor = m_VBR.VendorID.Substring(0, 4);

        // Guess VBR encoder and get ID
        if (vbrVendor == VBRVendorID.LAME)
        {
            result = MpegEncoder.LAME;
        }

        if (vbrVendor == VBRVendorID.GoGoNew)
        {
            result = MpegEncoder.GoGo;
        }

        if (vbrVendor == VBRVendorID.GoGoOld)
        {
            result = MpegEncoder.GoGo;
        }

        if (Encoding.ASCII.GetString(m_VBR.ID) == VBRHeaderID.Xing &&
            vbrVendor != VBRVendorID.LAME &&
            vbrVendor != VBRVendorID.GoGoNew &&
            vbrVendor != VBRVendorID.GoGoOld)
        {
            result = MpegEncoder.Xing;
        }

        if (Encoding.ASCII.GetString(m_VBR.ID) == VBRHeaderID.FhG)
        {
            result = MpegEncoder.FhG;
        }

        if (vbrVendor == VBRVendorID.LAME)
        {
            result = MpegEncoder.LAME;
        }

        return result;
    }

    /// <summary>
    /// Returns encoder from a CBR info tag
    /// </summary>
    /// <returns>Encoder from a CBR info tag</returns>
    private MpegEncoder GetCBREncoderID()
    {
        var result = MpegEncoder.FhG;

        string shortVendor;

        if (!string.IsNullOrEmpty(m_VendorID) && m_VendorID.Length >= 4)
        {
            shortVendor = m_VendorID.Substring(0, 4);
        }
        else
        {
            shortVendor = "";
        }

        // Guess CBR encoder and get ID
        if (m_Frame.OriginalBit && m_Frame.ProtectionBit)
        {
            result = MpegEncoder.LAME;
        }

        if (GetBitRate(m_Frame) <= 160 && m_Frame.ModeID == MpegChannel.Stereo)
        {
            result = MpegEncoder.Blade;
        }

        if (m_Frame.CopyrightBit && m_Frame.OriginalBit && !m_Frame.ProtectionBit)
        {
            result = MpegEncoder.Xing;
        }

        if (m_Frame.Xing && m_Frame.OriginalBit)
        {
            result = MpegEncoder.Xing;
        }

        if (m_Frame.LayerID == MpegLayer.LayerII)
        {
            result = MpegEncoder.QDesign;
        }

        if (m_Frame.ModeID == MpegChannel.DualChannel && m_Frame.ProtectionBit)
        {
            result = MpegEncoder.Shine;
        }

        if (shortVendor == VBRVendorID.LAME)
        {
            result = MpegEncoder.LAME;
        }

        if (shortVendor == VBRVendorID.GoGoNew)
        {
            result = MpegEncoder.GoGo;
        }

        return result;
    }

    #endregion <<< Private Methods >>>

    #region <<< Public Properties >>>

    /// <summary>
    /// Returns MPEG version
    /// </summary>
    public string Version { get { return MPEG_VERSION[(int)m_Frame.VersionID]; } }

    /// <summary>
    /// Returns MPEG layer
    /// </summary>
    public string Layer { get { return MPEG_LAYER[(int)m_Frame.LayerID]; } }

    /// <summary>
    /// Returns MPEG encoder and version (if known)
    /// </summary>
    public string Encoder
    {
        get
        {
            var myVendorID = "";

            // Get guessed encoder name and encoder version for LAME
            var result = GetEncoderID().ToString();
            if (!string.IsNullOrEmpty(m_VBR.VendorID))
            {
                myVendorID = m_VBR.VendorID;
            }

            if (!string.IsNullOrEmpty(m_VendorID))
            {
                myVendorID = m_VendorID;
            }

            if (GetEncoderID() == MpegEncoder.LAME &&
                myVendorID.Length >= 8 &&
                char.IsDigit(myVendorID, 4) &&
                myVendorID[5] == '.' &&
                char.IsDigit(myVendorID, 6) &&
                char.IsDigit(myVendorID, 7))
            {
                result += " " + myVendorID.Substring(4, 4);
            }

            return result;
        }
    }

    #endregion
}
