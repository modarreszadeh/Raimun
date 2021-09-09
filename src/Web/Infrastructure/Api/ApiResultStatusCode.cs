using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Api
{
    public enum ApiResultStatusCode
    {
        [Display(Name = "عملیات با موفقیت انجام شد")]
        Success = 200,
        [Display(Name = "یافت نشد")] NotFound = 404,
        [Display(Name = "درخواست نا معتبر")] BadRequest = 400,
        [Display(Name = "خطا در سرور")] ServerError = 500,
        [Display(Name = "عدم دسترسی")] Unauthorized = 401
    }
}