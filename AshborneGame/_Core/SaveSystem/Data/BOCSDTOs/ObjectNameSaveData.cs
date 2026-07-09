namespace AshborneGame._Core.SaveSystem.Data.BOCSDTOs
{
    public sealed class ObjectNameSaveData
    {
        public string ReferenceName { get; set; } = null!;
        public string FirstTimeDisplayName { get; set; } = null!;
        public bool Seen { get; set; }
        public string? Article { get; set; }
        public List<string> Synonyms { get; set; } = new();

        public ObjectNameSaveData(string referenceName, string firstTimeDisplayName, bool seen, string? article, List<string> synonyms)
        {
            ReferenceName = referenceName;
            FirstTimeDisplayName = firstTimeDisplayName;
            Seen = seen;
            Article = article;
            Synonyms = synonyms;
        }
    }
}