using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Web.Infrastructure.Extensions;

namespace Web.Infrastructure.Api
{
    public class ApiResult
    {
        public string Message { get; set; }
        public ApiResultStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; }

        public ApiResult(ApiResultStatusCode statusCode, List<string> errors, string message = null)
        {
            Errors = errors;
            Message = message ?? statusCode.ToDisplay();
            StatusCode = statusCode;
        }

        #region implicit Operator

        public static implicit operator ApiResult(NotFoundResult result)
        {
            return new ApiResult(ApiResultStatusCode.NotFound, null);
        }

        public static implicit operator ApiResult(NotFoundObjectResult result)
        {
            return new ApiResult(ApiResultStatusCode.NotFound, null, result.Value.ToString());
        }

        public static implicit operator ApiResult(UnauthorizedResult result)
        {
            return new ApiResult(ApiResultStatusCode.Unauthorized, null);
        }

        public static implicit operator ApiResult(OkObjectResult result)
        {
            return new ApiResult(ApiResultStatusCode.Success, null, result.Value.ToString());
        }

        public static implicit operator ApiResult(OkResult result)
        {
            return new ApiResult(ApiResultStatusCode.Success, null);
        }

        public static implicit operator ApiResult(BadRequestResult result)
        {
            return new ApiResult(ApiResultStatusCode.BadRequest, null);
        }

        #endregion
    }

    public class ApiResult<TData> : ApiResult
    {
        public TData Data { get; set; }

        public ApiResult(TData data, ApiResultStatusCode statusCode, List<string> errors, string message = null)
            : base(statusCode, errors, message)
        {
            Data = data;
        }

        #region implicit Operator

        public ApiResult(ApiResultStatusCode statusCode, List<string> errors, string message = null)
            : base(statusCode, errors, message)
        {
        }


        public static implicit operator ApiResult<TData>(TData value)
        {
            return new ApiResult<TData>(value, ApiResultStatusCode.Success, null);
        }

        public static implicit operator ApiResult<TData>(NotFoundResult result)
        {
            return new ApiResult<TData>(ApiResultStatusCode.NotFound, null);
        }

        public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
        {
            return new ApiResult<TData>(ApiResultStatusCode.NotFound, null);
        }

        public static implicit operator ApiResult<TData>(UnauthorizedResult result)
        {
            return new ApiResult<TData>(ApiResultStatusCode.Unauthorized, null);
        }


        public static implicit operator ApiResult<TData>(OkObjectResult result)
        {
            return new ApiResult<TData>((TData)result.Value, ApiResultStatusCode.Success, null);
        }

        public static implicit operator ApiResult<TData>(BadRequestResult result)
        {
            return new ApiResult<TData>(ApiResultStatusCode.BadRequest, null);
        }

        public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
        {
            return new ApiResult<TData>(ApiResultStatusCode.BadRequest, null, result.Value.ToString());
        }

        #endregion
    }
}