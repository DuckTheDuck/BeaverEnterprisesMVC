document.addEventListener("DOMContentLoaded", function () {
    // Delegação de evento para garantir que os botões funcionem mesmo após mudanças de página


    // Chama a função changeView inicialmente
    changeView({ target: document.querySelector(".cta-button.active.index") }, '_MainLayout');
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

function changeViewLoginRegister(event, view) {
    if (!event.target) return;

    // Remover a classe 'active' de todos os botões
    document.querySelectorAll('.cta-button').forEach(btn => btn.classList.remove('active'));

    // Adicionar a classe 'active' ao botão clicado
    event.target.classList.add('active');

    // Seleciona a div principal onde a view será carregada
    const sharedView = document.getElementById('shared-view');

    // Remove todos os event listeners anteriores limpando o innerHTML corretamente
    sharedView.replaceChildren(); // Remove todos os elementos filhos da div

    // Carregar a nova Partial View
    $.get("/Home/GetPartialView", { viewName: view })
        .done(function (data) {
            sharedView.innerHTML = data; // Insere a nova view corretamente

            // Aguarda um curto tempo para garantir que os elementos foram adicionados ao DOM
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
        })
        .fail(function () {
            console.error("Erro ao carregar a partial view:", view);
            sharedView.innerHTML = "<p>Erro ao carregar a página. Tente novamente.</p>";
        });
}



function validateForm() {

    var origin = document.getElementById("airport-input-Origin").value.trim();
    var destination = document.getElementById("airport-input-Destination").value.trim();
    var departure = document.getElementById("departure").value;
    var arrival = document.getElementById("arrival").value;

    if (!origin || !destination || !departure || !arrival) { alert("Por favor, preencha todos os campos antes de continuar!"); return false; }
    if (origin === destination) { alert("Não podes ter o mesmo local para origem e destino!"); return false; }

    var today = new Date();
    today.setHours(0, 0, 0, 0);

    var departureDate = new Date(departure + "T00:00:00");
    var arrivalDate = new Date(arrival + "T00:00:00");

    if (departureDate < today) { alert("A data de partida não pode ser inferior à data de hoje!"); return false; }
    if (arrivalDate < departureDate) { alert("A data de chegada não pode ser antes da data de partida!"); return false; }

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