// ============================================================
//  SmaczaPL — formularz dodawania przepisu
//  Czysty JavaScript (Vanilla JS).
//  Skrypt obsługuje TYLKO dynamiczne dodawanie/usuwanie wierszy składników.
//  Samo zapisanie przepisu robi serwer (POST Home/Add) — po wysłaniu
//  formularza przechodzimy na stronę główną, gdzie widać nowy przepis.
// ============================================================

document.addEventListener("DOMContentLoaded", function () {
    const template = document.getElementById("ingredientRowTemplate");
    const ingredientList = document.getElementById("ingredientList");
    const addIngredientBtn = document.getElementById("addIngredientBtn");

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

    // Na start pokazujemy jeden pusty wiersz, a przycisk dokłada kolejne.
    addIngredientRow();
    addIngredientBtn.addEventListener("click", addIngredientRow);
});
