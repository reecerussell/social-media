namespace Core.Models
{
    public class MimeType
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string NormalizedName { get; protected set; }
    }
}
