namespace Home_Service_Finder.Email.Dtos
{
    public class EmailOTPResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
    }
}
