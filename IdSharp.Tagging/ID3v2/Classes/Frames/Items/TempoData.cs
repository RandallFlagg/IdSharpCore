using System;
using System.ComponentModel;

namespace IdSharp.Tagging.ID3v2.Frames.Items;

internal sealed class TempoData : ITempoData
{
    private short _tempoCode;
    private int _timestamp;

    public event PropertyChangedEventHandler PropertyChanged;

    public short TempoCode
    {
        get => _tempoCode;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value cannot be less than 0");
            }

            _tempoCode = value;
            RaisePropertyChanged(nameof(TempoCode));
        }
    }

    public int Timestamp
    {
        get => _timestamp;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value cannot be less than 0");
            }

            _timestamp = value;
            RaisePropertyChanged(nameof(Timestamp));
        }
    }

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
