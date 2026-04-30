namespace Recipe.Application.Services.Interfaces;

/// <summary>
/// Defines image upload operations used by the application.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Uploads an image file and returns its hosted URL.
    /// </summary>
    Task<string> UploadImageAsync(IFormFile file);

    /// <summary>
    /// Deletes an image by its hosted URL when possible.
    /// </summary>
    Task DeleteImageAsync(string imageUrl);
}
