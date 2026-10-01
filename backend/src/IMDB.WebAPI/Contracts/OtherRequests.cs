using System.ComponentModel.DataAnnotations;
using IMDB.Application.Features.People;
using IMDB.Domain.Enums;

namespace IMDB.WebAPI.Contracts;

public class GenreRequest
{
    [Required(ErrorMessage = "Genre title is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Genre title must be between 2 and 50 characters")]
    public string Title { get; set; } = string.Empty;
}

public class PersonRequest
{
    [Required(ErrorMessage = "IMDB ID is required")]
    public string ImdbId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date, ErrorMessage = "Birth date is not valid")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Biography is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Biography must be between 10 and 2000 characters")]
    public string Bio { get; set; } = string.Empty;

    [Required(ErrorMessage = "Photo URL is required")]
    [Url(ErrorMessage = "Photo URL should be valid")]
    public string PhotoUrl { get; set; } = string.Empty;

    public PersonData ToData() => new(ImdbId, FullName, BirthDate, Bio, PhotoUrl);
}

public class CastRequest
{
    [Required(ErrorMessage = "Media ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Media ID")]
    public int MediaId { get; set; }

    [Required(ErrorMessage = "Person ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Person ID")]
    public int PersonId { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public CastRole Role { get; set; }
}

public class CommentRequest
{
    [Required(ErrorMessage = "Comment text is required")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 1000 characters")]
    public string Text { get; set; } = string.Empty;
}

public class RateRequest
{
    [Required(ErrorMessage = "Score is required")]
    [Range(0, 5, ErrorMessage = "Score must be between 0 and 5")]
    public ScoreEnum Score { get; set; }
}

public class RegisterRequest
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers and underscores")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters")]
    public string EmailAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
