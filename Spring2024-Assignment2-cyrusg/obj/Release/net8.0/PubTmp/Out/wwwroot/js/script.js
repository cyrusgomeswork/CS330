
// Function to make an API search call
function apiSearch() {
    var params = {
        'q': $('#query').val(),
        'count': 50,
        'offset': 0,
        'mkt': 'en-us'
    };

    $.ajax({
        url: 'https://api.bing.microsoft.com/v7.0/search?' + $.param(params),
        type: 'GET',
        headers: {
            'Ocp-Apim-Subscription-Key': '9463d3a2cbd24a9f8f68ff5a1947fedb' // Replace with your valid Bing Search API key
        }
    })
    .done(function (data) {
        var len = data.webPages.value.length;
        var results = '';
        for (i = 0; i < len; i++) {
            results += `<p><a href="${data.webPages.value[i].url}">${data.webPages.value[i].name}</a>: ${data.webPages.value[i].snippet}</p>`;
        }

        $('#searchResults').html(results);
        $('#searchResults').dialog({
            title: 'Search Results'
        });
    })
    .fail(function () {
        alert('Error performing the search.');
    });
}

// Function to display current time
function showTime() {
    var now = new Date();
    var timeString = now.getHours() + ":" + (now.getMinutes() < 10 ? '0' : '') + now.getMinutes();
    $('#time').html(`<p>Current Time: ${timeString}</p>`);
    $('#time').css('visibility', 'visible').dialog({
        title: 'Current Time'
    });
}

// Function to change background image on header click
let imageToggle = false;
function changeBackground() {
    if (imageToggle) {
        $('body').css('background-image', 'url(../css//1st_photo.jpeg)');
    } else {
        $('body').css('background-image', 'url(../css//2nd_photo.jpeg)');
    }
    imageToggle = !imageToggle;
}

// Function to handle 'I'm Feeling Lucky' button
function imFeelingLucky() {
    var params = {
        'q': $('#query').val(),
        'count': 1,  // Request only 1 result
        'offset': 0,
        'mkt': 'en-us'
    };

    $.ajax({
        url: 'https://api.bing.microsoft.com/v7.0/search?' + $.param(params),
        type: 'GET',
        headers: {
            'Ocp-Apim-Subscription-Key': '9463d3a2cbd24a9f8f68ff5a1947fedb'  // Replace with your valid Bing Search API key
        }
    })
    .done(function (data) {
        // Check if there is at least one result
        if (data.webPages && data.webPages.value.length > 0) {
            // Redirect to the URL of the first result
            window.location.href = data.webPages.value[0].url;
        } else {
            alert('No results found.');
        }
    })
    .fail(function () {
        alert('Error performing the search.');
    });
}

// Event listener for the "I'm Feeling Lucky" button
$(document).ready(function () {
    $('#luckyButton').click(imFeelingLucky); // Bind the 'I'm Feeling Lucky' button
});

// Event listeners for buttons
$(document).ready(function () {
    $('#searchButton').click(apiSearch);
    $('#timeButton').click(showTime);
    
    // Remove the redundant click event binding and the page reload
    $('#searchEngineName').click(function() {
        changeBackground(); // Call the changeBackground function on header click
    });

    $('#searchEngineName').css('cursor', 'pointer'); // Shows pointer cursor

    // Optionally, load an initial background (if you want one image to show when the page loads)
    $('body').css('background-image', 'url(../css/1st_photo.jpeg)'); // Load the first image by default
});