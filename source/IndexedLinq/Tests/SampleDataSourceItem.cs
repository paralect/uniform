namespace IndexedLinq.Tests
{
    // The item type that our data source will return.
    public class SampleDataSourceItem
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public void SetupIndexes()
        {
            // return Setup.Index(p => Name)
            // return Setup.Index(p => Name, p => Description)
            // return Setup.Index(p => Name, p => p.Inner.Subinner.Property)
        }
    }
}