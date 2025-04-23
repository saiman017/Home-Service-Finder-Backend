namespace Home_Service_Finder.Users.ServiceProvider.Dtos
{
    public class ServiceProviderResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Role { get; set; }
        //public string Password { get; set; }
        public string? ProfilePicture { get; set; }
        //public string Licenses { get; set; } // later if needed change it to array to store more document for future
        public int Experience { get; set; }
        public string? PersonalDescription { get; set; }

        public bool IsAdminVerified { get; set; }
        public bool IsEmailVerified { get; set; }

        public Guid ServiceCategoryId { get; set; }


        public string ServiceCategory { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }



        //public bool IsDeleted { get; set; }
    }
}
