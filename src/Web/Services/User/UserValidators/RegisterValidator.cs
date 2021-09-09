using System.Data;
using FluentValidation;
using Web.Models.Dtos;

namespace Web.Services.User.UserValidators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(u => u.Name).NotEmpty().WithMessage("نام نمی تواند خالی باشد");
            RuleFor(u => u.Lastname).NotEmpty().WithMessage("نام خانوادگی نمی تواند خالی باشد");
            RuleFor(u => u.Username).NotEmpty().WithMessage("نام کاربری نمی تواند خالی باشد");
            RuleFor(u => u.Password).NotEmpty().WithMessage("رمز نمی تواند خالی باشد");
        }
    }
}