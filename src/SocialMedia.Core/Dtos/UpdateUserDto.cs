namespace SocialMedia.Core.Dtos
{
    public class UpdateUserDto
    {
        public string Username { get; set; }
        public string Bio { get; set; }
        public UpdateSocialsDto Socials { get; set; }
    }
}
