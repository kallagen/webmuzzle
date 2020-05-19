using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TSensor.Web.ViewModels.Helper
{
    public static class ViewHelper
    {
        public static HtmlString ValidState(this IHtmlHelper html, string field)
        {
            return new HtmlString(
                html?.ViewData?.ModelState[field]?.ValidationState == ModelValidationState.Invalid ?
                    " has-error" : null);
        }

        public static HtmlString ViewModelWindow(this IHtmlHelper html)
        {
            var viewModel = html?.ViewData?.Model as ViewModelBase;
            if (viewModel?.IsError == true)
            {
                return new HtmlString($@"
<script>
    new ErrorModal('{viewModel.ErrorMessage}').show();
</script>");
            }
            else if (viewModel?.IsSuccess == true)
            {
                return new HtmlString($@"
<script>
    new SuccessModal('{viewModel?.SuccessMessage}').show();
</script>");
            }
            else
            {
                return null;
            }
        }

        public static HtmlString MenuElementCheckbox(this IHtmlHelper html, Guid guid, Guid? parentGuid)
        {
            var selectedMenuItems = html?.ViewBag.SelectedMenuElements as IEnumerable<Guid>;
            var isChecked = selectedMenuItems?.Any(p => p == guid) == true ? " checked=\"checked\"" : string.Empty;

            return new HtmlString(
                $"<input type=\"checkbox\" {isChecked} data-parent=\"{parentGuid}\" data-guid=\"{guid}\" class=\"menu-checkbox\" />");
        }

        public static HtmlString TrimLabel(this IHtmlHelper html, string label, int maxLength)
        {
            string result;

            var idx = 0;
            var lexems = (label ?? string.Empty).ToUpper().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(p => idx++);

            if (lexems.Sum(p => p.Value.Length) + lexems.Count() - 1 > maxLength)
            {
                var modified = lexems.Select(p =>
                {
                    var value = p.Value;
                    var isModified = false;

                    if (!Regex.IsMatch(p.Value, "[^А-ЯA-Z]"))
                    {
                        value = p.Value.Substring(0, 1);
                        isModified = true;
                    }

                    return new { value, isModified };
                });

                result = string.Empty;
                var isLastModified = false;

                foreach (var item in modified)
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        isLastModified = item.isModified;
                        result = item.value;
                    }
                    else
                    {
                        if (isLastModified != item.isModified || (!isLastModified && !item.isModified))
                        {
                            result += " ";
                        }

                        isLastModified = item.isModified;
                        result += item.value;
                    }
                }
            }
            else
            {
                result = string.Join(' ', lexems.Select(p => p.Value));
            }

            return new HtmlString($"<span title=\"{label}\">{result}</span>");
        }
    }
}