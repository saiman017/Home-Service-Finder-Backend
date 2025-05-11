namespace Home_Service_Finder.Users.DTO
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        //public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        //public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Role { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool IsEmailVerified { get; set; }



    }
}
