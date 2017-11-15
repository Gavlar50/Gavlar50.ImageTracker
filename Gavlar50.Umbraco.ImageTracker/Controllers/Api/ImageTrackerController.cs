using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Umbraco.Web.WebApi;
using Newtonsoft.Json;
using Gavlar50.Umbraco.ImageTracker.Models;
using Gavlar50.Umbraco.ImageTracker.Handlers;
using Umbraco.Core.Persistence;
using System.Text.RegularExpressions;
using Umbraco.Web;

namespace Gavlar50.Umbraco.ImageTracker.Controllers.Api
{
    public class ImageTrackerController : UmbracoApiController
    {
        /// <summary>
        /// Reports all usages of selected image
        /// </summary>
        /// <param name="id">The image id</param>
        /// <returns>Json list of image usages</returns>
        [System.Web.Http.HttpGet]
        public string Usages(int id)
        {
            var ctx = ApplicationContext.DatabaseContext.Database;
            var cache = UmbracoContext.ContentCache;

            var model = new List<ImageTrackerModel>();
            var query = ctx.Query<ImageTrackerModel>(string.Format(
                "select t.name as Property, mt.PageId, " +
                "n.text as Page from gavlar50ImageTracker mt " +
                "inner join umbracoNode n on mt.PageId = n.id " +
                "inner join cmsPropertyType t on mt.PropertyId=t.id " +
                "inner join cmsdocument d on mt.PageId = d.nodeid and d.newest=1" +
                "where mt.ImageId={0} order by mt.PageId", id)).ToList();
            foreach (var item in query)
            {
                var page = cache.GetById(item.PageId);
                item.PageUrl = page.Url;
                model.Add(item);
            }

            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// Reports all tracked images across the site
        /// </summary>
        /// <returns>Json list of image usages</returns>
        [System.Web.Http.HttpGet]
        public string Everything()
        {
            var ms = ApplicationContext.Services.MediaService;
            var cs = ApplicationContext.Services.ContentService;
            var ctx = ApplicationContext.DatabaseContext.Database;
            var cache = UmbracoContext.ContentCache;

            var model = new List<ImageTrackerModel>();
            var query = ctx.Query<ImageTrackerModel>(
               "select t.name as Property, mt.PageId, n.text as ContentName, mt.ImageId from gavlar50ImageTracker mt " +
               "inner join umbracoNode n on mt.PageId = n.id " +
               "inner join cmsPropertyType t on mt.PropertyId=t.id " +
               "inner join cmsPropertyData p on mt.ImageId=p.contentNodeId " +
               "inner join cmsPropertyType y on p.propertytypeid=y.id and y.Alias='umbracoFile' " +
               "inner join cmsdocument d on mt.PageId = d.nodeid and d.newest=1" +
               "order by mt.PageId").ToList();
            foreach (var item in query)
            {
                var page = cache.GetById(item.PageId);
                var image = ms.GetById(item.ImageId);
                var umbracoFile = image.GetValue<string>("umbracoFile");
                if (!string.IsNullOrWhiteSpace(umbracoFile))
                {
                    var match = Regex.Match(umbracoFile, "(?<=\"src\": \")([^\"])*");
                    // new content appears to store as src:, older content appears to be the actual path value
                    item.ImageUrl = string.IsNullOrEmpty(match.Value) ? umbracoFile : match.Value; 
                }
                item.Page = page.Name;
                item.PageUrl = page.Url;
                item.Image = image.Name;
                item.Size = image.GetValue<long>("umbracoBytes");
                item.Location = ImageLocationFromPath(image.Path, item.ImageId.ToString());
                model.Add(item);
            }

            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// Reports all tracked images that aren't used 
        /// </summary>
        /// <returns>Json list of image usages</returns>
        [System.Web.Http.HttpGet]
        public string Unused()
        {
            var ms = ApplicationContext.Services.MediaService;
            var cs = ApplicationContext.Services.ContentService;
            var ctx = ApplicationContext.DatabaseContext.Database;

            var model = new List<ImageTrackerModel>();
           
            var query = ctx.Query<ImageTrackerModel>(
                "select d.contentNodeId as ImageId,n.text as Image from cmspropertydata d " +
               "inner join cmsPropertyType t on d.propertytypeid = t.id " +
               "inner join umbracoNode n on d.contentNodeId = n.id " +
               "where t.Alias = 'umbracoFile' and n.trashed = 0 and d.contentNodeId not in " +
               "(select ImageId from gavlar50ImageTracker)").ToList();

            foreach (var item in query)
            {
                var image = ms.GetById(item.ImageId);
                item.ImageId = image.Id;
                var umbracoFile = image.GetValue<string>("umbracoFile");
                if (!string.IsNullOrWhiteSpace(umbracoFile))
                {
                    var match = Regex.Match(umbracoFile, "(?<=\"src\": \")([^\"])*");
                    // new content appears to store as src:, older content appears to be the actual path value
                    item.ImageUrl = string.IsNullOrEmpty(match.Value) ? umbracoFile : match.Value;
                }
                item.Image = image.Name;
                item.Size = image.GetValue<long>("umbracoBytes");
                item.Location = ImageLocationFromPath(image.Path, item.ImageId.ToString());
                model.Add(item);
            }
 
            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// Populates tracker table for all existing content.
        /// Call using the Url {siteroot}/umbraco/api/imagetracker/init
        /// This url is explicitly excluded from the UI to force you to run it manually for obvious reasons
        /// </summary>
        /// <returns>Response message</returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Init()
        {
            var cs = Services.ContentService;
            var db = ApplicationContext.DatabaseContext.Database;

            // if any records exist in the progress table then the last run timed out so skip the tracker deletion
            // and continue after the last page processed
            var exists = new Sql("select PageId from gavlar50ImageTrackerProgress", null);
            var inProgress = db.FirstOrDefault<int>(exists);
            if (inProgress == 0)
            {
                // theres no existing init in progress so clear any existing table data, its about to be rebuilt
                Sql delete = new Sql("delete from gavlar50ImageTracker", null);
                db.Execute(delete);
            }

            // ensure there are no o/s page edits for those you want to include. Only published/newest pages
            // will be selected for initial tracking
            var query = "select nodeId from cmsDocument d left outer join gavlar50ImageTrackerProgress t on d.nodeId=t.PageId where newest=1 and published=1 and t.PageId is null";
            var nodeIds = db.Query<int>(query).ToList();
            foreach (var nodeId in nodeIds)
            {
                var entity = cs.GetById(nodeId);
                PropertyMatchHandler.MatchProperties(entity);
                PropertyMatchHandler.UpdateProgress(entity.Id);
            }

            // if we got this far then the init is complete so we can bin the progress data
            var reset = new Sql("delete from gavlar50ImageTrackerProgress", null);
            db.Execute(reset);

            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Success! image tracker init completed successfully" });
        }

        /// <summary>
        /// Unpacks the comma separated path location into a meaningful logical Umbraco media folder structure
        /// </summary>
        /// <param name="path">The source path csv</param>
        /// <param name="imageId">The image id</param>
        /// <returns>Umbraco media path to image in the media tree</returns>
        private string ImageLocationFromPath(string path, string imageId)
        {
            var location = "Media";
            var ms = ApplicationContext.Services.MediaService;
            var tree = path.Split(new[] { ',' }).ToList();
            foreach (var folderStr in tree)
            {
                if (folderStr == "-1" || folderStr == imageId) continue;
                var media = ms.GetById(int.Parse(folderStr));
                location += "/" + media.Name;
            }
            return location;
        }
    }
}