namespace AshborneGame._Core.Globals.Services
{
    public static class SlugIdService
    {
        public static string GenerateSlugId(string name, string prefix)
        {
            if (string.IsNullOrWhiteSpace(name))
                return $"{prefix}.unknown-" + Guid.NewGuid().ToString("N")[..8];

            // Normalize: lowercase, replace spaces with hyphens, remove non-alphanumeric except hyphens
            var slug = name.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("'", "");
            
            // Remove any characters that aren't letters, numbers, or hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
            
            // Remove consecutive hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-").Trim('-');

            return $"{prefix}.{slug}";
        }
    }
}