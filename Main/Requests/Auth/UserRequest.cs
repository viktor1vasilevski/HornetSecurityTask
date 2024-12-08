using System.ComponentModel.DataAnnotations;

namespace Main.Requests.Auth;

public class UserRequest
{
    [Required, MinLength(3)]
    public required string FirstName { get; set; }

    [Required, MinLength(3)]
    public required string LastName { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+(\.[a-zA-Z]{2,})+$",
        ErrorMessage = "Please enter a valid email address in the format 'example@domain.com'.")]
    public required string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{4,}$",
        ErrorMessage = "Password must be at least 4 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public required string Password { get; set; }
}
