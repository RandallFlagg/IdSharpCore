using System.ComponentModel;

namespace IdSharp.Tagging.ID3v2.Frames.Items;

internal sealed class InvolvedPerson : IInvolvedPerson
{
    private string _name;
    private string _involvement;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            RaisePropertyChanged(nameof(Name));
        }
    }

    public string Involvement
    {
        get => _involvement;
        set
        {
            _involvement = value;
            RaisePropertyChanged(nameof(Involvement));
        }
    }

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
