using System;
using System.Collections.Generic;

namespace Recipe.Domain.Models;

/// <summary>
/// Represents a one-time password issued to a user.
/// </summary>
public partial class OtpRecord
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Gets or sets the OTP code.
    /// </summary>
    public string OtpCode { get; set; } = null!;
    /// <summary>
    /// Gets or sets the expiration date and time.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    /// <summary>
    /// Gets or sets whether the OTP has been used.
    /// </summary>
    public bool IsUsed { get; set; }
    /// <summary>
    /// Gets or sets the creation date and time.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Gets or sets the user that owns the OTP record.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
