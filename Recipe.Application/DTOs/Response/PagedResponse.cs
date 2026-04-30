namespace Recipe.Application.DTOs.Response;

/// <summary>
/// Represents a paged result returned from list endpoints.
/// </summary>
public class PagedResponse<T>
{
    /// <summary>
    /// Gets or sets the paged items.
    /// </summary>
    public List<T> Items { get; set; } = [];
    /// <summary>
    /// Gets or sets the total number of records.
    /// </summary>
    public int TotalCount { get; set; }
    /// <summary>
    /// Gets or sets the requested page number.
    /// </summary>
    public int Page { get; set; }
    /// <summary>
    /// Gets or sets the number of records per page.
    /// </summary>
    public int PageSize { get; set; }
    /// <summary>
    /// Gets the total number of pages available for the result set.
    /// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    /// <summary>
    /// Gets whether another page is available after the current page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
    /// <summary>
    /// Gets whether a page is available before the current page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
