using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using FluentValidation.Validators;
using NUnit.Framework;

namespace Question60871574
{
    public class Foo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public interface IFooRepository
    {
        Foo GetById(Guid id);

        Task<Foo> GetByIdAsync(Guid id, CancellationToken cancellation = default);
    }

    public class FooRepository : IFooRepository
    {
        private readonly List<Foo> _items = new List<Foo>();

        public FooRepository(IEnumerable<Foo> items)
        {
            _items.AddRange(items);
        }

        public Foo GetById(Guid id)
        {
            Console.WriteLine("FooRepository.GetById invoked");
            return _items.Single(x => x.Id.Equals(id));
        }

        public async Task<Foo> GetByIdAsync(Guid id, CancellationToken cancellation = default)
        {
            Console.WriteLine("FooRepository.GetByIdAsync invoked");
            var foo = await Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    return GetById(id);
                },
                cancellation);
            return foo;
        }
    }

    public class UpdateFooDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class FooExistsValidator : AsyncValidatorBase
    {
        private readonly IFooRepository _fooRepository;

        public FooExistsValidator(IFooRepository fooRepository) : base("Foo does not exist")
        {
            _fooRepository = fooRepository;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            Console.WriteLine("FooExistsValidator.IsValid start");
            var idToValidate = (Guid) context.PropertyValue;
            try
            {
                var foo = _fooRepository.GetById(idToValidate);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                Console.WriteLine("FooExistsValidator.IsValid end");
            }
        }

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            Console.WriteLine("FooExistsValidator.IsValidAsync start");
            var idToValidate = (Guid) context.PropertyValue;
            try
            {
                //Simulate multiple threads
#pragma warning disable 4014
                Task.Run(() =>
#pragma warning restore 4014
                    {
                        Console.WriteLine("Time waster start");
                        Thread.Sleep(2500);
                        Console.WriteLine("Time waster end");
                    },
                    cancellation);

                var foo = await _fooRepository.GetByIdAsync(idToValidate, cancellation);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                Console.WriteLine("FooExistsValidator.IsValidAsync end");
            }
        }
    }

    public class UpdateFooValidator : AbstractValidator<UpdateFooDto>
    {
        public UpdateFooValidator(IFooRepository fooRepository)
        {
            RuleFor(x => x.Id).NotNull().SetValidator(new FooExistsValidator(fooRepository));
        }
    }

    public class Tests
    {
        [Test]
        public async Task ValidateAsync_ValidDto_NoErrors()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<Foo>();
            var itemToUpdate = items.First();
            var updateDto = new UpdateFooDto { Id = itemToUpdate.Id, Name = fixture.Create<string>() };

            var fooRepository = new FooRepository(items);
            var validator = new UpdateFooValidator(fooRepository);

            var validationResult = await validator.ValidateAsync(updateDto);

            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void Validate_ValidDto_NoErrors()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<Foo>();
            var itemToUpdate = items.First();
            var updateDto = new UpdateFooDto { Id = itemToUpdate.Id, Name = fixture.Create<string>() };

            var fooRepository = new FooRepository(items);
            var validator = new UpdateFooValidator(fooRepository);

            var validationResult = validator.Validate(updateDto);

            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ValidateAsync_InvalidDto_ReturnsErrors()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<Foo>();
            var updateDto = new UpdateFooDto { Id = Guid.NewGuid(), Name = fixture.Create<string>() };

            var fooRepository = new FooRepository(items);
            var validator = new UpdateFooValidator(fooRepository);

            var validationResult = await validator.ValidateAsync(updateDto);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_InvalidDto_ReturnsErrors()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<Foo>();
            var updateDto = new UpdateFooDto { Id = Guid.NewGuid(), Name = fixture.Create<string>() };

            var fooRepository = new FooRepository(items);
            var validator = new UpdateFooValidator(fooRepository);

            var validationResult = validator.Validate(updateDto);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(1));
        }
    }
}