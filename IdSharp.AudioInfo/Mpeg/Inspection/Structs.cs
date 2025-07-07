using System.Runtime.InteropServices;

using IdSharp.AudioInfo.Mpeg.Inspection;

namespace IdSharp.AudioInfo.Inspection;

// Xing/FhG VBR header data
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct VBRData
{
    public bool Found;                  // True if VBR header found
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
    public byte[] ID;                      // Header ID: "Xing" or "VBRI"
    public int Frames;                   // Total number of frames
    public int Bytes;                    // Total number of bytes
    public byte Scale;                     // VBR scale (1..100)
    public string VendorID;                // Vendor ID (if present)
}

// MPEG frame header data
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct FrameData
{
    public bool Found;                  // True if frame found
    public int Position;                   // Frame position in the file
    public ushort Size;                    // Frame size (bytes)
    public bool Xing;                   // True if Xing encoder
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
    public byte[] Data;                    // The whole frame header data
    public MpegVersion VersionID;          // MPEG version ID
    public MpegLayer LayerID;              // MPEG layer ID
    public bool ProtectionBit;          // True if protected by CRC
    public ushort BitRateID;               // Bit rate ID
    public SampleRateLevel SampleRateID;   // Sample rate ID
    public bool PaddingBit;             // True if frame padded
    public bool PrivateBit;             // Extra information
    public MpegChannel ModeID;             // Channel mode ID
    public JointStereoExtensionMode ModeExtensionID;           // Mode extension ID (for Joint Stereo)
    public bool CopyrightBit;           // True if audio copyrighted
    public bool OriginalBit;            // True if original media
    public Emphasis EmphasisID;            // Emphasis ID
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct LameTag
{
    public byte Quality;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
    public byte[] Encoder;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.AsAny)]
    public byte[] VersionString;
    public byte TagRevision_EncodingMethod;
    public byte Lowpass;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.AsAny)]
    public byte[] ReplayGain;
    public byte EncodingFlags_ATHType;
    public byte Bitrate;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.AsAny)]
    public byte[] EncoderDelays;
    public byte MiscInfo;
    public byte MP3Gain;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.AsAny)]
    public byte[] Surround_Preset;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
    public byte[] MusicLength;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.AsAny)]
    public byte[] MusicCRC;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.AsAny)]
    public byte[] InfoTagCRC;
    public byte NoiseShaping;
    public byte StereoMode;

    public static LameTag FromBinaryReader(BinaryReader br)
    {
        var tmpLameTag = new LameTag
        {
            Quality = br.ReadByte(),
            Encoder = br.ReadBytes(4),
            VersionString = br.ReadBytes(5),
            TagRevision_EncodingMethod = (byte)(br.ReadByte() & 0x0F),
            Lowpass = br.ReadByte(),
            ReplayGain = br.ReadBytes(8),
            EncodingFlags_ATHType = (byte)(br.ReadByte() & 0x0F),
            Bitrate = br.ReadByte(),
            EncoderDelays = br.ReadBytes(3),
            MiscInfo = br.ReadByte(),
            MP3Gain = br.ReadByte(),
            Surround_Preset = br.ReadBytes(2),
            MusicLength = br.ReadBytes(4),
            MusicCRC = br.ReadBytes(2),
            InfoTagCRC = br.ReadBytes(2)
        };

        tmpLameTag.NoiseShaping = (byte)(tmpLameTag.MiscInfo & 0x03);
        tmpLameTag.StereoMode = (byte)((tmpLameTag.MiscInfo & 0x1C) >> 2);

        return tmpLameTag;
    }
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct OldLameHeader
{
    public byte UnusedByte;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
    public byte[] Encoder;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.AsAny)]
    public byte[] VersionString;

    public static OldLameHeader FromBinaryReader(BinaryReader br)
    {
        OldLameHeader tmpOldLameHeader = new OldLameHeader();
        tmpOldLameHeader.UnusedByte = br.ReadByte();
        tmpOldLameHeader.Encoder = br.ReadBytes(4);
        tmpOldLameHeader.VersionString = br.ReadBytes(16);
        return tmpOldLameHeader;
    }
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct StartOfFile
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13, ArraySubType = UnmanagedType.AsAny)]
    public byte[] Misc1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
	    public byte[] Info1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
	    public byte[] Misc2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
	    public byte[] Info2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.AsAny)]
	    public byte[] Misc3;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.AsAny)]
	    public byte[] Info3;

    public static StartOfFile FromBinaryReader(BinaryReader br)
    {
        var tmpStartOfFile = new StartOfFile();
        tmpStartOfFile.Misc1 = br.ReadBytes(13);
        tmpStartOfFile.Info1 = br.ReadBytes(4);
        tmpStartOfFile.Misc2 = br.ReadBytes(4);
        tmpStartOfFile.Info2 = br.ReadBytes(4);
        tmpStartOfFile.Misc3 = br.ReadBytes(11);
        tmpStartOfFile.Info3 = br.ReadBytes(4);
        return tmpStartOfFile;
    }
};
