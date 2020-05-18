using System.IO;

namespace Core.Dtos
{
    public class UpdateMediaDto
    {
        public string Id { get; set; }
        public Stream File { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }
    }
}
