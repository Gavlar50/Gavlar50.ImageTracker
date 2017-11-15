
namespace Gavlar50.Umbraco.ImageTracker.Models
{
    // The model used for reporting
    public class ImageTrackerModel
    {
        /// <summary>
        /// The Umbraco page id
        /// </summary>
        public int PageId { get; set; }
        /// <summary>
        /// The name of the page
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// The Umbraco property that contains a reference to the image
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// The umbraco property id
        /// </summary>
        public int PropertyId { get; set; }
        /// <summary>
        /// The url of the page
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// The image id
        /// </summary>
        public int ImageId { get; set; }
        /// <summary>
        /// The url of the image
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// The name of the image
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// The size of the image in bytes
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// The location of the image in the media tree
        /// </summary>
        public string Location { get; set; }
    }
}