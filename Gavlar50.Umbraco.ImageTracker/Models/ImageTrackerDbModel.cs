
using Umbraco.Core.Persistence;

namespace Gavlar50.Umbraco.ImageTracker.Models
{
    // The model used to create records in the gavlar50ImageTracker table
    [TableName("gavlar50ImageTracker")]
    public class ImageTrackerDbModel
    {
        public int ImageId { get; set; }
        public int PageId { get; set; }
        public int PropertyId { get; set; }
    }
}