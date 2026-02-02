namespace AshborneGame._Core.Globals.Services
{
    public static class SlugIdService
    {
        /// <summary>
        /// When true, appends a unique GUID suffix to all generated slug IDs.
        /// Set this to true in test setup to ensure unique IDs across test runs.
        /// </summary>
        public static bool IsTestMode { get; set; } = false;

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

            // In test mode, append a unique suffix to avoid duplicate ID errors
            if (IsTestMode)
            {
                var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
                return $"{prefix}.{slug}-{uniqueSuffix}";
            }

            return $"{prefix}.{slug}";
        }
    }
}