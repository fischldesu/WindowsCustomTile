using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LiveTileWinUI3.Components
{
    public sealed partial class XmlUriInput : UserControl
    {
        public static readonly DependencyProperty DependencyHeader = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(XmlUriInput),
            new PropertyMetadata("Text input"));

        public event EventHandler<XmlUriInput> RequestRemove = delegate { };

        public Uri? Uri
        {
            get
            {
                _ = Uri.TryCreate(input.Text, UriKind.Absolute, out var uri);
                return uri;
            }
        }

        public string Header
        {
            get => (string)GetValue(DependencyHeader);
            set => SetValue(DependencyHeader, value);
        }

        public XmlUriInput()
        {
            this.InitializeComponent();
        }

        public void Remove()
        {
            RequestRemove?.Invoke(this, this);
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            Remove();
        }
    }
}
