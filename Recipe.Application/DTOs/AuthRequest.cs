namespace Recipe.Application.DTOs.Request;
/// <summary>
/// Represents the request payload used to register a new user.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Represents the request payload used to authenticate a user.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Represents the request payload used to start password recovery.
/// </summary>
public class ForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Represents the request payload used to reset a password with an OTP code.
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the OTP code.
    /// </summary>
    public string OtpCode { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Represents the request payload used to change the current user's password.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// Gets or sets the current password.
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Represents the request payload used to permanently delete an account.
/// </summary>
public class DeleteAccountRequest
{
    /// <summary>
    /// Gets or sets the current password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Represents the form-data payload used to update the current user's profile.
/// </summary>
public class UpdateProfileRequest
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the profile bio.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the uploaded profile image.
    /// </summary>
    public IFormFile? ProfileImage { get; set; }
}

/// <summary>
/// Represents the request payload used by admins to update a user's role.
/// </summary>
public class UpdateUserRoleRequest
{
    /// <summary>
    /// Gets or sets the target role.
    /// </summary>
    public string Role { get; set; } = string.Empty;
}
