using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Umbraco.Core.Services;

namespace Gavlar50.Umbraco.ImageTracker.Matchers
{
    /// <summary>
    /// Matches umbraco content which reference media as umb://media/{guid}
    /// </summary>
    public class UmbracoUmbMediaMatcher : IImageMatcher
    {
        private const string _regex = "(?<=umb://media/)[a-f0-9A-F]{32}"; // match on string beginning umb://media/ but don't include it in the match results, we just want the guid 
        private List<string> _umbracoPropertyEditors = new List<string> {
            "Umbraco.Grid",
            "Umbraco.MediaPicker2",
            "Umbraco.TinyMCEv3",
            "Umbraco.MultiNodeTreePicker2",
            "Umbraco.NestedContent"
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
                var guid = Guid.Parse(match.ToString());
                var media = ms.GetById(guid);
                if (media == null) continue;
                ids.Add(media.Id);
            }
            return ids;
        }
    }
}