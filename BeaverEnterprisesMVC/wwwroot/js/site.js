// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    const origin = document.getElementById("airport-input-Origin"); 
    const destination = document.getElementById("airport-input-Destination");

    origin.addEventListener("input", () => {

        suggest(origin, "suggestionsOrigin");

    });

    destination.addEventListener("input", () => {

        suggest(destination, "suggestionsDestination");

    });

    $(document).click(function (event) {
        if (!$("#airport-input").is(event.target) && !$("#suggestions").is(event.target) && $("#suggestions").has(event.target).length === 0) {
            $("#suggestions").hide();
        }
    });

    
});


function suggest(input, suggestion) {

    const userInput = input.value.toLowerCase();
    var suggestions = $("#" + suggestion);
    suggestions.empty().hide();

    if (userInput.length > 0) {
        $.getJSON("/Airport/GetSuggestions", { term: userInput }, function (data) {
            if (data.length > 0) {
                suggestions.show();

                data.forEach(function (aeroporto) {
                    var li = $("<li>").text(aeroporto).css({
                        "cursor": "pointer",
                        "padding": "5px"
                    });

                    li.on("click", function () {
                        $(input).val(aeroporto);
                        suggestions.hide();
                    });

                    suggestions.append(li);
                });
            }
        });
    }
}