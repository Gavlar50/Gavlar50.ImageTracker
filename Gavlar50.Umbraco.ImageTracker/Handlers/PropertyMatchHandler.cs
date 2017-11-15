using Gavlar50.Umbraco.ImageTracker.DbProviders;
using Gavlar50.Umbraco.ImageTracker.Matchers;
using Gavlar50.Umbraco.ImageTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Gavlar50.Umbraco.ImageTracker.Handlers
{
    public static class PropertyMatchHandler
    {
        // implement additional IImageMatchers and register them here to handle other property editors
        public static List<IImageMatcher> ImageHandlers = new List<IImageMatcher>
        {
                new UmbracoUmbMediaMatcher(),
                new UmbracoImageCropperMatcher(),
                new UmbracoMediaPickerMatcher()
        };

        private static IImageTrackerDbProvider dbProvider = new UmbracoProvider();

        public static void MatchProperties(IContent entity)
        {
            var properties = entity.Properties.ToList();
            foreach (var prop in properties)
            {
                var propertyType = entity.PropertyTypes.Single(x => x.Alias == prop.Alias);
                var editorType = GetInstanceField(typeof(PropertyType), propertyType, "PropertyEditorAlias").ToString();
                if (prop.Value == null) continue;
                var propertyVal = prop.Value.ToString();
                if (string.IsNullOrWhiteSpace(propertyVal)) continue;

                // find a tracker handler for the current property or skip if its not tracked
                var handler = ImageHandlers.SingleOrDefault(x => x.UmbracoPropertyEditors.Contains(editorType));
                if (handler == null) continue;

                foreach (var imageId in handler.Matches(propertyVal, ApplicationContext.Current.Services.MediaService))
                {
                    var model = EnsureModel(entity.Id, imageId, propertyType.Id);
                    dbProvider.AddImageTrack(model);
                }
            }
        }
        
        /// <summary>
        /// Delete image track info from database
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(int id)
        {
            dbProvider.RemoveImageTrack(id);
        }

        /// <summary>
        /// Update the progress of the init in case of db timeouts
        /// </summary>
        /// <param name="pageId"></param>
        public static void UpdateProgress(int pageId)
        {
            dbProvider.UpdateProgress(pageId);
        }

        /// <summary>
        /// Obtains access to non-public properties
        /// </summary>
        /// <param name="type">Object type</param>
        /// <param name="instance">Object instance</param>
        /// <param name="fieldName">Required field</param>
        /// <returns></returns>
        private static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = type.GetProperty(fieldName, bindFlags);
            return field.GetValue(instance, null);
        }

        /// <summary>
        /// Creates a new imagetracker model object
        /// </summary>
        /// <param name="pageId">Id of content page</param>
        /// <param name="imageId">Id of image</param>
        /// <param name="propertyId">Id of umbraco property</param>
        /// <returns>ImageTrackerDbModel object</returns>
        private static ImageTrackerDbModel EnsureModel(int pageId, int imageId, int propertyId)
        {
            return new ImageTrackerDbModel
            {
                PageId = pageId,
                ImageId = imageId,
                PropertyId = propertyId
            };
        }
    }
}