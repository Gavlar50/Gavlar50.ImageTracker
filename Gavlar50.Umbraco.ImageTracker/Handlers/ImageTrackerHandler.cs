using System.Linq;
using Gavlar50.Umbraco.ImageTracker.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;

namespace Gavlar50.Umbraco.ImageTracker.Handlers
{
    public class ImageTrackerHandler : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication,
           ApplicationContext applicationContext)
        {
            var ctx = applicationContext.DatabaseContext;
            var db = new DatabaseSchemaHelper(ctx.Database, applicationContext.ProfilingLogger.Logger, ctx.SqlSyntax);

            if (!db.TableExist("gavlar50ImageTracker"))
            {
                db.CreateTable<ImageTrackerDbModel>(false);
            }
            if (!db.TableExist("gavlar50ImageTrackerProgress"))
            {
                db.CreateTable<ImageTrackerProgress>(false);
            }
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Published += ContentServiceOnPublished;
            ContentService.Deleted += ContentServiceOnDeleted;
            LogHelper.Info<ImageTrackerHandler>("ImageTracker initialised and tracking images");
        }

        /// <summary>
        /// Called when user empties the recycle bin, or an item in the recycle bin is permanently deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deleteEventArgs"></param>
        private void ContentServiceOnDeleted(IContentService sender, DeleteEventArgs<IContent> deleteEventArgs)
        {
            if (!deleteEventArgs.DeletedEntities.Any()) return;
            foreach (var entity in deleteEventArgs.DeletedEntities)
            {
                PropertyMatchHandler.Delete(entity.Id);
            }
        }

        /// <summary>
        /// Called when an item is published
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="publishEventArgs"></param>
        private void ContentServiceOnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> publishEventArgs)
        {
            if (!publishEventArgs.PublishedEntities.Any()) return;
            foreach (var entity in publishEventArgs.PublishedEntities)
            {
                // remove any existing stale entries for this page
                PropertyMatchHandler.Delete(entity.Id);

                // match and store image usage for current page 
                PropertyMatchHandler.MatchProperties(entity);
            }
        }
    }
}