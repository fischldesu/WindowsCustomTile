using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using WCT_WinUI3.Utility;
using Microsoft.UI.Xaml.Media.Animation;
using WCT_WinUI3.Utility.Log;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WCT_WinUI3.Components
{

    public sealed partial class SlideShowTilePreviewer : UserControl
    {
        private List<ImageSource>? imageSources;
        private SlideShowAnimation? animation;
        public double UnitSize
        {
            get => (double)GetValue(UnitSizeProperty);
            set
            {
                SetValue(UnitSizeProperty, value);
                Update();
            }
        }
        public static readonly DependencyProperty UnitSizeProperty = DependencyProperty.Register(
            nameof(UnitSize),
            typeof(double),
            typeof(SlideShowTilePreviewer),
            new PropertyMetadata(49));
        public TileSize TileSize
        {
            get => (TileSize)GetValue(TileSizeProperty);
            set
            {
                SetValue(TileSizeProperty, value);
                Update();
            }
        }
        public static readonly DependencyProperty TileSizeProperty = DependencyProperty.Register(
            nameof(TileSize),
            typeof(TileSize),
            typeof(SlideShowTilePreviewer),
            new PropertyMetadata(TileSize.Medium));

        public event EventHandler? RequestPreview;
        public bool IsPreviewing { get; private set; } = false;

        public SlideShowTilePreviewer()
        {
            this.InitializeComponent();
        }

        public void Update()
        {
            const double gap = 3;
            container.Width = displayButton.Width = UnitSize * 4 + gap * 3;
            switch (TileSize)
            {
                case TileSize.Small:
                    displayButton.Width = UnitSize;
                    displayButton.Height = UnitSize;
                    break;
                case TileSize.Medium:
                    displayButton.Width = UnitSize * 2 + gap;
                    displayButton.Height = UnitSize * 2 + gap;
                    break;
                case TileSize.Large:
                    displayButton.Width = UnitSize * 4 + gap * 3;
                    displayButton.Height = UnitSize * 4 + gap * 3;
                    break;
                case TileSize.Wide:
                    displayButton.Width = UnitSize * 4 + gap * 3;
                    displayButton.Height = UnitSize * 2 + gap;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("size", TileSize, "Invalid new size for SlideShowTilePreviewer");
            }
        }

        public void SetSources(IEnumerable<ImageSource> sources)
        {
            imageSources = [.. sources];
        }

        public void StartPreview()
        {
            if (imageSources?.Count < 0)
                return;

            StopPreview();

            for (int i = 0; i < imageSources?.Count; i++)
            {
                var src = imageSources[i];
                var img = new Image()
                {
                    Source = src,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 },
                    Stretch = Stretch.UniformToFill,
                    Tag = src
                };
                grid.Children.Add(img);
                Canvas.SetZIndex(img, imageSources.Count - i);
            }

            if (grid.Children.Count == 0)
                return;
            
            animation?.Stop();
            animation = new SlideShowAnimation(SlideShowStoryboard, grid);
            animation.Start();
        }

        public void StopPreview()
        {
            grid.Children.Clear();
            animation?.Stop();
            animation = null;
        }

        private void displayButton_Click(object sender, RoutedEventArgs e)
        {
            if (SlideShowStoryboard.GetCurrentState() == ClockState.Stopped)
                RequestPreview?.Invoke(this, EventArgs.Empty);
            else animation?.Stop();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            grid.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, grid.ActualWidth, grid.ActualHeight)
            };
        }
    }

    internal class SlideShowAnimation(Storyboard storyboard, Grid targetContainer)
    {
        private readonly Storyboard animations = storyboard;
        private readonly Grid container = targetContainer;
        private Action? stop;

        public void Start()
        {
            if (container.Children.Count < 2)
                return;

            var index = -1;
            var list = container.Children.ToList();

            stop?.Invoke();
            stop = Timer.SetInterval(() =>
            {
                animations.Stop();

                if (++index >= list.Count)
                {
                    index = 0;
                    Canvas.SetZIndex(list.Last(), 0);
                    list.Last().Opacity = 1;
                }

                var ele = list[index] as Image;
                if (ele == null) return;

                if (ele == list.Last())
                {
                    Canvas.SetZIndex(list.Last(), list.Count);
                    foreach (var item in list)
                    {
                        if (item != ele)
                            item.Opacity = 1.0;
                    }
                }

                ResetTarget(ele);
                animations.Begin();
                Logger.Log($"ZI {Canvas.GetZIndex(ele)}| List {index}");
                Timer.SetTimeout(() => ele.Opacity = 0, animations.Children[0].Duration.TimeSpan);

            }, TimeSpan.FromSeconds(4));
        }

        private void ResetTarget(UIElement element)
        {
            if (element is not Image { RenderTransform: ScaleTransform scale } img) return;
            
            foreach (var anim in animations.Children)
            {
                if (anim is not DoubleAnimation animation) continue;
                
                if ( Storyboard.GetTargetProperty(animation) == "Opacity")
                    Storyboard.SetTarget(animation, img);
                else
                    Storyboard.SetTarget(animation, scale);
            }
        }

        public void Stop()
        {
            if (stop != null)
            {
                stop();
                stop = null;
            }
            animations.Stop();
        }

    }

}
