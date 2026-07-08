namespace AshborneGame._Core.SaveSystem.Data.BOCSDTOs
{
    public sealed class ObjectNameSaveData
    {
        public string ReferenceName { get; set; }
        public string FirstTimeDisplayName { get; set; }
        public bool Seen { get; set; }
        public string? Article { get; set; }
        public List<string> Synonyms { get; set; }
    }
}