using System.Globalization;

namespace SocialMedia.Core
{
    internal class Normalizer : INormalizer
    {
        public string Normalize(string text)
        {
            return text.ToUpper(CultureInfo.InvariantCulture);
        }
    }
}
