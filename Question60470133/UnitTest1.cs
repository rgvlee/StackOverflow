using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Question60470133
{
    public class Report
    {
        public string FinalDate { get; set; }
        public string Type { get; set; }
    }

    public class MyValidator1 : AbstractValidator<Report>
    {
        public MyValidator1()
        {
            RuleFor(report => report.FinalDate)
                .NotNull()
                .NotEmpty().When(report => report.Type.Equals("P")).WithMessage("A valid final date must be provided for this report");
        }
    }

    public class MyValidator2 : AbstractValidator<Report>
    {
        public MyValidator2()
        {
            RuleFor(report => report.FinalDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty().When(report => report.Type.Equals("P")).WithMessage("A valid final date must be provided for this report");
        }
    }

    public class UnitTest1
    {
        [Test]
        public void FinalDate_Null_HasTwoErrors()
        {
            var dto = new Report { Type = "P" };
            var validator = new MyValidator1();

            var validationResult = validator.TestValidate(dto);

            Assert.That(validationResult.Errors.Count, Is.EqualTo(2));
        }

        [Test]
        public void FinalDate_Null_HasOneError()
        {
            var dto = new Report { Type = "P" };
            var validator = new MyValidator2();

            var validationResult = validator.TestValidate(dto);

            Assert.That(validationResult.Errors.Count, Is.EqualTo(1));
        }
    }
}