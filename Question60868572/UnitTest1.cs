using AutoFixture;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Question60868572
{
    public class User
    {
        public string Username { get; set; }
        public bool Found { get; set; }
    }

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Username).NotEmpty().When(x => x.Found);
        }
    }

    public class Tests
    {
        [Test]
        public void Validate_UsernameEmptyAndFoundTrue_HasError()
        {
            var user = new User { Username = string.Empty, Found = true };
            var validator = new UserValidator();

            validator.ShouldHaveValidationErrorFor(x => x.Username, user);
        }

        [Test]
        public void Validate_UsernameEmptyAndFoundFalse_DoesNotHaveError()
        {
            var user = new User { Username = string.Empty, Found = false };
            var validator = new UserValidator();

            validator.ShouldNotHaveValidationErrorFor(x => x.Username, user);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Validate_UsernameNotEmpty_DoesNotHaveError(bool found)
        {
            var fixture = new Fixture();
            var user = new User { Username = fixture.Create<string>(), Found = found };
            var validator = new UserValidator();

            validator.ShouldNotHaveValidationErrorFor(x => x.Username, user);
        }
    }
}