// ============================================================
//  SmaczaPL — formularz dodawania przepisu
//  Czysty JavaScript (Vanilla JS).
//  - Składniki dodaje się dynamicznie (pole tekstowe z podpowiedziami).
//  - Jeśli wybrano zdjęcie, wysyłamy je AJAX-em na backend kolegi
//    (akcja Recipe/UploadImage, która zwraca JSON z adresem zapisanego pliku)
//    i pokazujemy je w podglądzie. Bez zdjęcia pokazujemy placeholder.
// ============================================================

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("addRecipeForm");
    const template = document.getElementById("ingredientRowTemplate");
    const ingredientList = document.getElementById("ingredientList");
    const addIngredientBtn = document.getElementById("addIngredientBtn");
    const previewGrid = document.getElementById("previewGrid");
    const previewEmpty = document.getElementById("previewEmpty");
    const imageInput = document.getElementById("imageFile");

    // Dodaje nowy wiersz składnika (klonuje szablon z widoku).
    function addIngredientRow() {
        const row = template.content.firstElementChild.cloneNode(true);

        row.querySelector(".ing-remove").addEventListener("click", function () {
            // Zostawiamy zawsze przynajmniej jeden wiersz składnika
            if (ingredientList.querySelectorAll(".ingredient-row").length > 1) {
                row.remove();
            }
        });

        ingredientList.appendChild(row);
    }

    addIngredientRow();
    addIngredientBtn.addEventListener("click", addIngredientRow);

    // Buduje kartę podglądu. imageUrl: adres zdjęcia (po uploadzie) albo null.
    function buildPreviewCard(title, category, time, ingredients, imageUrl) {
        const card = document.createElement("article");
        card.className = "recipe-card preview-card";

        if (imageUrl) {
            // Mamy zdjęcie zapisane na serwerze — pokazujemy je
            const img = document.createElement("img");
            img.src = imageUrl;
            img.alt = "Zdjęcie dania: " + title;
            card.appendChild(img);
        } else {
            // Brak zdjęcia — pokazujemy placeholder z ikoną
            const top = document.createElement("div");
            top.className = "preview-card-top";
            top.textContent = "🍴";
            card.appendChild(top);
        }

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

        previewGrid.prepend(card);
        previewEmpty.hidden = true;
    }

    // Wysłanie formularza
    form.addEventListener("submit", function (event) {
        event.preventDefault(); // obsługujemy wszystko sami (endpoint zwraca JSON, nie stronę)

        const title = document.getElementById("recipeTitle").value.trim();
        const category = document.getElementById("recipeCategory").value;
        const time = document.getElementById("recipeTime").value;

        // Prosta walidacja zgodna z wymaganiami backendu (nazwa min. 3 znaki)
        if (title.length < 3) {
            alert("Podaj nazwę dania (przynajmniej 3 znaki).");
            return;
        }

        // Zbieramy składniki z wierszy (pomijamy puste)
        const ingredients = [];
        ingredientList.querySelectorAll(".ingredient-row").forEach(function (row) {
            const name = row.querySelector(".ing-name").value.trim();
            if (name === "") return;
            const amount = row.querySelector(".ing-amount").value;
            const unit = row.querySelector(".ing-unit").value;
            ingredients.push(amount + " " + unit + " " + name);
        });

        // Czyści formularz i pokazuje kartę podglądu
        function finish(imageUrl) {
            buildPreviewCard(title, category, time, ingredients, imageUrl);
            form.reset();
            ingredientList.innerHTML = "";
            addIngredientRow();
        }

        // Czy wybrano plik ze zdjęciem?
        const file = imageInput && imageInput.files.length > 0 ? imageInput.files[0] : null;

        if (file) {
            // Budujemy dane zgodne z RecipeCreateViewModel kolegi (Title, Category, PrepTime, ImageFile)
            const data = new FormData();
            data.append("Title", title);
            data.append("Category", category);
            data.append("PrepTime", time);
            data.append("ImageFile", file);

            // Wysyłamy zdjęcie na backend (form.action = /Recipe/UploadImage)
            fetch(form.action, { method: "POST", body: data })
                .then(function (response) {
                    return response.ok ? response.json() : null;
                })
                .then(function (result) {
                    // Backend zwraca { imageUrl: "/images/..." }
                    finish(result && result.imageUrl ? result.imageUrl : null);
                })
                .catch(function () {
                    finish(null); // gdyby upload się nie udał — pokaż podgląd bez zdjęcia
                });
        } else {
            finish(null);
        }
    });
});
