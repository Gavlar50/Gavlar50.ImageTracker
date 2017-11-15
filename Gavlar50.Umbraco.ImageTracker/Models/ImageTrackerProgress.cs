using Umbraco.Core.Persistence;

namespace Gavlar50.Umbraco.ImageTracker.Models
{
    /// <summary>
    /// This class stores the progress made by the init api method in case of db timeouts.
    /// Page ids are stored here as they are processed. If the init times out for large sites
    /// you can rerun it and it carries on from where it left off. The table is cleared on
    /// successful completion to allow reruns.
    /// </summary>
    [TableName("gavlar50ImageTrackerProgress")]
    public class ImageTrackerProgress
    {
        public int PageId { get; set; }
    }
}