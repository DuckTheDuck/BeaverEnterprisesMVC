

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

//Javascrip do Login Gabriel

const forms = document.querySelector(".forms"),
    pwShowHide = document.querySelectorAll(".eye-icon"),
    links = document.querySelectorAll(".link");
// Add click event listener to each eye icon for toggling password visibility
pwShowHide.forEach(eyeIcon => {
    eyeIcon.addEventListener("click", () => {
        let pwFields = eyeIcon.parentElement.parentElement.querySelectorAll(".password");
        pwFields.forEach(password => {
            if (password.type === "password") { // If password is hidden
                password.type = "text"; // Show password
                eyeIcon.classList.replace("bx-hide", "bx-show"); // Change icon to show state
                return;
            }
            password.type = "password"; // Hide password
            eyeIcon.classList.replace("bx-show", "bx-hide"); // Change icon to hide state
        });
    });
});
// Add click event listener to each link to toggle between forms
links.forEach(link => {
    link.addEventListener("click", e => {
        e.preventDefault(); // Prevent default link behavior
        forms.classList.toggle("show-signup");
    });
});

//Fim do Javascript do Gabriel