using System;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Question60893578.AsAskedUsingChildRules
{
    public class Properties
    {
        public Properties(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class TestClass
    {
        public TestClass(Properties[] testProperties)
        {
            Id = Guid.NewGuid();
            TestProperties = testProperties;
        }

        public Properties[] TestProperties { get; set; }
        public Guid Id { get; set; }
    }

    public class TestClassValidator : AbstractValidator<TestClass>
    {
        public TestClassValidator()
        {
            RuleForEach(x => x.TestProperties)
                .ChildRules(testProperties =>
                {
                    testProperties.RuleFor(testProperty => testProperty.Name)
                        .NotNull()
                        .NotEmpty()
                        .WithMessage(testProperty => $"TestProperty {testProperty.Id} name can't be null or empty");
                });
        }
    }

    public class Tests
    {
        [Test]
        public void TestProperties_PropertyWithEmptyName_HasCustomError()
        {
            var property = new Properties(string.Empty);
            var testClass = new TestClass(new[] { property });

            var validator = new TestClassValidator();

            var validatorResult = validator.TestValidate(testClass);

            validatorResult.ShouldHaveValidationErrorFor("TestProperties[0].Name").WithErrorMessage($"TestProperty {property.Id} name can't be null or empty");
        }

        [Test]
        public void TestProperties_PropertyWithNullName_HasCustomError()
        {
            var property = new Properties(null);
            var testClass = new TestClass(new[] { property });

            var validator = new TestClassValidator();

            var validatorResult = validator.TestValidate(testClass);

            validatorResult.ShouldHaveValidationErrorFor("TestProperties[0].Name").WithErrorMessage($"TestProperty {property.Id} name can't be null or empty");
        }

        [Test]
        public void TestProperties_PropertyWithName_HasNoError()
        {
            var property = new Properties("asdf");
            var testClass = new TestClass(new[] { property });

            var validator = new TestClassValidator();

            validator.ShouldNotHaveValidationErrorFor(x => x.TestProperties, testClass);
        }
    }
}