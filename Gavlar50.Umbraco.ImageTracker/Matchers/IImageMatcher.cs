using System.Collections.Generic;
using Umbraco.Core.Services;

namespace Gavlar50.Umbraco.ImageTracker.Matchers
{
    public interface IImageMatcher
    {
        List<string> UmbracoPropertyEditors { get; }
        List<int> Matches(string source, IMediaService ms);
        string Source { get; set; }
    }
}
