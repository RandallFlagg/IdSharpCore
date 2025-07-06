using System.ComponentModel;

namespace IdSharp.Tagging.ID3v2.Frames.Items;

internal sealed class PriceInformation : IPriceInformation
{
    private string _currencyCode;
    private decimal _price;

    public event PropertyChangedEventHandler PropertyChanged;

    public string CurrencyCode
    {
        get => _currencyCode;
        set
        {
            _currencyCode = value;
            RaisePropertyChanged("CurrencyCode");
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            _price = value;
            RaisePropertyChanged("Price");
        }
    }

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
