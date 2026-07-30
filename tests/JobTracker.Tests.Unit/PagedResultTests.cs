using JobTracker.Application.Common;

namespace JobTracker.Tests.Unit;

public sealed class PagedResultTests
{
    [Theory]
    [InlineData(0, 1, 10, 0, false, false)]
    [InlineData(1, 1, 10, 1, false, false)]
    [InlineData(25, 1, 10, 3, true, false)]
    [InlineData(25, 2, 10, 3, true, true)]
    [InlineData(25, 3, 10, 3, false, true)]
    public void PagingMetadata_ReflectsCurrentPage(
        int totalItems,
        int page,
        int pageSize,
        int totalPages,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        var result = new PagedResult<int>(
            [],
            page,
            pageSize,
            totalItems);

        Assert.Equal(totalPages, result.TotalPages);
        Assert.Equal(hasNextPage, result.HasNextPage);
        Assert.Equal(hasPreviousPage, result.HasPreviousPage);
    }
}
