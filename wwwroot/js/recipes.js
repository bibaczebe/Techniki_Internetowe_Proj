// ============================================================
//  SmaczaPL — wyszukiwanie i filtrowanie przepisów
//  Czysty JavaScript (Vanilla JS) — bez jQuery i bez bibliotek.
//  Skrypt działa po stronie przeglądarki na stronie głównej.
// ============================================================

// Czekamy, aż cała strona się załaduje, zanim sięgniemy po elementy.
document.addEventListener("DOMContentLoaded", function () {

    // Pobieramy potrzebne elementy ze strony
    const searchInput = document.getElementById("searchInput");   // pole wyszukiwania
    const categoryButtons = document.querySelectorAll(".cat-btn"); // przyciski kategorii
    const cards = document.querySelectorAll(".recipe-card");        // karty przepisów
    const noResults = document.getElementById("noResults");        // komunikat o braku wyników

    // Zapamiętujemy aktualnie wybraną kategorię ("all" = wszystkie)
    let selectedCategory = "all";

    // Główna funkcja: pokazuje lub ukrywa karty zależnie od kategorii i wpisanego tekstu.
    function filterRecipes() {
        const text = searchInput.value.trim().toLowerCase(); // tekst z wyszukiwarki
        let visibleCount = 0;                                 // licznik widocznych kart

        cards.forEach(function (card) {
            const title = card.getAttribute("data-title");        // tytuł przepisu (małe litery)
            const category = card.getAttribute("data-category");  // id kategorii przepisu

            // Sprawdzamy dwa warunki naraz:
            const matchesCategory = (selectedCategory === "all" || category === selectedCategory);
            const matchesText = title.includes(text);

            if (matchesCategory && matchesText) {
                card.hidden = false; // pokaż kartę
                visibleCount++;
            } else {
                card.hidden = true;  // ukryj kartę
            }
        });

        // Jeśli żadna karta nie pasuje — pokazujemy komunikat
        noResults.hidden = (visibleCount > 0);
    }

    // Reagujemy na każde wpisanie znaku w wyszukiwarce
    searchInput.addEventListener("input", filterRecipes);

    // Reagujemy na kliknięcie przycisku kategorii
    categoryButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            // Usuwamy podświetlenie ze wszystkich przycisków...
            categoryButtons.forEach(function (b) {
                b.classList.remove("active");
            });
            // ...i podświetlamy kliknięty
            button.classList.add("active");

            // Zapisujemy wybraną kategorię i filtrujemy listę
            selectedCategory = button.getAttribute("data-category");
            filterRecipes();
        });
    });
});
