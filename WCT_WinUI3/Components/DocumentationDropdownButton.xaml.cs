using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
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

namespace WCT_WinUI3.Components
{
    [ContentProperty(Name = "FlyoutChildren")]
    public sealed partial class DocumentationDropdownButton : UserControl
    {
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(PageFooter),
            new PropertyMetadata(null));

        public DocumentationDropdownButton()
        {
            this.InitializeComponent();
        }

        public UIElementCollection FlyoutChildren
        {
            get { return this.flyoutContent.Children; }
        }
    }
}
