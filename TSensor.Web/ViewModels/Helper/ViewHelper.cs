using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}