namespace Hyphen.Sdk;

public class PagedParamsTests
{
	public class PageNumber
	{
		[Fact]
		public void GuardClause()
		{
			var ex = Record.Exception(() => new PagedParams { PageNumber = 0 });

			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith("Page number must be a positive integer", argEx.Message);
			Assert.Equal("PageNumber", argEx.ParamName);
		}

		[Theory]
		[InlineData(null)]
		[InlineData(42)]
		public void ValidValues(int? pageNumber)
		{
			var ex = Record.Exception(() => new PagedParams { PageNumber = pageNumber });

			Assert.Null(ex);
		}
	}

	public class PageSize
	{
		[Theory]
		[InlineData(4)]
		[InlineData(201)]
		public void GuardClause(int value)
		{
			var ex = Record.Exception(() => new PagedParams { PageSize = value });

			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith("Page size must be between 5 and 200", argEx.Message);
			Assert.Equal("PageSize", argEx.ParamName);
		}

		[Theory]
		[InlineData(null)]
		[InlineData(42)]
		public void ValidValues(int? pageSize)
		{
			var ex = Record.Exception(() => new PagedParams { PageSize = pageSize });

			Assert.Null(ex);
		}
	}
}
