using System.ComponentModel;

namespace LiveTileWinUI3.Components;

public partial class ImageTilePreviewerSourceSet : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _large = string.Empty;
    private string _wide = string.Empty;
    private string _medium = string.Empty;
    private string _small = string.Empty;

    public string Large
    {
        get => _large;
        set
        {
            if (_large != value)
            {
                _large = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Large)));
            }
        }
    }

    public string Wide
    {
        get => _wide;
        set
        {
            if (_wide != value)
            {
                _wide = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wide)));
            }
        }
    }

    public string Medium
    {
        get => _medium;
        set
        {
            if (_medium != value)
            {
                _medium = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Medium)));
            }
        }
    }

    public string Small
    {
        get => _small;
        set
        {
            if (_small != value)
            {
                _small = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Small)));
            }
        }
    }
}
