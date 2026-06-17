// ============================================================
//  SmaczaPL — formularz dodawania przepisu (tylko frontend)
//  Czysty JavaScript (Vanilla JS). Dodany przepis pokazujemy w podglądzie
//  na stronie — nie zapisujemy go nigdzie (to zadanie backendu).
// ============================================================

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("addRecipeForm");
    const template = document.getElementById("ingredientRowTemplate");
    const ingredientList = document.getElementById("ingredientList");
    const addIngredientBtn = document.getElementById("addIngredientBtn");
    const previewGrid = document.getElementById("previewGrid");
    const previewEmpty = document.getElementById("previewEmpty");

    // Dodaje nowy wiersz składnika (klonuje szablon z widoku).
    function addIngredientRow() {
        const row = template.content.firstElementChild.cloneNode(true);

        // Obsługa przycisku usuwania tego wiersza
        row.querySelector(".ing-remove").addEventListener("click", function () {
            // Zostawiamy zawsze przynajmniej jeden wiersz składnika
            if (ingredientList.querySelectorAll(".ingredient-row").length > 1) {
                row.remove();
            }
        });

        ingredientList.appendChild(row);
    }

    // Na start pokazujemy jeden wiersz składnika
    addIngredientRow();

    // Kliknięcie "+ Dodaj składnik" dokłada kolejny wiersz
    addIngredientBtn.addEventListener("click", addIngredientRow);

    // Wysłanie formularza — budujemy kartę podglądu na stronie
    form.addEventListener("submit", function (event) {
        event.preventDefault(); // nie wysyłamy nic na serwer

        const title = document.getElementById("recipeTitle").value.trim();
        const category = document.getElementById("recipeCategory").value;
        const time = document.getElementById("recipeTime").value;

        // Prosta walidacja: nazwa jest wymagana
        if (title === "") {
            alert("Podaj nazwę dania.");
            return;
        }

        // Zbieramy składniki z wszystkich wierszy do tablicy tekstów
        const rows = ingredientList.querySelectorAll(".ingredient-row");
        const ingredients = [];
        rows.forEach(function (row) {
            const name = row.querySelector(".ing-name").value;
            const amount = row.querySelector(".ing-amount").value;
            const unit = row.querySelector(".ing-unit").value;
            ingredients.push(amount + " " + unit + " " + name);
        });

        // Budujemy kartę bezpiecznie (createElement + textContent — bez wstrzykiwania HTML)
        const card = document.createElement("article");
        card.className = "recipe-card preview-card";

        const top = document.createElement("div");
        top.className = "preview-card-top";
        top.textContent = "🍴";
        card.appendChild(top);

        const body = document.createElement("div");
        body.className = "recipe-card-body";

        const badge = document.createElement("span");
        badge.className = "badge-kat";
        badge.textContent = category;
        body.appendChild(badge);

        const heading = document.createElement("h2");
        heading.textContent = title;
        body.appendChild(heading);

        const timeP = document.createElement("p");
        timeP.className = "time";
        timeP.textContent = "⏱ " + time + " min";
        body.appendChild(timeP);

        const ul = document.createElement("ul");
        ul.className = "preview-ingredients";
        ingredients.forEach(function (text) {
            const li = document.createElement("li");
            li.textContent = text;
            ul.appendChild(li);
        });
        body.appendChild(ul);

        card.appendChild(body);

        // Dodajemy nową kartę na początek listy podglądu
        previewGrid.prepend(card);
        previewEmpty.hidden = true;

        // Czyścimy formularz i wracamy do jednego pustego wiersza składnika
        form.reset();
        ingredientList.innerHTML = "";
        addIngredientRow();
    });
});
