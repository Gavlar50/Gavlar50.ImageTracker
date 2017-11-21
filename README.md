# Gavlar50.ImageTracker

Gavlar50.ImageTracker is an image tracker plugin for Umbraco 7.7.4+. It is a rewrite of the old Gavlar50.MediaTracker Umbraco 6 plugin. 

The plugin detects references to images in the Media tree when content is saved by examining the document types during publish.

Out-the-box the following property editor types are supported:
* Image Cropper
* Media Picker
* Multi Node Tree Picker
* Multiple Media Picker
* Grid
* Media Picker 2
* Rich text editor (TinyMCE)
* Multi Node Tree Picker 2
* Nested Content

## Property Match Handler
The property match handler static class registers matchers for the image capable property editors. When content is published, all
published entities are passed to the property match handler. The match handler finds a matcher that is registered to handle the
editor type and records any references to images in the media tree.

## Matchers
The matchers implement the IImageMatcher interface. This declares which property editor types the matcher will handle. A regex 
expression is used to match the image references as stored by the specific property editors. The handler returns a list of all
image ids for storage.

You can add your own match handlers by creating classes that implement the IImageMatcher interface and providing the appropriate
regex expression for the raw data as stored in the cmsPropertyData table. These custom handlers should then be registered in the
PropertyMatchHandler static class ImageHandlers collection.

## Dashboard
Add the following section to the dashboard.config just before the closing dashBoard tag for development and testing:

    <section alias="ImageTrackerSection">
     <areas>
       <area>media</area>
     </areas>
     <tab caption="Image Tracker">
       <control showOnce="false" addPanel="false" panelCaption="">
         ~/app_plugins/Gavlar50.Umbraco.Imagetracker/imagetracker.html
       </control>
     </tab>
   </section>

This will add the ImageTracker tab to the Media pane.

## Installing to an existing site
The umbraco/api/imagetracker/init controller action enables you to scan all content in the site and build the tracker data when
installing in an existing site that already contains data. If the site is large and this action times out you can simply rerun
this controller action and it will continue from the point of failure. This should be repeated until you see the success message
at which point all existing image usage is being tracked.

## Database
The gavlar50ImageTracker table is created in the Umbraco database to store the tracking information. It contains the following fields:
 Id - integer identity column
 ImageId - foreign key to the image data on umbraconode.id
 PageId - foreign key to the page info on cmsdocument.nodeid and umbraconode.id
 PropertyId - foreign key to the document property values on cmsPropertyData.propertytypeid

The gavlar50ImageTrackerProgress table stores the page id of every cmsdocument record that has been processed during the 
umbraco/api/init action. Whenever this action is run, if the table contains records it assumes that a previous run timed
out so it continues. Once this process is complete, this table is cleared. Therefore records in this table implies that
a full content scan is in progress.