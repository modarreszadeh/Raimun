using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Infrastructure.Api
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                SerializableError serialError = new SerializableError(context.ModelState);
                var errors = serialError.SelectMany(p => (string[])p.Value).Distinct();
                var apiResult = new ApiResult(ApiResultStatusCode.BadRequest, errors.ToList());
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(apiResult);
            }

            base.OnResultExecuting(context);
        }
    }
}