using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Web.Domain;
using Web.Models.Dtos;

namespace Web.Services.User.UserValidators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator(RaimunDbContext ctx)
        {
            RuleFor(u => u.Username).NotEmpty().WithMessage("نام کاربری نمی تواند خالی باشد");

            RuleFor(u => u.Password).NotEmpty().WithMessage("رمز عبور نمی تواند خالی باشد");

            RuleFor(x => x)
                .CustomAsync(async (x, context, cancellationToken) =>
                {
                    var user = await ctx.Users
                        .SingleOrDefaultAsync(u => u.Username == x.Username && u.Password == x.Password,
                            cancellationToken);

                    if (user == null)
                        context.AddFailure("نام کاربری یا رمز عبور اشتباه است");
                });
        }
    }
}