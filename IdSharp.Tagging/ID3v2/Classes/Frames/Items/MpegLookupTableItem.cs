using System.ComponentModel;

namespace IdSharp.Tagging.ID3v2.Frames.Items;

internal sealed class MpegLookupTableItem : IMpegLookupTableItem
{
    private long _deviationInBytes;
    private long _deviationInMilliseconds;

    public event PropertyChangedEventHandler PropertyChanged;

    public long DeviationInBytes
    {
        get => _deviationInBytes;
        set
        {
            _deviationInBytes = value;
            RaisePropertyChanged(nameof(DeviationInBytes));
        }
    }

    public long DeviationInMilliseconds
    {
        get => _deviationInMilliseconds;
        set
        {
            _deviationInMilliseconds = value;
            RaisePropertyChanged(nameof(DeviationInMilliseconds));
        }
    }

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
