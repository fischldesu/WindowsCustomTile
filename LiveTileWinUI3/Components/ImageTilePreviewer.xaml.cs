using LiveTileWinUI3.Utility;
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

    public sealed partial class ImageTilePreviewer : UserControl
    {
        public static readonly DependencyProperty DependencyUnitSize = DependencyProperty.Register(
            nameof(UnitSize), typeof(GridLength), typeof(ImageTilePreviewer), new PropertyMetadata(new GridLength(49), OnLayoutPropertyChanged));
        public static readonly DependencyProperty DependencyGap = DependencyProperty.Register(
            nameof(Gap), typeof(double), typeof(ImageTilePreviewer), new PropertyMetadata(3.0, OnLayoutPropertyChanged));
        public static readonly DependencyProperty DependencySourceSet = DependencyProperty.Register(
            nameof(Source), typeof(ImageTilePreviewerSourceSet), typeof(ImageTilePreviewer), new PropertyMetadata(new ImageTilePreviewerSourceSet()));

        public event EventHandler<TileSize>? ImageButtonFlyoutClick = delegate { };

        public GridLength UnitSize
        {
            get => (GridLength)GetValue(DependencyUnitSize);
            set => SetValue(DependencyUnitSize, value);
        }
        
        public double Gap
        {
            get => (double)GetValue(DependencyGap);
            set => SetValue(DependencyGap, value);
        }

        public ImageTilePreviewerSourceSet Source
        {
            get => (ImageTilePreviewerSourceSet)GetValue(DependencySourceSet);
            set => SetValue(DependencySourceSet, value);
        }

        private readonly MenuFlyout flyout = new();
        private readonly MenuFlyoutItem flyoutItem = new();

        public ImageTilePreviewer()
        {
            this.InitializeComponent();
            foreach (var item in grid.Children)
            {
                if (item != null && item is Button item_)
                {
                    item_.HorizontalAlignment = HorizontalAlignment.Stretch;
                    item_.VerticalAlignment = VerticalAlignment.Stretch;
                    item_.CornerRadius = new CornerRadius(0);
                    item_.Padding = new Thickness(0);
                }
            }

            flyout.Placement = FlyoutPlacementMode.RightEdgeAlignedTop;
            flyout.Items.Add(flyoutItem);
            flyoutItem.Text = "Edit";
            flyoutItem.Click += (obj, _) =>
            {
                var lastClicked = GetLastClicked();
                if (lastClicked != null)
                {
                    ImageButtonFlyoutClick?.Invoke(flyout, (TileSize)lastClicked);
                    LastClicked = null;
                }
            };
        }

        private TileSize? GetLastClicked()
        {
            return LastClicked;
        }

        private TileSize? LastClicked = null;

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null || sender.GetType() != typeof(Button))
                return;
            
            var button = (Button)sender;
            flyout.ShowAt(button);
            LastClicked = (button.Tag as string)?.ToLower() switch
            {
                "large" => TileSize.Large,
                "wide" => TileSize.Wide,
                "medium" => TileSize.Medium,
                "small" => TileSize.Small,
                _ => null
            };
        }

        private static void OnLayoutPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ImageTilePreviewer ins)
            {
                foreach (var row in ins.grid.RowDefinitions)
                    row.Height = ins.UnitSize;
                foreach (var col in ins.grid.ColumnDefinitions)
                    col.Width = ins.UnitSize;
                ins.grid.RowSpacing = ins.Gap;
                ins.grid.ColumnSpacing = ins.Gap;
            }
        }

        public string GetXml()
        {
            return TileXmlEditor.FromTemplate(
                $"<image src='{Source.Small}' placement='background' />",
                $"<image src='{Source.Medium}' placement='background' />",
                $"<image src='{Source.Wide}' placement='background' />",
                $"<image src='{Source.Large}' placement='background' />");
        }
    }
}
