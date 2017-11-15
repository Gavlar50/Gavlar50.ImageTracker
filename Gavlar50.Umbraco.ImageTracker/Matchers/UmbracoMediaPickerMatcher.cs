using System.Collections.Generic;
using Umbraco.Core.Services;

namespace Gavlar50.Umbraco.ImageTracker.Matchers
{
    /// <summary>
    /// Matches umbraco content which reference media as one or more media ids  
    /// </summary>
    public class UmbracoMediaPickerMatcher : IImageMatcher
    {
        private List<string> _umbracoPropertyEditors = new List<string> {
            "Umbraco.MediaPicker",
            "Umbraco.MultiNodeTreePicker",
            "Umbraco.MultipleMediaPicker"
        };

        public string Source { get; set; }
        public List<string> UmbracoPropertyEditors
        {
            get { return _umbracoPropertyEditors; }
        }

        public List<int> Matches(string source, IMediaService ms)
        {
            var ids = new List<int>();
            var sources = source.Split(new[] { ',' });
            foreach (var strId in sources)
            {
                var id = int.Parse(strId);
                ids.Add(id);
            }
            return ids;
        }

    }
}