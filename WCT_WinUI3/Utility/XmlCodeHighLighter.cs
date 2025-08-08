using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI;

namespace WCT_WinUI3.Utility
{
    public partial class XmlCodeHighLighter
    {
        public enum HighLightResultType
        {
            SyntaxError,
            Finished
        }

        private Action? operationCancelToken = null;
        private string lastText = string.Empty;
        private static readonly Regex XmlRegex = new(
            @"(<!--.*?-->)|" +
            @"(?<=</?)([\w\d\:]+)|" +
            @"(?<=\s)([\w\d\:]+)(?=\s*=)|" +
            @"("".*?""|'.*?')|" +
            @"(</?|/?>|=)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        public TimeSpan DebounceTime = TimeSpan.FromMilliseconds(600);
        public Color DefaultColor = Colors.White;
        public Color ElementColor = Color.FromArgb(255, 80, 150, 255);
        public Color AttributeNameColor = Color.FromArgb(200, 60, 120, 220);
        public Color AttributeValueColor = Color.FromArgb(255, 206, 110, 85);
        public Color CommentColor = Color.FromArgb(255, 87, 166, 74); 
        public Color PunctuationColor = Colors.Gray;

        public event EventHandler<HighLightResultType>? HighLightOver;

        public XmlCodeHighLighter()
        {

        }

        public XmlCodeHighLighter(TimeSpan debounceTime)
        {
            DebounceTime = debounceTime;
        }

        public void TextChangedEventHandler (object sender, RoutedEventArgs e)
        {
            if (sender is RichEditBox editor)
                Debounce(editor);
        }

        private void Debounce(RichEditBox editor)
        {
            operationCancelToken?.Invoke();
            operationCancelToken = Timer.SetTimeout(() => HighLight(editor, this, false), DebounceTime);
        }

        private static void PutColor(RichEditBox editor, Group group, Color color)
        {
            if (!group.Success) return;
            ITextRange range = editor.Document.GetRange(group.Index, group.Index + group.Length);
            range.CharacterFormat.ForegroundColor = color;
        }

        public void CancelAutoHighLight() => operationCancelToken?.Invoke();

        public static void HighLight(RichEditBox editor, XmlCodeHighLighter hl, bool ignoreTextNotChanged = true)
        {
            editor.Document.GetText(TextGetOptions.None, out string currentText);

            if (string.IsNullOrWhiteSpace(currentText))
            {
                hl.HighLightOver?.Invoke(hl, HighLightResultType.Finished);
                return;
            }

            if (!ignoreTextNotChanged && currentText == hl.lastText)
                return;
            else hl.lastText = currentText;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(currentText);
            }
            catch
            {
                hl.HighLightOver?.Invoke(hl, HighLightResultType.SyntaxError);
                return;
            }

            editor.Document.BeginUndoGroup();
            editor.Document.BatchDisplayUpdates();
            try
            {

                Color defaultColor = editor.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

                ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
                if (documentRange.CharacterFormat.ForegroundColor != defaultColor)
                {
                    documentRange.CharacterFormat.ForegroundColor = defaultColor;
                }

                foreach (Match match in XmlRegex.Matches(currentText))
                {
                    if (match.Groups[1].Success)
                        PutColor(editor, match.Groups[1], hl.CommentColor);
                    else if (match.Groups[2].Success)
                        PutColor(editor, match.Groups[2], hl.ElementColor);
                    else if (match.Groups[3].Success)
                        PutColor(editor, match.Groups[3], hl.AttributeNameColor);
                    else if (match.Groups[4].Success)
                        PutColor(editor, match.Groups[4], hl.AttributeValueColor);
                    else if (match.Groups[5].Success)
                        PutColor(editor, match.Groups[5], hl.PunctuationColor);
                }
            }
            finally
            {
                editor.Document.ApplyDisplayUpdates();
                editor.Document.EndUndoGroup();
                editor.Document.UndoLimit--;
                hl.HighLightOver?.Invoke(hl, HighLightResultType.Finished);
            }
            
        }
    }

}
