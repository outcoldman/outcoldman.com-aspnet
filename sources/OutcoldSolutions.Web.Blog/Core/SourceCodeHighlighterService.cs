// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using CodeSnippet.Config;
    using CodeSnippet.Formats;
    using CodeSnippet.Formats.Base;

    public class SourceCodeHighlighterService
    {
        private readonly StyleConfig _config = new StyleConfig()
            {
                AlternateLines = true, 
                EmbedStyles = true, 
                LineNumbers = false, 
                UseContainer = true, 
                AltevenStyle = "margin: 0em;", 
                AltStyle = "background-color: white; margin: 0em;", 
                AspStyle = "background-color: #ffff00;", 
                AttrStyle = "color: #ff0000;", 
                ClsStyle = "color: #cc6633;", 
                CodeStyle = "padding: 0px 0px 0px 0px;", 
                HtmlStyle = "color: #800000;", 
                KwrdStyle = "color: #0000ff;", 
                LnumStyle = "color: #606060;", 
                OpStyle = "color: #0000c0;", 
                PreprocStyle = "color: #cc6633;", 
                PreStyle = "margin: 0em;", 
                RemStyle = "color: #008000;", 
                StrStyle = "color: #006080;", 
                WrapperStyle =
                    "border: silver 1px solid; text-align: left; padding: 4px 4px 4px 4px; line-height: 10pt; background-color: #f4f4f4; margin: 20px 0px 10px; width: 97.5%; font-family: Tahoma, Verdana, courier, monospace; direction: ltr; color: black; overflow: auto; cursor: text; font-size:9pt;"
            };

        private readonly SortedDictionary<string, SupportedFormatType> _supportedFormats =
            new SortedDictionary<string, SupportedFormatType>()
                {
                    { "cs", SupportedFormatType.CSharp }, 
                    { "js", SupportedFormatType.JavaScript }, 
                    { "html", SupportedFormatType.Html }, 
                    { "xml", SupportedFormatType.Html }, 
                    { "sql", SupportedFormatType.Tsql }, 
                    { "css", SupportedFormatType.Css }, 
                    { "cpp", SupportedFormatType.CCpp }, 
                    { "java", SupportedFormatType.Java }, 
                    { "php", SupportedFormatType.Php }, 
                };

        public List<string> GetSupportedFormats()
        {
            return this._supportedFormats.Keys.ToList();
        }

        public string HighlightCode(string inputCode, string type)
        {
            SupportedFormatType ftype = SupportedFormatType.AutoIt;

            if (this._supportedFormats.ContainsKey(type))
            {
                ftype = this._supportedFormats[type];
            }

            SupportedFormat supportedFormat = SupportedFormat.GetItem(ftype);
            SourceFormat format = supportedFormat.NewFormatInstance();
            format.Editor.TabSpaces = 4;
            format.Editor.TrimIndentOnPaste = true;
            format.Editor.WordWrap = false;
            format.Style = this._config;
            return format.FormatCode(inputCode);
        }
    }
}