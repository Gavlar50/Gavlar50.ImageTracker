using Gavlar50.Umbraco.ImageTracker.Models;

namespace Gavlar50.Umbraco.ImageTracker.DbProviders
{
   public interface IImageTrackerDbProvider
   {
      string DbConnString { get; set; }
      void AddImageTrack(ImageTrackerDbModel track);
      void RemoveImageTrack(int pageId);
      void UpdateProgress(int pageId);
   }
}
