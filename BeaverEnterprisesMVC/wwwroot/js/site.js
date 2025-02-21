document.addEventListener("DOMContentLoaded", function () {
    // Delegação de evento para garantir que os botões funcionem mesmo após mudanças de página
    document.addEventListener("click", function (event) {
        if (event.target.classList.contains("cta-button")) {
            changeView(event, '_MainLayout');
        }
    });

    // Chama a função changeView inicialmente
    changeView({ target: document.querySelector(".cta-button.active") }, '_MainLayout');
});

function changeView(event, view) {
    // Verifica se event.target é válido antes de tentar acessar classList
    if (!event.target) return;

    // Remover a classe 'active' de todos os botões
    document.querySelectorAll('.cta-button').forEach(btn => btn.classList.remove('active'));

    // Adicionar a classe 'active' ao botão clicado
    event.target.classList.add('active');

    // Se for uma Partial View, faz uma requisição AJAX
    if (view === '_MainLayout' || view === '_Login') {
        $.get("/Home/GetPartialView", { viewName: view }, function (data) {
            document.getElementById('shared-view').innerHTML = data;


            // Aguarda um pequeno tempo para garantir que o HTML foi atualizado antes de adicionar eventos
            setTimeout(() => {
                const origin = document.getElementById("airport-input-Origin");
                const destination = document.getElementById("airport-input-Destination");

                if (origin) {
                    origin.addEventListener("input", () => suggest(origin, "suggestionsOrigin"));
                }
                if (destination) {
                    destination.addEventListener("input", () => suggest(destination, "suggestionsDestination"));
                }
            }, 200);
        });
    }
}




function validateForm() {

    var origin = document.getElementById("airport-input-Origin").value.trim();
    var destination = document.getElementById("airport-input-Destination").value.trim();
    var departure = document.getElementById("departure").value;
    var arrival = document.getElementById("arrival").value;

    if (!origin || !destination || !departure || !arrival) {alert("Por favor, preencha todos os campos antes de continuar!");return false;}
    if (origin === destination) {alert("Não podes ter o mesmo local para origem e destino!"); return false;}

    var today = new Date();  
    today.setHours(0, 0, 0, 0); 

    var departureDate = new Date(departure + "T00:00:00"); 
    var arrivalDate = new Date(arrival + "T00:00:00");

    if (departureDate < today) {alert("A data de partida não pode ser inferior à data de hoje!");return false;}
    if (arrivalDate < departureDate) {alert("A data de chegada não pode ser antes da data de partida!");return false;}

    return true; 
}


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