using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core.AnimationMetrics;

namespace LiveTileWinUI3.Utility.Animation
{
    public class Appearance(UIElement target) : AnimationBase(target)
    {
        public EasingFunctionBase TransitionEasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
        public double Translate = 10;
        public double Opacity = 0;
        public bool Horizontal = true;

        public Storyboard Start(bool? horizontal = null)
        {
            if (Target.RenderTransform is not TranslateTransform)
                Target.RenderTransform = new TranslateTransform();

            DoubleAnimation opacityAnimation = new()
            {
                From = Opacity,
                To = 1,
                Duration = TransitionDuration
            };
            DoubleAnimation transformAnimtion = new()
            {
                From = Translate,
                To = 0,
                EasingFunction = TransitionEasingFunction,
                Duration = TransitionDuration
            };

            var storyboard = new Storyboard();
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            horizontal ??= Horizontal;
            if (!(bool)horizontal) Storyboard.SetTargetProperty(transformAnimtion, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            else Storyboard.SetTargetProperty(transformAnimtion, "(UIElement.RenderTransform).(TranslateTransform.X)");

            Storyboard.SetTarget(opacityAnimation, Target);
            Storyboard.SetTarget(transformAnimtion, Target);

            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(transformAnimtion);

            storyboard.Begin();

            return storyboard;
        }
    }
}
