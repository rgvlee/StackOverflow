using FluentValidation;
using Question60078465.MVC.Models;

namespace Question60078465.MVC.Validators
{
    public class SystemSettingsValidator : AbstractValidator<SystemSettings>
    {
        public SystemSettingsValidator()
        {
            RuleFor(x => x.EmailFromAddress).NotEmpty()
                .WithMessage("Supply an email from address please");
        }
    }
}