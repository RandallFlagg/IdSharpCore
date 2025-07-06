using System;
using System.ComponentModel;

namespace IdSharp.Tagging.ID3v2.Frames.Items;

internal sealed class SynchronizedTextItem : ISynchronizedTextItem
{
    private string _text;
    private int _timestamp;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            FirePropertyChanged(nameof(Text));
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
            FirePropertyChanged(nameof(Timestamp));
        }
    }

    private void FirePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
