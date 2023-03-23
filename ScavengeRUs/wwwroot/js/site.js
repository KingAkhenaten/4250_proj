"using strict"

// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*
 * Library-style JS should go here. Beware functions that are async and accept callbacks. These are marked with Async suffixes.
 */


/*
 * ## Location-handling API ##
 * 
 * Contains async functions for querying the user GPS location, if able. This wraps the slightly lower
 * level browser API. Locations are handled as GeolocationCoordinates objects. For more reference,
 * see <https://developer.mozilla.org/en-US/docs/Web/API/GeolocationCoordinates>. Note that not all devices
 * can return altitude, altitude accuracy, speed or heading information. They will be null if this is the case.
 * 
 * All latitude/longitude units are handled internally as decimal degrees where positive values are north/east
 * and negative values are south/west. ETSU, for reference, is north and west, so latitude should be positive,
 * and longitude should be negative (if this isn't the case, something is wrong!). Degrees-minutes-seconds format
 * is not used.
 * 
 * All distance units are internally handled as metric metres. Functions are available for conversion to
 * American customary/Imperial units for display purposes.
 * 
 * Only the latitude, longitude, and accuracy values should be relevant for BucHunt. A device capable of location
 * services should provide all three values or none of them.
 * 
 * GeolocationCoordinates {
 *      double latitude; // latitude in decimal degrees
 *      double longitude; // longitude in decimal degress
 *      double? altitude; // altitude in metres, or null if not present
 *      double accuracy; // accuracy of latitude/longitude in metres
 *      double? altitudeAccuracy; // altitude accuracy in metres, or null if not present
 *      double? heading; // user's heading in degrees from [0,360), NaN if speed is 0, or null if not present
 *      double? speed; // user's speed in metres per second, or null if not present
 * }
 * 
 * An example use can be seen in Questions/Index.cshtml.
 */

/*
 * Get the user's location async.
 * On success, callbackSuccess is called with a GeolocationCoordinates object describing the coordinates of the user at highest
 * accuracy available. On failure, callbackError is called with an error code describing why the call failed.
 * At most 5 seconds will be taken to determine location.
 * Error codes:
 * 1 - Device does not support geolocation
 * 2 - User denied geolocation permission
 * 3 - Not enough functioning geolocators
 * 4 - Call timed out before data could be acquired (5 seconds)
 */
function getLocationAsync(callbackSuccess,callbackError) {
    var geoapi = navigator.geolocation;
    if (geoapi === undefined) {
        setTimeout(() => callbackError(1), 0); // device does not support
    } else {
        // Use a 5 second timeout
        geoapi.getCurrentPosition(callWithLocationSuccess, callWithLocationFailure, { enableHighAccuracy: true, timeout: 5000 });
    }
    // CALLBACKS/PRIVATE
    function callWithLocationSuccess(gpLocation) {
        var coords = gpLocation.coords;
        console.log('success');
        setTimeout(() => callbackSuccess(coords), 0); // success
    }

    function callWithLocationFailure(gpError) {
        switch (gpError.code) {
            case 1: // permission denied
                setTimeout(() => callbackError(2), 0);
                break;
            case 2: // not enough locators
                setTimeout(() => callbackError(3), 0);
                break;
            case 3: // timed out
                setTimeout(() => callbackError(4), 0);
                break;
        }
    }
}

/*
 * Given a GeolocationCoordinates object, and the decimal forms of the target's latitude and longitude,
 * determines the player distance to the target in metres.
 */
function distanceToLocation(coords, targetLat, targetLon) {
    var playerLat = coords.latitude;
    var playerLon = coords.longitude;
    // Turns out this is surprisingly difficult to do. Lat/lon coordinates do not map to linear distances
    // in the obvious sense.

    // This is based on errata from FCC rules on distance measurement for radio stations to
    // avoid interference and is only applicable for distances no larger than 475 km or 295 miles.
    // It is unlikely that a hunt will have distances of this length to where accuracy would be a problem at this extreme.

    // For trig functions, it is not clear whether degrees or radians are in use.
    // The mathematical definition using radians is assumed.

    // https://www.govinfo.gov/content/pkg/CFR-2016-title47-vol4/pdf/CFR-2016-title47-vol4-sec73-208.pdf
    // section 73.209, p. 87

    // calculate middle latitude
    var middleLatitude = (playerLat + targetLat) / 2;
    // calculate kilometres per degree latitude difference for the middle latitude
    var kiloPerDegDiff = 111.13209 - 0.56605 * Math.cos(2 * middleLatitude) + 0.00120 * Math.cos(4 * middleLatitude);
    // calculate North-South distance
    var nsDist = kiloPerDegDiff * (playerLat - targetLat);
    // calculate East-West distance
    var ewDist = kiloPerDegDiff * (playerLon - targetLon);
    // Now take the Pythagorean using these two NS and EW distances giving kilometres
    var distKm = Math.hypot(nsDist, ewDist);
    // Convert to metres
    return distKm * 1000;
}

// Above testing coords
// Ross Hall                36DEG18'00"N 82DEG22'12"W => 36.30000000N 82.37000000W (lossy conversions)
// DM Centre                36DEG18'20"N 82DEG22'18"W => 36.30555556N 82.37166667W
// Google claims 636.57 metres linear
// Manual test with the above numbers is 647.6763 metres for an error of 11.1 metres (very close.)
// Therefore this sanity tests okay at least done by hand in Java.

/*
 * Convert metres to feet.
 */
function metresToFeet(v) {
    return 3.2808399 * v;
}

/*
 * Return a friendly string IN IMPERIAL for a given distance in METRES.
 */
function distanceToStringImperial(distInMetres) {
    var distInFeet = metresToFeet(distInMetres);
    const FEET_IN_MILE = 5280; // 5280 ft = 1mi
    if (dist < metresToFeet(5)) {
        return 'Here';
    } else if (dist < FEET_IN_MILE) {
        return (dist) + 'ft';
    } else {
        return (dist / FEET_IN_MILE).toFixed(2) + 'mi'; // 2 digits fractional miles
    }
}

/*
 * Return a friendly string IN METRIC for a given distance in METRES.
 */
function distanceToStringMetric(distInMetres) {
    if (dist < 5) {
        return 'Here';
    } else if (dist < 1000) {
        return (dist) + 'm';
    } else {
        return (dist / 1000).toFixed(2) + 'Km'; // 2 digits fractional kilometres
    }
}


var offcampus = document.getElementById('offcanvas');
var sideBarOpen = document.getElementById("openSidebar"); //Open sidebar on the hunt page
sideBarOpen.addEventListener('click', e => {
    document.getElementById("toggleSidebar").click();
    document.getElementById("taskarea").style.marginRight = "0px";
})

var sideBarClose = document.getElementById("closeSidebar"); //Close sidebar on hunt page
sideBarClose.addEventListener('click', e => {
    document.getElementById("toggleSidebar").click();
    document.getElementById("taskarea").style.marginRight = "0";
});

(function _homeIndexMain() {        //This function handles the modal on the hunt page
    const createTaskModalDOM = document.querySelector("#createTaskModal");
    const createTaskModal = new bootstrap.Modal(createTaskModalDOM);
    const createTaskButton = document.querySelectorAll("#btnCreateTask");
 //   console.log(createTaskButton);
    createTaskButton.forEach(item => {
        item.addEventListener("click", event => {
            var TaskId = $(item).data("id");
            var HuntId = $(item).data("huntid");
            var Task = $(item).data("task");

            $('#TaskIdInput').val(TaskId);  //Passing parameters to the modal
            $('#HuntIdInput').val(HuntId);
            $('#TaskInput').text(Task); //Set the task question in the modal
            console.log($('a[data-id="' + TaskId + '"] #status').text());
            if ($('a[data-id="' + TaskId + '"] #status').text() == "Incomplete") { //Only show modal if task is incomplete
                createTaskModal.show();
            }
        })
    })
    $("#createTaskModal").submit(function (event) { //On modal submit it passes the form data with huntid, taskid, and answer to an AJAX request in the locations controller
        var formData = {
            id: $("#HuntIdInput").val(),
            taskid: $("#TaskIdInput").val(),
            answer: $("#AnswerInput").val(),
        };
        $.ajax({
            type: "POST",
            url: "../../Locations/Validateanswer",
            data: formData,
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    $('#successMessageArea').html("Success! Task Completed.");
                    $('#alertAreaSuccess').show();
                    //console.log(formData.taskid);
                    $('a[data-id="' + formData.taskid + '"] #status').text('Completed').css({ "color": "Green" }); //Change text to completed if success
                    // After 500ms, fade out over 500ms
                    setInterval(() => {
                        $('#alertAreaSuccess').fadeOut(1500);
                    }, 1500);
                    setTimeout(() => {
                        createTaskModal.hide();
                    }, 1500);
                }
                else {
                    $('#failedMessageArea').html("Incorrect! Try again.");
                    $('#alertAreaFailed').show();
                    // After 500ms, fade out over 500ms
                    setInterval(() => {
                        $('#alertAreaFailed').fadeOut(1500);
                    }, 1500); 
                }
            },
        }).done(function (data) {
            console.log(data);
        });
        event.preventDefault();
    });
}());