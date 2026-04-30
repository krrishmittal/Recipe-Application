namespace Recipe.Application.DTOs.Request;

/// <summary>
/// Represents pagination and search parameters for list endpoints.
/// </summary>
public class PagedRequest
{
    private int _pageSize = 10;
    private const int MaxPageSize = 50;

    /// <summary>
    /// Gets or sets the requested page number.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of records per page.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
    }

    /// <summary>
    /// Gets or sets the search.
    /// </summary>
    public string? Search { get; set; }
    /// <summary>
    /// Gets or sets the sort by.
    /// </summary>
    public string? SortBy { get; set; }
    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    public string? SortOrder { get; set; } = "desc";

    /// <summary>
    /// Gets or sets the category filter.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the tag filter.
    /// </summary>
    public string? Tag { get; set; }
}
