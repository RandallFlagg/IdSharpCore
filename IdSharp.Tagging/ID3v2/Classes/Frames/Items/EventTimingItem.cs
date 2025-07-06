using System;
using System.ComponentModel;

namespace IdSharp.Tagging.ID3v2.Frames.Items;

internal sealed class EventTimingItem : IEventTimingItem
{
    private MusicEvent _eventType;
    private int _timestamp;

    public event PropertyChangedEventHandler PropertyChanged;

    public MusicEvent EventType
    {
        get => _eventType;
        set
        {
            _eventType = value;
            RaisePropertyChanged(nameof(EventType));
        }
    }

    public int Timestamp
    {
        get => _timestamp;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be less than 0");
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
