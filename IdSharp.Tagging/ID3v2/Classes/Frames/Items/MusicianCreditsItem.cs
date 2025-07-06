using System;
using System.ComponentModel;

namespace IdSharp.Tagging.ID3v2.Frames.Items;

internal sealed class MusicianCreditsItem : IMusicianCreditsItem
{
    private string _instrument;
    private string _artists;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Instrument
    {
        get => _instrument;
        set
        {
            _instrument = value;
            RaisePropertyChanged(nameof(Instrument));
        }
    }

    public string Artists
    {
        get => _artists;
        set
        {
            _artists = value;
            RaisePropertyChanged(nameof(Artists));
        }
    }

    private void RaisePropertyChanged(String propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
