<Query Kind="Program">
  <NuGetReference>Moq</NuGetReference>
  <NuGetReference>NHibernate</NuGetReference>
  <Namespace>Moq</Namespace>
  <Namespace>NHibernate</Namespace>
  <Namespace>NHibernate.Linq</Namespace>
  <Namespace>NHibernate.Type</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var requestRolesList = new List<RequestRole>();
	requestRolesList.Add(new RequestRole { Id = 0, RequestOperator = new RequestOperator { Id = 1 } });
	requestRolesList.Add(new RequestRole { Id = 1, RequestOperator = new RequestOperator { Id = 2 } });

	var requestRolesAsQueryable = requestRolesList.AsQueryable();

	var queryProviderMock = new Mock<INhQueryProvider>();
	queryProviderMock.Setup(x => x.CreateQuery<RequestRole>(It.IsAny<Expression>()))
		.Returns((Expression providedExpression) =>
		{
			return new TestingQueryable<RequestRole>(requestRolesAsQueryable.Provider.CreateQuery<RequestRole>(providedExpression));
		});

	var queryableMock = new Mock<IQueryable<RequestRole>>();
	queryableMock.Setup(x => x.Provider).Returns(queryProviderMock.Object);
	queryableMock.Setup(x => x.Expression).Returns(requestRolesAsQueryable.Expression);
	queryableMock.Setup(x => x.GetEnumerator()).Returns(requestRolesAsQueryable.GetEnumerator());
	queryableMock.Setup(x => x.ElementType).Returns(requestRolesAsQueryable.ElementType);

	var sessionMock = new Mock<ISession>();
	sessionMock.Setup(s => s.Query<RequestRole>()).Returns(queryableMock.Object);
	var query = sessionMock.Object.Query<RequestRole>();

	var task = Task.Run(() =>
	{
		return query.Where(x => x.Id != 0).ToListAsync();
	});

	Task.WaitAll(task);

	Console.WriteLine(task.Result);
}

// Define other methods, classes and namespaces here
public class RequestRole
{
	public int Id { get; set; }
	public RequestOperator RequestOperator { get; set; }
}

public class RequestOperator
{
	public int Id { get; set; }
}

public class TestingQueryable<T> : IQueryable<T>
{
	private readonly IQueryable<T> _queryable;

	public TestingQueryable(IQueryable<T> queryable)
	{
		_queryable = queryable;
		Provider = new TestingQueryProvider<T>(_queryable);
	}

	public Type ElementType => _queryable.ElementType;

	public Expression Expression => _queryable.Expression;

	public IQueryProvider Provider { get; }

	public IEnumerator<T> GetEnumerator()
	{
		return _queryable.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _queryable.GetEnumerator();
	}
}

public class TestingQueryProvider<T> : INhQueryProvider
{
	public TestingQueryProvider(IQueryable<T> source)
	{
		Source = source;
	}

	public IQueryable<T> Source { get; set; }

	public IQueryable CreateQuery(Expression expression)
	{
		throw new NotImplementedException();
	}

	public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
	{
		return new TestingQueryable<TElement>(Source.Provider.CreateQuery<TElement>(expression));
	}

	public object Execute(Expression expression)
	{
		throw new NotImplementedException();
	}

	public TResult Execute<TResult>(Expression expression)
	{
		return Source.Provider.Execute<TResult>(expression);
	}

	public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
	{
		return Task.FromResult(Execute<TResult>(expression));
	}

	public int ExecuteDml<T1>(QueryMode queryMode, Expression expression)
	{
		throw new NotImplementedException();
	}

	public Task<int> ExecuteDmlAsync<T1>(QueryMode queryMode, Expression expression, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression)
	{
		throw new NotImplementedException();
	}

	public IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression)
	{
		throw new NotImplementedException();
	}

	public void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters)
	{
		throw new NotImplementedException();
	}
}