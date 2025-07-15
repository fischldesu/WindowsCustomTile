using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCT_WinUI3.Utility.Animation
{
    public class AnimationBase
    {
        public UIElement Target;
        public Duration TransitionDuration;

        protected AnimationBase(UIElement target)
        {
            Target = target;
            TransitionDuration = new Duration(TimeSpan.FromMilliseconds(300));
        }
    }
}
