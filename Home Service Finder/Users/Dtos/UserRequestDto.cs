namespace Home_Service_Finder.Users.Dtos
{
    public class UserRequestDto
{

        public string FirstName { get; set; }
        //public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public string Username { get; set; }
        public Guid RoleId {get; set;}
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? ProfilePicture { get; set; }




    }
}
