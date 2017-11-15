using System.Collections.Generic;
using System.Text.RegularExpressions;
using Umbraco.Core.Services;

namespace Gavlar50.Umbraco.ImageTracker.Matchers
{
    /// <summary>
    /// Matches umbraco content which reference media as /media/{media id}  
    /// </summary>
    public class UmbracoImageCropperMatcher : IImageMatcher
    {
        private const string _regex = "(?<=\"src\": \"/media/)[0-9]*";
        private List<string> _umbracoPropertyEditors = new List<string> {
            "Umbraco.ImageCropper"
        };
        public string Source { get; set; }
        public List<string> UmbracoPropertyEditors
        {
            get { return _umbracoPropertyEditors; }
        }

        public List<int> Matches(string source, IMediaService ms)
        {
            var ids = new List<int>();
            foreach (var match in Regex.Matches(source, _regex))
            {
                var id = int.Parse(match.ToString());
                ids.Add(id);
            }
            return ids;
        }

    }
}