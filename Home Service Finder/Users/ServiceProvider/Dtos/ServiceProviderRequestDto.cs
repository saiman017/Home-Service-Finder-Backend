namespace Home_Service_Finder.Users.ServiceProvider.Dtos
{
    public class ServiceProviderRequestDto
    {


            public string Email { get; set; }
            //public string Username { get; set; }
            public string PhoneNumber { get; set; }
            public string FirstName { get; set; }
            //public string? MiddleName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public DateOnly DateOfBirth { get; set; }
            public Guid RoleId { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }

            public string? ProfilePicture { get; set; }


            //public string Licenses { get; set; }
            public int Experience { get; set; }

            public string? PersonalDescription { get; set; }


            public Guid ServiceCategoryId { get; set; }









    }
}
