// ============================================================
//  SmaczaPL — "Co mam w lodówce" (tylko frontend)
//  Czysty JavaScript (Vanilla JS). Użytkownik WPISUJE składniki w polu
//  (z podpowiedziami), dodaje je jako "chipy", a my pokazujemy przepisy,
//  które można (lub prawie można) z nich ugotować.
// ============================================================

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("fridgeForm");
    const input = document.getElementById("fridgeInput");
    const addBtn = document.getElementById("addFridgeIngredient");
    const chipsList = document.getElementById("fridgeChips");
    const clearBtn = document.getElementById("clearFridge");
    const cards = document.querySelectorAll(".fridge-card");
    const hint = document.getElementById("fridgeHint");

    // Składniki podstawowe — zakładamy, że każdy ma je w domu, więc ich nie wymagamy.
    const basics = ["sól", "pieprz", "olej", "cukier", "woda"];

    // Ile składników może maksymalnie brakować, żeby wciąż pokazać przepis.
    const maxMissing = 2;

    // Lista składników wpisanych przez użytkownika (małymi literami)
    const have = [];

    // Dodaje składnik jako "chip" (jeśli nie jest pusty i nie ma go już na liście)
    function addChip(rawName) {
        const name = rawName.trim().toLowerCase();
        if (name === "" || have.indexOf(name) !== -1) {
            input.value = "";
            return;
        }
        have.push(name);

        // Tworzymy element chipa: tekst + przycisk usuwania
        const chip = document.createElement("li");
        chip.className = "chip";
        chip.textContent = name;

        const remove = document.createElement("button");
        remove.type = "button";
        remove.className = "chip-remove";
        remove.setAttribute("aria-label", "Usuń składnik");
        remove.textContent = "✕";
        remove.addEventListener("click", function () {
            // Usuwamy składnik z listy i z ekranu
            const i = have.indexOf(name);
            if (i !== -1) have.splice(i, 1);
            chip.remove();
        });

        chip.appendChild(remove);
        chipsList.appendChild(chip);
        input.value = "";
    }

    // Główna funkcja: sprawdza każdy przepis i pokazuje pasujące.
    function search() {
        if (have.length === 0) {
            cards.forEach(function (card) { card.hidden = true; });
            hint.hidden = false;
            hint.textContent = "Dodaj przynajmniej jeden składnik.";
            return;
        }

        let shown = 0;

        cards.forEach(function (card) {
            // Składniki przepisu (rozdzielone "|"), porównujemy małymi literami
            const ingredients = card.getAttribute("data-ingredients").split("|");

            // Wyszukujemy składniki, których użytkownik NIE ma (pomijając podstawowe)
            const missing = ingredients.filter(function (ing) {
                const name = ing.toLowerCase();
                if (basics.indexOf(name) !== -1) return false; // podstawowe pomijamy
                return have.indexOf(name) === -1;              // brakuje, jeśli nie wpisano
            });

            const note = card.querySelector(".match-note");

            if (missing.length === 0) {
                card.hidden = false;
                card.classList.add("can-make");
                note.textContent = "✓ Możesz to zrobić!";
                shown++;
            } else if (missing.length <= maxMissing) {
                card.hidden = false;
                card.classList.remove("can-make");
                note.textContent = "Brakuje: " + missing.join(", ");
                shown++;
            } else {
                card.hidden = true;
            }
        });

        if (shown === 0) {
            hint.hidden = false;
            hint.textContent = "Brak pasujących przepisów. Dodaj więcej składników.";
        } else {
            hint.hidden = true;
        }
    }

    // Kliknięcie "+ Dodaj" dodaje wpisany składnik
    addBtn.addEventListener("click", function () {
        addChip(input.value);
    });

    // Enter w polu też dodaje składnik (a nie wysyła formularza)
    input.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            addChip(input.value);
        }
    });

    // Wysłanie formularza = uruchomienie wyszukiwania
    form.addEventListener("submit", function (event) {
        event.preventDefault();
        search();
    });

    // Przycisk "Wyczyść" — kasuje wszystkie chipy i chowa wyniki
    clearBtn.addEventListener("click", function () {
        have.length = 0;
        chipsList.innerHTML = "";
        cards.forEach(function (card) { card.hidden = true; });
        hint.hidden = false;
        hint.textContent = "Dodaj składniki i kliknij „Szukaj przepisów\".";
    });
});
