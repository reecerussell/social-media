using System.Globalization;

namespace Core.Abstractions
{
    public class Normalizer : INormalizer
    {
        public string Normalize(string text)
        {
            return text.ToUpper(CultureInfo.InvariantCulture);
        }
    }
}
