/*
*  Gavlar50.ImageTracker - AngularJS module for selecting and reporting image usage in Umbraco content
*
*  Credits
*  =======
*  great article on tables with angular: https://ciphertrick.com/2015/06/01/search-sort-and-pagination-ngrepeat-angularjs/
*  dirPagination library by Michael Bromley https://github.com/michaelbromley/angularUtils/tree/master/src/directives/pagination
*  csv export table data https://codepen.io/YuvarajTana/pen/yNoNdZ
*/

"use strict";

/*
*  must get the existing umbraco module otherwise it gets recreated
*  see creation versus retrieval: https://stackoverflow.com/questions/30187977/use-angular-datatable-in-umbraco-7-custom-section
*/
var app = angular.module("umbraco");

//  now we can inject the dirPagination library to our umbraco app
app.requires.push("angularUtils.directives.dirPagination");

//  the ImageTrackerController logic
app.controller("Gavlar50.ImageTrackerController", function ($scope, $http, dialogService) {
    //var vm = this;

//  Toolbar button status flags. These control when buttons are enabled or displayed as active
    $scope.btnDisabled = true;
    $scope.btnUsageActive = false;
    $scope.btnAllActive = false;
    $scope.btnUnusedActive = false;
    $scope.tableActive = false;

//  Table column flags. These toggle the visible columns for each report.
    $scope.Columns = [];
    $scope.Columns.PageId = false;
    $scope.Columns.Page = false;
    $scope.Columns.Property = false;
    $scope.Columns.Image = false;
    $scope.Columns.Size = false;
    $scope.Columns.Location = false;

    $scope.pageSize = 10;

//  Use the umbraco media picker to choose an image from the media library
    $scope.imageSelector = function () {
        dialogService.mediaPicker({
            startNodeId: 0,
            multiPicker: false,
            callback: function (data) {
                data = [data];
                _.each(data, function (media) {
                    //console.dir(media); // adds the object to the chrome console as a viewable object
                    $scope.Id = media.id;
                    $scope.Url = media.image;
                    $scope.Filename = media.name;
                    $scope.Bytes = media.properties.find(x => x.alias === "umbracoBytes").value;
                    $scope.Width = media.originalWidth;
                    $scope.Height = media.originalHeight;
                });
                $scope.btnDisabled = false;
            }
        });
    };

//  Runs the active report. Url is the associated controller action that queries and returns the json data for the report
    $scope.report = function (url) {
        return $http({
            method: "GET",
            url: url
        }).then(function (res) {
            // strip the \ chars from \" and leading/trailing " chars from the JSON string otherwise JSON.parse fails
            var data = res.data.replace(/\\/g, ""); // remove all \ chars - all quotes are encoded as \"
            data = data.replace("\"[", "["); // leading "
            data = data.replace("]\"", "]"); // trailing "
            $scope.results = JSON.parse(data);
            $scope.tableActive = true;
        });
    };

//  Usages report
    $scope.usages = function (id) {
        // set the button status
        $scope.btnUsageActive = true;
        $scope.btnAllActive = false;
        $scope.btnUnusedActive = false;

        // toggle visible table columns
        $scope.Columns.PageId = true;
        $scope.Columns.Page = true;
        $scope.Columns.Property = true;
        $scope.Columns.Image = false;
        $scope.Columns.Size = false;
        $scope.Columns.Location = false;

        var url = "/umbraco/api/imagetracker/usages?id=" + id;
        return $scope.report(url);
    };

//  Everything report
    $scope.everything = function () {
        // set the button status
        $scope.btnUsageActive = false;
        $scope.btnAllActive = true;
        $scope.btnUnusedActive = false;

        // toggle visible table columns
        $scope.Columns.PageId = true;
        $scope.Columns.Page = true;
        $scope.Columns.Property = true;
        $scope.Columns.Image = true;
        $scope.Columns.Size = true;
        $scope.Columns.Location = true;

        return $scope.report("/umbraco/api/ImageTracker/everything");
    };

//  Unused report
    $scope.unused = function () {
        // set the button status
        $scope.btnUsageActive = false;
        $scope.btnAllActive = false;
        $scope.btnUnusedActive = true;

        // toggle visible table columns
        $scope.Columns.PageId = false;
        $scope.Columns.Page = false;
        $scope.Columns.Property = false;
        $scope.Columns.Image = true;
        $scope.Columns.Size = true;
        $scope.Columns.Location = true;

        return $scope.report("/umbraco/api/ImageTracker/unused");
    };

//  Table column sorter as discussed in the ciphertrick.com article
    $scope.sort = function (keyname) {
        $scope.sortKey = keyname; // where keyname is the ImageTrackerModel property to sort by 
        $scope.reverse = !$scope.reverse;
    };

//  CSV export, based on the AngularJS directive in Juvaraj's CSV article
    $scope.export = function () {
        var csvString = '';
        var headerString = '';
        for (var i = 0; i < $scope.results.length; i++) {
            var rowData = $scope.results[i];
            if ($scope.Columns.PageId) {
                if (i === 0) {
                    headerString = headerString + 'Page Id,';
                }
                csvString = csvString + '"' + rowData.PageId + '",'; 
            }
            if ($scope.Columns.Page) {
                if (i === 0) {
                    headerString = headerString + 'Page,Page Url,';
                }
                csvString = csvString + '"' + rowData.Page + '",';
                csvString = csvString + '"' + rowData.PageUrl + '",';
            }
            if ($scope.Columns.Property) {
                if (i === 0) {
                    headerString = headerString + 'Property,';
                }
                csvString = csvString + '"' + rowData.Property + '",'; 
            }
            if ($scope.Columns.Image) {
                if (i === 0) {
                    headerString = headerString + 'Image,Image Url,';
                }
                csvString = csvString + '"' + rowData.Image + '",';
                csvString = csvString + '"' + rowData.ImageUrl + '",';
            }
            if ($scope.Columns.Size) {
                if (i === 0) {
                    headerString = headerString + 'Size,';
                }
                csvString = csvString + '"' + rowData.Size + '",'; 
            }
            if ($scope.Columns.Location) {
                if (i === 0) {
                    headerString = headerString + 'Location,';
                }
                csvString = csvString + '"' + rowData.Location + '",'; 
            }
            csvString = csvString.substring(0, csvString.length - 1);
            csvString = csvString + "\n";
        }
        headerString = headerString.substring(0, headerString.length - 1);
        headerString = headerString + "\n";
        csvString = csvString.substring(0, csvString.length - 1);
        var a = $('<a/>', {
            style: 'display:none',
            href: 'data:application/octet-stream;base64,' + btoa(headerString + csvString),
            download: 'imagetrackerreport.csv'
        }).appendTo('body');
        a[0].click();
        a.remove();
    };

//  sets the number of results per page in the table
    $scope.setTablePages = function (newPageSize) {
        $scope.pageSize = newPageSize;
    };
});