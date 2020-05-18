using System.IO;

namespace Core.Dtos
{
    public class CreatePostDto
    {
        public Stream File { get; set; }
        public string ContentType { get; set; }
        public string Caption { get; set; }
    }
}
