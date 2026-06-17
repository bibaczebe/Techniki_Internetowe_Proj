// ============================================================
//  SmaczaPL — "Co mam w lodówce" (tylko frontend)
//  Czysty JavaScript (Vanilla JS). Porównujemy składniki zaznaczone przez
//  użytkownika ze składnikami przepisów i pokazujemy te, które można
//  (lub prawie można) ugotować.
// ============================================================

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("fridgeForm");
    const clearBtn = document.getElementById("clearFridge");
    const cards = document.querySelectorAll(".fridge-card");
    const hint = document.getElementById("fridgeHint");

    // Składniki podstawowe — zakładamy, że każdy ma je w domu, więc ich nie wymagamy.
    const basics = ["sól", "pieprz", "olej", "cukier"];

    // Ile składników może maksymalnie brakować, żeby wciąż pokazać przepis.
    const maxMissing = 2;

    // Główna funkcja: sprawdza każdy przepis i pokazuje pasujące.
    function search() {
        // Zbieramy zaznaczone składniki użytkownika do tablicy
        const have = [];
        form.querySelectorAll('input[type="checkbox"]:checked').forEach(function (c) {
            have.push(c.value);
        });

        // Jeśli nic nie zaznaczono — prosimy o zaznaczenie
        if (have.length === 0) {
            cards.forEach(function (card) { card.hidden = true; });
            hint.hidden = false;
            hint.textContent = "Zaznacz przynajmniej jeden składnik.";
            return;
        }

        let shown = 0;

        cards.forEach(function (card) {
            // Składniki przepisu (rozdzielone "|")
            const ingredients = card.getAttribute("data-ingredients").split("|");

            // Wyszukujemy składniki, których użytkownik NIE ma (pomijając podstawowe)
            const missing = ingredients.filter(function (ing) {
                if (basics.indexOf(ing) !== -1) return false; // podstawowe pomijamy
                return have.indexOf(ing) === -1;              // brakuje, jeśli nie zaznaczono
            });

            const note = card.querySelector(".match-note");

            if (missing.length === 0) {
                // Mamy wszystko — przepis w pełni wykonalny
                card.hidden = false;
                card.classList.add("can-make");
                note.textContent = "✓ Możesz to zrobić!";
                shown++;
            } else if (missing.length <= maxMissing) {
                // Prawie się da — pokazujemy, czego brakuje
                card.hidden = false;
                card.classList.remove("can-make");
                note.textContent = "Brakuje: " + missing.join(", ");
                shown++;
            } else {
                // Za dużo brakuje — ukrywamy
                card.hidden = true;
            }
        });

        // Komunikat, gdy nic nie pasuje
        if (shown === 0) {
            hint.hidden = false;
            hint.textContent = "Brak pasujących przepisów. Zaznacz więcej składników.";
        } else {
            hint.hidden = true;
        }
    }

    // Wysłanie formularza = uruchomienie wyszukiwania
    form.addEventListener("submit", function (event) {
        event.preventDefault();
        search();
    });

    // Przycisk "Wyczyść" — odznacza wszystko i chowa wyniki
    clearBtn.addEventListener("click", function () {
        form.querySelectorAll('input[type="checkbox"]:checked').forEach(function (c) {
            c.checked = false;
        });
        cards.forEach(function (card) { card.hidden = true; });
        hint.hidden = false;
        hint.textContent = "Zaznacz składniki i kliknij „Szukaj przepisów\".";
    });
});
