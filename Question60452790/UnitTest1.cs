using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFixture;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.TestHelper;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Question60452790
{
    public class PatchDTO
    {
        public PatchDTO()
        {
            Data = new List<Datum>();
        }

        public List<Datum> Data { get; set; }

        public class Datum
        {
            public Datum()
            {
                Attributes = new Dictionary<string, object>();
            }

            public string Id { get; set; }
            public Dictionary<string, object> Attributes { get; set; }
        }
    }

    public class PatchDTOValidator : AbstractValidator<PatchDTO>
    {
        public PatchDTOValidator()
        {
            RuleFor(oo => oo.Data)
                .NotEmpty()
                .WithMessage("One or more Data blocks must be provided");

            RuleForEach(d => d.Data).ChildRules(datum =>
            {
                datum.RuleFor(d => d.Id)
                    .NotEmpty()
                    .WithMessage("Invalid 'Data.Id' value");
            });
        }
    }

    public class Tests
    {
        [TestCase(null)]
        [TestCase("")]
        public void ValidateIdUsingString_InvalidValue_HasError(string id)
        {
            var fixture = new Fixture();
            var datum = fixture.Build<PatchDTO.Datum>().With(x => x.Id, id).Create();
            var dto = fixture.Build<PatchDTO>().With(x => x.Data, new List<PatchDTO.Datum> { datum }).Create();

            var validator = new PatchDTOValidator();

            var validationResult = validator.TestValidate(dto);

            validationResult.ShouldHaveValidationErrorFor("Data[0].Id")
                .WithErrorMessage("Invalid 'Data.Id' value");

            Console.WriteLine(string.Join(Environment.NewLine, validationResult.Errors.Select(JsonConvert.SerializeObject)));
        }

        [TestCase(null)]
        [TestCase("")]
        public void ValidateIdUsingExpression_InvalidValue_HasError(string id)
        {
            var fixture = new Fixture();
            var datum = fixture.Build<PatchDTO.Datum>().With(x => x.Id, id).Create();
            var dto = fixture.Build<PatchDTO>().With(x => x.Data, new List<PatchDTO.Datum> { datum }).Create();

            var validator = new PatchDTOValidator();

            var validationResult = validator.TestValidate(dto);

            validationResult.ShouldHaveValidationErrorFor(x => x.Data[0].Id)
                .WithErrorMessage("Invalid 'Data.Id' value");
        }

        [Test]
        public void ResolvePropertyName_ChildProperty_ReturnsExpectedPropertyName()
        {
            Expression<Func<PatchDTO, string>> memberAccessor = patchDto => patchDto.Data[0].Id;
            var resolvedPropertyName = ValidatorOptions.PropertyNameResolver(typeof(PatchDTO), memberAccessor.GetMember(), memberAccessor);

            Assert.That(resolvedPropertyName, Is.EqualTo("Id"));
            //Assert.That(resolvedPropertyName, Is.EqualTo("Data[0].Id"));
        }
    }
}