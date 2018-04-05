namespace EmbyStat.Controllers.Tasks
{
    public class TaskStateViewModel
    {
        public string Id { get; set; }
        public int State { get; set; }
        public double? CurrentProgress { get; set; }
    }
}
