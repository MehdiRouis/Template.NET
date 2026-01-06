namespace Template.Api.Models.Dtos.Users
{
    public class UserDto : BaseDto
    {

        public required string FullName { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
    }
}
