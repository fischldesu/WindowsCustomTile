using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fischldesu.WCTCore;

namespace WCT_WinUI3.Pages
{
    public sealed partial class PageTemplate : ContentControl
    {
        #region DependencyProperties
        public static readonly DependencyProperty FooterContentProperty = DependencyProperty.Register(
                nameof(FooterContent),
                typeof(UIElement),
                typeof(PageTemplate),
                new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register(
                nameof(Header),
                typeof(string),
                typeof(PageTemplate),
                new PropertyMetadata(string.Empty));

        public UIElement? FooterContent
        {
            get { return (UIElement)GetValue(FooterContentProperty); }
            set { SetValue(FooterContentProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        #endregion

        private InfoBar? infoBar;

        public PageTemplate()
        {
            DefaultStyleKey = typeof(PageTemplate);
        }

        public bool ShowInfo(string? title, string? message, InfoBarSeverity severity, bool closable = true, Microsoft.UI.Xaml.Controls.Primitives.ButtonBase? actionButton = null)
        {
            if (infoBar != null)
            {
                infoBar.Title = title;
                infoBar.Message = message ?? string.Empty;
                infoBar.Severity = severity;
                infoBar.IsClosable = closable;
                infoBar.ActionButton = actionButton;
                infoBar.IsOpen = true;
                return true;
            }
            else
            {
                Log.Warning("Page ShowInfo: InfoBar not exist");
                return false;
            }
        }

        public void CloseInfoBar()
        {
            if (infoBar != null)
                infoBar.IsOpen = false;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            infoBar = GetTemplateChild("infoBar") as InfoBar;
        }
    }
}
