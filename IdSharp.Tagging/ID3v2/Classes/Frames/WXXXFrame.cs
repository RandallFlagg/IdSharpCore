using System;
using System.IO;
using IdSharp.Tagging.ID3v2.Extensions;
using IdSharp.Common.Utils;

namespace IdSharp.Tagging.ID3v2.Frames;

internal sealed class WXXXFrame : Frame, IWXXXFrame
{
    private EncodingType _textEncoding;
    private string _description;
    private string _value;

    public EncodingType TextEncoding
    {
        get => _textEncoding;
        set
        {
            _textEncoding = value;
            RaisePropertyChanged(nameof(TextEncoding));
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            RaisePropertyChanged(nameof(Description));
        }
    }

    public string Value
    {
        get => _value;
        set
        {
            _value = value;
            RaisePropertyChanged(nameof(Value));
        }
    }

    public override string GetFrameID(ID3v2TagVersion tagVersion)
    {
        switch (tagVersion)
        {
            case ID3v2TagVersion.ID3v24:
            case ID3v2TagVersion.ID3v23:
                return "WXXX";
            case ID3v2TagVersion.ID3v22:
                return "WXX";
            default:
                throw new ArgumentException("Unknown tag version");
        }
    }

    public override void Read(TagReadingInfo tagReadingInfo, Stream stream)
    {
        _frameHeader.Read(tagReadingInfo, ref stream);
        if (_frameHeader.FrameSizeExcludingAdditions > 0)
        {
            TextEncoding = (EncodingType)stream.Read1();
            var bytesLeft = _frameHeader.FrameSizeExcludingAdditions - 1;
            Description = ID3v2Utils.ReadString(TextEncoding, stream, ref bytesLeft);
            Value = ID3v2Utils.ReadString(EncodingType.ISO88591, stream, bytesLeft);
        }
        else
        {
            /*String msg = String.Format("0 length frame '{0}' at position {1}", "WXXX", stream.Position);
            Trace.WriteLine(msg);*/

            Description = string.Empty;
            Value = string.Empty;
        }
    }

    public override byte[] GetBytes(ID3v2TagVersion tagVersion)
    {
        if (string.IsNullOrEmpty(Value))
        {
            return new byte[0];
        }

        byte[] descriptionData;
        do
        {
            descriptionData = ID3v2Utils.GetStringBytes(tagVersion, TextEncoding, Description, true);
        } while (this.RequiresFix(tagVersion, Description, descriptionData));

        using (var frameData = new MemoryStream())
        {
            frameData.WriteByte((byte)TextEncoding);
            frameData.Write(descriptionData);
            frameData.Write(ByteUtils.ISO88591GetBytes(Value));

            return _frameHeader.GetBytes(frameData, tagVersion, GetFrameID(tagVersion));
        }
    }
}
