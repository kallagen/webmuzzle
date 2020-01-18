using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public static HtmlString MenuPage(this IHtmlHelper html, string controllerName)
        {
            var routeData = html?.ViewContext.RouteData.Values;

            if (
                routeData?.ContainsKey("controller") == true &&
                routeData["controller"] as string == controllerName)
            {
                return new HtmlString("class=\"active\"");
            }
            else
            {
                return null;
            }
        }
    }
}