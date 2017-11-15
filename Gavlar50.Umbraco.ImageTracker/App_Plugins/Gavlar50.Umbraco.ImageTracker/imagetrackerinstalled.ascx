<h2>Image Tracker installed</h2>
<p>Images are now tracked as content is published. This plugin has added a new tab to the media pane where you can report on image usage.</p>
<p>Usage - select an image to see the pages and properties where it is used</p>
<p>Everything - list all images that are tracked across all pages and properties</p>
<p>Unused - list all images that are not referenced by any page properties</p>
<p>Export - export the report to csv</p>
<h3>Tracking existing content</h3>
<p>Run the <a href="/umbraco/api/imagetracker/init">/umbraco/api/imagetracker/init</a> action if this site already contains content. This will check all pages/properties and create the tracking info. If this site contains a lot of content and the init process times out, just rerun the init process again. The init process will pick up from where it timed out and continue. Once the success message is displayed the process is complete and all existing content is being tracked.</p>