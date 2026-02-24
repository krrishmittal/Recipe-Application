using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Server.Services.Interfaces;

namespace Server.Services;

public class ImageService : IImageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<ImageService> _logger;

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

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        _logger.LogInformation("Uploading image: {FileName}", file.FileName);

        if (file.Length == 0)
            throw new ArgumentException("File is empty.");

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Only JPEG, PNG and WEBP images are allowed.");

        if (file.Length > 5 * 1024 * 1024)
            throw new ArgumentException("File size must be less than 5MB.");

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


























    //public async Task DeleteImageAsync(string imageUrl)
    //{
    //    try
    //    {
    //        var uri = new Uri(imageUrl);
    //        var segments = uri.Segments;
    //        var publicId = string.Concat(
    //            segments.SkipWhile(s => !s.Contains("recipe-app")).ToArray()
    //        ).Trim('/');
    //        publicId = Path.ChangeExtension(publicId, null);

    //        var deleteParams = new DeletionParams(publicId);
    //        await _cloudinary.DestroyAsync(deleteParams);

    //        _logger.LogInformation("Image deleted from Cloudinary: {PublicId}", publicId);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Failed to delete image: {Url}", imageUrl);
    //    }
    //}
}