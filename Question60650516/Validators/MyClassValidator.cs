using FluentValidation;
using Question60650516.Models;

namespace Question60650516.Validators
{
    public class MyClassValidator : AbstractValidator<MyClass>
    {
        public MyClassValidator()
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.Code).NotEmpty();
        }
    }
}