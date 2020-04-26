using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Question61426094
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestCommandHandlerConfiguration_RepositoryConfiguredWithCancellationTokenAndHandleInvokedWithCancellationToken_ReturnsExpectedResult()
        {
            var cts = new CancellationTokenSource();

            var myList = new List<MyTable> { new MyTable() };

            var myRepository = Substitute.For<IMyRepository>();
            myRepository.DoSomething(cts.Token).Returns(Task.FromResult(myList));

            var command = Substitute.For<MyCommand>();

            var sut = new MyCommandHandler(myRepository);

            var result = await sut.HandleThatReturnsListOfMyTable(command, cts.Token);

            result.Should().BeSameAs(myList);
        }

        [Fact]
        public async Task TestCommandHandlerConfiguration_RepositoryConfiguredWithCancellationTokenMatcherAndHandleInvokedWithCancellationToken_ReturnsExpectedResult()
        {
            var cts = new CancellationTokenSource();

            var myList = new List<MyTable> { new MyTable() };

            var myRepository = Substitute.For<IMyRepository>();
            myRepository.DoSomething(Arg.Any<CancellationToken>()).Returns(Task.FromResult(myList));

            var command = Substitute.For<MyCommand>();

            var sut = new MyCommandHandler(myRepository);

            var result = await sut.HandleThatReturnsListOfMyTable(command, cts.Token);

            result.Should().BeSameAs(myList);
        }

        [Fact]
        public async Task TestCommandHandlerConfiguration_WithDefaultCancellationToken_ReturnsExpectedResult()
        {
            var myList = new List<MyTable> { new MyTable() };

            var myRepository = Substitute.For<IMyRepository>();
            myRepository.DoSomething(Arg.Any<CancellationToken>()).Returns(Task.FromResult(myList));

            var command = Substitute.For<MyCommand>();

            var sut = new MyCommandHandler(myRepository);

            var result = await sut.HandleThatReturnsListOfMyTable(command);

            result.Should().BeSameAs(myList);
        }

        [Fact]
        public async Task TestRepositoryConfiguration()
        {
            var myList = new List<MyTable> { new MyTable() };

            var myRepository = Substitute.For<IMyRepository>();
            myRepository.DoSomething(Arg.Any<CancellationToken>()).Returns(Task.FromResult(myList));

            var result = await myRepository.DoSomething();

            result.Should().BeSameAs(myList);
        }
    }

    public class MyCommandHandler
    {
        private readonly IMyRepository _myRepository;

        public MyCommandHandler(IMyRepository myRepository)
        {
            _myRepository = myRepository ?? throw new ArgumentNullException(nameof(myRepository));
        }

        public async Task<List<MyTable>> HandleThatReturnsListOfMyTable(MyCommand command, CancellationToken cancellationToken = default)
        {
            return await _myRepository.DoSomething(cancellationToken);
        }
    }

    public class MyCommand { }

    public interface IMyRepository
    {
        Task<List<MyTable>> DoSomething(CancellationToken cancellationToken = default);
    }

    public class MyTable
    {
        public Guid Id { get; } = new Guid();
    }
}