using System;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

namespace ProductiveMvc
{
    public static class ProductiveHelper
    {
        /// <summary>
        /// Shortcut method to format a string to TitleCase without needing to setup Globalization and Threading each time.
        /// </summary>
        /// <param name="input">Any String</param>
        /// <returns>TitleCaseString</returns>
        public static string TitleCase(string input)
        {
            if (input != null)
            {
                // For TitleCase
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;

                return textInfo.ToTitleCase(input);
            }
            else
            {
                return "";
            }
        }
        
        /// <summary>
        /// Searches for a needle string in a haystack string. If found, the needle will be wrapped in a span class.
        /// </summary>
        /// <param name="needle">A word or string you are searching for. (Case Insensitive)</param>
        /// <param name="haystack">The string of text you wish to search in.</param>
        /// <param name="highlightCssClass">The CSS class name(s) to wrap around the found word. Defaults to 'highlight'</param>
        /// <returns>The haystack MvcHtmlString with any found needles wrapped in a span tag.</returns>
        public static MvcHtmlString HighlightWord(string needle = "", string haystack = "", string highlightCssClass = "highlight")
        {
            // both must contain something in order for a search to occur
            if (haystack == null || needle == null || needle.Trim().Length == 0)
            {
                // return the haystack unless it is null
                return new MvcHtmlString((haystack != null ? haystack : ""));
            }
            else
            {
                // Replace the searched string with a copy wrapped with a span.highlight tag.

                // $& is the matched text in the REGEX expression
                // See: http://www.knowdotnet.com/articles/regereplacementstrings.html
                // See: http://stackoverflow.com/a/1139465/177260

                string replacement = "<span class=\"" + highlightCssClass + "\">$&</span>";
                string result = Regex.Replace(haystack, needle, replacement, RegexOptions.IgnoreCase);

                return new MvcHtmlString(result);
            }
        }
        
        /// <summary>
        /// Formats a ViewBag date string as if it was a DateTime object
        /// </summary>
        /// <param name="rawDate">Date string</param>
        /// <param name="dateFormat">Optional format. Default 'MM/d/yy'</param>
        /// <returns>A formated date string</returns>
        public static MvcHtmlString ShortDate(DateTime? rawDate, string dateFormat = "MM/d/yy")
        {
            if (rawDate != null)
            {
                // http://stackoverflow.com/questions/1091870/how-to-convert-datetime-to-datetime
                // Converting to a non-nullable datetime so that we have access to ToShortDateString()
                // But knowing that we won't ever get to "DateTime.Now" since we are testing for Null above
                DateTime StatusDate = rawDate ?? DateTime.Now;
                return new MvcHtmlString(StatusDate.ToString(dateFormat));
            }
            else
                return new MvcHtmlString("");
        }

        /// <summary>
        /// Formats a ViewBag date string as if it was a DateTime object
        /// </summary>
        /// <param name="rawDate">Date string</param>
        /// <param name="dateFormat">Optional format. Default 'MM/d/yy'</param>
        /// <returns>A formated date string</returns>
        public static MvcHtmlString ShortDateHighlightNew(DateTime? rawDate, int days = 7, string dateFormat = "MM/d/yy")
        {
            if (rawDate != null)
            {
                // http://stackoverflow.com/questions/1091870/how-to-convert-datetime-to-datetime
                // Converting to a non-nullable datetime so that we have access to ToShortDateString()
                // But knowing that we won't ever get to "DateTime.Now" since we are testing for Null above
                DateTime StatusDate = rawDate ?? DateTime.Now;
                String result = StatusDate.ToString(dateFormat);

                bool isNew = (StatusDate - DateTime.Now).Days >= days;

                if (isNew)
                {
                    result = "<span class=\"new\">" + result + "</span>";
                }

                return new MvcHtmlString(result);
            }
            else
                return new MvcHtmlString("");
        }

        /// <summary>
        /// Outputs a single Html Select Option, setting it as 'selected' if it matches the selected string. (case insensitive)
        /// Different from Html.DropDownList, which outputs the full select list.
        /// This is better for creating a few known options.
        /// For Html.DropDownList, see: http://www.mikesdotnetting.com/Article/128/Get-The-Drop-On-ASP.NET-MVC-DropDownLists
        /// </summary>
        /// <param name="val">Value of this option</param>
        /// <param name="labelText">Text to display</param>
        /// <param name="currentlySelected">The currently selected item to compare with this value</param>
        /// <returns>An MvcHtmlString with an HTML Option tag</returns>
        public static MvcHtmlString DropDownOption(string val, string labelText, string currentlySelected = "", string CssClass = "")
        {
            string OptionTagPattern = "<option value=\"{0}\" class=\"{1}\" {2}>{3}</option>";
            string isSelected = "";

            // if currently selected is not blank, and matches val(ue) without regard to case,
            // set this option as 'selected'
            if (currentlySelected != null && currentlySelected.Trim().Length > 0 && val.ToLower() == currentlySelected.ToLower())
                isSelected = "selected";

            // Outputs the same as:
            // <option value="Pending" @if (!String.IsNullOrEmpty(Model.Status.ToString()) && Model.Status.Trim().ToLower() == "pending")
            //   {<text>selected</text>}>Pending</option>

            return new MvcHtmlString(String.Format(OptionTagPattern, val, CssClass, isSelected, labelText));
        }

        /// <summary>
        /// Creates a Cancel/Back button that relies on javascript and browser history
        /// </summary>
        /// <param name="label">Button Label. Default: Cancel</param>
        /// <param name="cssClass">CSS Classes. Default: btn</param>
        /// <returns>MvcHtmlString Anchor link tag using JavaScript History -1 as the URL.</returns>
        public static MvcHtmlString JsHistoryBackButton(string label = "Cancel", string cssClass = "btn")
        {
            // Originally tried: <a href="#" onclick="window.history.back();" class="btn">← Back to List</a>
            // But found that it was not as compatible as the one used below.
            // See: http://blog.tcs.de/fix-historyback-not-working-in-google-chrome-safari-and-webkit/
            // This back button relies on javascript and browser history - see: http://www.w3schools.com/jsref/met_his_back.asp

            string ButtonPattern = "<a href=\"javascript:history.go(-1);\" class=\"{0}\">{1}</a>";
            return new MvcHtmlString(String.Format(ButtonPattern, cssClass, label));
        }
    }
}