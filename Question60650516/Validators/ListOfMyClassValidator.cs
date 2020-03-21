using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Question60650516.Models;

namespace Question60650516.Validators
{
    public class ListOfMyClassValidator : AbstractValidator<List<MyClass>>
    {
        public ListOfMyClassValidator()
        {
            RuleFor(x => x).Must(x => x.Any()).WithMessage("The list cannot be empty");

            RuleForEach(x => x).SetValidator(new MyClassValidator());
        }
    }
}