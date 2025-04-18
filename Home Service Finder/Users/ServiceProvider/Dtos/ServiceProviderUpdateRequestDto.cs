namespace Home_Service_Finder.Users.ServiceProvider.Dtos
{
    public class ServiceProviderUpdateRequestDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        
        public string? ProfilePicture { get; set; }

        //public string Licenses { get; set; }
        public int Experience { get; set; }
        public string? PersonalDescription { get; set; }

    }
}
