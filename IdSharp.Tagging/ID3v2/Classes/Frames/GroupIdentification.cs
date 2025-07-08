using System;
using System.IO;
using IdSharp.Common.Utils;

namespace IdSharp.Tagging.ID3v2.Frames;

internal sealed class GroupIdentification : Frame, IGroupIdentification
{
    private string _ownerIdentifier;
    private byte _groupSymbol;
    private byte[] _groupDependentData;

    public string OwnerIdentifier
    {
        get => _ownerIdentifier;
        set
        {
            _ownerIdentifier = value;
            RaisePropertyChanged(nameof(OwnerIdentifier));
        }
    }

    public byte GroupSymbol
    {
        get => _groupSymbol;
        set
        {
            _groupSymbol = value;
            RaisePropertyChanged(nameof(GroupSymbol));
        }
    }

    public byte[] GroupDependentData
    {
        get => ByteUtils.Clone(_groupDependentData);
        set
        {
            _groupDependentData = ByteUtils.Clone(value);
            RaisePropertyChanged(nameof(GroupDependentData));
        }
    }

    public override string GetFrameID(ID3v2TagVersion tagVersion)
    {
        switch (tagVersion)
        {
            case ID3v2TagVersion.ID3v24:
            case ID3v2TagVersion.ID3v23:
                return "GRID";
            case ID3v2TagVersion.ID3v22:
                return null;
            default:
                throw new ArgumentException("Unknown tag version");
        }
    }

    public override void Read(TagReadingInfo tagReadingInfo, Stream stream)
    {
        throw new NotImplementedException();
    }

    public override byte[] GetBytes(ID3v2TagVersion tagVersion)
    {
        using (var frameData = new MemoryStream())
        {
            frameData.Write(ID3v2Utils.GetStringBytes(tagVersion, EncodingType.ISO88591, _ownerIdentifier, true));
            frameData.WriteByte(_groupSymbol);
            if (_groupDependentData != null)
            {
                frameData.Write(_groupDependentData);
            }

            return _frameHeader.GetBytes(frameData, tagVersion, GetFrameID(tagVersion));
        }
    }
}
