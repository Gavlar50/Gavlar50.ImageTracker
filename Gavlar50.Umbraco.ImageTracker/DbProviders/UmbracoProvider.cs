using System;
using System.Linq;
using Gavlar50.Umbraco.ImageTracker.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Web;

namespace Gavlar50.Umbraco.ImageTracker.DbProviders
{
    public class UmbracoProvider : IImageTrackerDbProvider
    {
        public string DbConnString { get; set; }

        readonly Database _context = UmbracoContext.Current.Application.DatabaseContext.Database;

        public void AddImageTrack(ImageTrackerDbModel track)
        {
            try
            {
                var query =
                   _context.Query<ImageTrackerModel>(
                      string.Format("select * from gavlar50ImageTracker where ImageId={0} and PageId={1} and PropertyId={2}",
                         track.ImageId, track.PageId, track.PropertyId));
                if (query.Any()) return;
                _context.Insert("gavlar50ImageTracker", "Id", track);
            }
            catch (Exception ex)
            {
                LogHelper.Error<IImageTrackerDbProvider>(ex.Message, ex);
            }
        }

        public void RemoveImageTrack(int pageId)
        {
            try
            {
                _context.Delete(string.Format(
                   "delete from gavlar50ImageTracker where PageId={0}",
                   pageId));
            }
            catch (Exception ex)
            {
                LogHelper.Error<IImageTrackerDbProvider>(ex.Message, ex);
            }
        }

        public void UpdateProgress(int pageId)
        {
            try
            {
                //_context.Insert("gavlar50ImageTrackerProgress", "PageId", new ImageTrackerProgress { PageId = pageId });
                _context.Insert(new ImageTrackerProgress { PageId = pageId });
            }
            catch (Exception ex)
            {
                LogHelper.Error<IImageTrackerDbProvider>(ex.Message, ex);
            }
        }
    }
}
