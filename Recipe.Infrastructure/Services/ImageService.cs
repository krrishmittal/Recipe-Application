using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Recipe.Application.Services.Interfaces;

namespace Recipe.Infrastructure.Services;

/// <summary>
/// Uploads and deletes images in Cloudinary.
/// </summary>
public class ImageService : IImageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<ImageService> _logger;

    /// <summary>
    /// Initializes a new instance of the ImageService class.
    /// </summary>
    public ImageService(IConfiguration config, ILogger<ImageService> logger)
    {
        _logger = logger;
        var settings = config.GetSection("CloudinarySettings");
        var account = new Account(
            settings["CloudName"],
            settings["ApiKey"],
            settings["ApiSecret"]);
        _cloudinary = new Cloudinary(account);
    }

    /// <summary>
    /// Uploads an image file and returns its hosted URL.
    /// </summary>
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        _logger.LogInformation("Uploading image: {FileName}", file.FileName);

        if (file.Length == 0)
        {
            throw new ArgumentException("File is empty.");
        }

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
        {
            throw new ArgumentException("Only JPEG, PNG and WEBP images are allowed.");
        }

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "recipe-app",
            Transformation = new Transformation().Width(800).Height(600).Crop("fill")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
        {
            _logger.LogError("Cloudinary upload failed: {Error}", result.Error.Message);
            throw new Exception($"Image upload failed: {result.Error.Message}");
        }

        _logger.LogInformation("Image uploaded: {Url}", result.SecureUrl);
        return result.SecureUrl.ToString();
    }

    /// <summary>
    /// Deletes an image by its hosted URL when possible.
    /// </summary>
    public async Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        try
        {
            var uri = new Uri(imageUrl);
            var publicId = string.Concat(
                uri.Segments.SkipWhile(s => !s.Contains("recipe-app", StringComparison.OrdinalIgnoreCase)).ToArray()
            ).Trim('/');

            if (string.IsNullOrWhiteSpace(publicId))
            {
                return;
            }

            publicId = Path.ChangeExtension(publicId, null);
            await _cloudinary.DestroyAsync(new DeletionParams(publicId));

            _logger.LogInformation("Image deleted from Cloudinary: {PublicId}", publicId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image: {Url}", imageUrl);
        }
    }
}
