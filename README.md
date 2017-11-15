#Gavlar50.ImageTracker

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

###Property Match Handler
The property match handler static class registers matchers for the image capable property editors. When content is published, all
published entities are passed to the property match handler. The match handler finds a matcher that is registered to handle the
editor type and records any references to images in the media tree.

###Matchers
The matchers implement the IImageMatcher interface. This declares which property editor types the matcher will handle. A regex 
expression is used to match the image references as stored by the specific property editors. The handler returns a list of all
image ids for storage.

You can add your own match handlers by creating classes that implement the IImageMatcher interface and providing the appropriate
regex expression for the raw data as stored in the cmsPropertyData table. These custom handlers should then be registered in the
PropertyMatchHandler static class ImageHandlers collection.