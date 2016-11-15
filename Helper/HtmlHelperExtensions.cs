using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace BYOD_manager.Helper
{

    public static class HtmlHelperExtensions
    {
        public static System.Web.Mvc.MvcHtmlString ToggleSwitchFor<TModel>(this System.Web.Mvc.HtmlHelper<TModel> htmlHelper, System.Linq.Expressions.Expression<Func<TModel, bool>> expression)
        {
            return htmlHelper.DropDownListFor(expression, new[]
            {
new SelectListItem { Text = "No", Value = "false" },
new SelectListItem { Text = "Yes", Value = "true" }
}, new { @class = "toggleswitch" });
        }
    }

}