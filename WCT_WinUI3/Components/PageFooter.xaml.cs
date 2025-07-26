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
using WCT_WinUI3.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Components
{
    public sealed partial class PageFooter : UserControl
    {
        public UIElement? FrontContent
        {
            get { return (UIElement)GetValue(FrontContentProperty); }
            set { SetValue(FrontContentProperty, value); }
        }
        public static readonly DependencyProperty FrontContentProperty = DependencyProperty.Register(
            nameof(FrontContent),
            typeof(UIElement),
            typeof(PageFooter),
            new PropertyMetadata(null));

        public UIElement? RearContent
        {
            get { return (UIElement)GetValue(RearContentProperty); }
            set { SetValue(RearContentProperty, value); }
        }
        public static readonly DependencyProperty RearContentProperty = DependencyProperty.Register(
            nameof(RearContent),
            typeof(UIElement),
            typeof(PageFooter),
            new PropertyMetadata(null));

        public PageFooter()
        {
            this.InitializeComponent();
        }

    }
}
