namespace EmbyStat.Api.EmbyClient.Model
{
    public class PersonsQuery : ItemsByNameQuery
    {
        public string[] PersonTypes { get; set; }
    }
}
