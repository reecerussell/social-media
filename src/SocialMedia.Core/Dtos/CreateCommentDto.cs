namespace SocialMedia.Core.Dtos
{
    public class CreateCommentDto
    {
        public string PostId { get; set; }
        public string ReplyToId { get; set; }
        public string Comment { get; set; }
    }
}
