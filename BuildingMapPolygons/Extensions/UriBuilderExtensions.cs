namespace WeatherServiceApp.Extensions
{
    internal static class UriBuilderExtensions
    {
        public static void AppendToPath(this UriBuilder uriBuilder, string relativeUri)
        {
            if (relativeUri == null) throw new ArgumentNullException(nameof(relativeUri));
            var newPath = Path.Combine(uriBuilder.Path, relativeUri);
            uriBuilder.Path = newPath;
        }
    }
}
