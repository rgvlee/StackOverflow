using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Question60639055
{
    public class Bar
    {
        public int Foo { get; set; }
    }

    public class BarValidator : AbstractValidator<Bar>
    {
        public BarValidator()
        {
            RuleFor(c => c.Foo).GreaterThan(0);
        }
    }

    public class BarValidatorTests
    {
        [Test]
        public void Foo_Zero_HasValidationError()
        {
            var validator = new BarValidator();
            var dto = new Bar { Foo = 0 };

            validator.ShouldHaveValidationErrorFor(x => x.Foo, dto);
        }

        [Test]
        public void Foo_One_DoesNotHaveValidationError()
        {
            var validator = new BarValidator();
            var dto = new Bar { Foo = 1 };

            validator.ShouldNotHaveValidationErrorFor(x => x.Foo, dto);
        }
    }

    public class BarValidatorTestsUsingTestValidate
    {
        [Test]
        public void Foo_Zero_HasValidationError()
        {
            var validator = new BarValidator();
            var dto = new Bar { Foo = 0 };

            var validationResults = validator.TestValidate(dto);

            validationResults.ShouldHaveValidationErrorFor(x => x.Foo);
        }

        [Test]
        public void Foo_One_DoesNotHaveValidationError()
        {
            var validator = new BarValidator();
            var dto = new Bar { Foo = 1 };

            var validationResults = validator.TestValidate(dto);

            validationResults.ShouldNotHaveValidationErrorFor(x => x.Foo);
        }
    }
}