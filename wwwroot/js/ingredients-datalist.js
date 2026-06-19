// ============================================================
//  SmaczaPL — ładowanie bazy składników do podpowiedzi (datalist)
//  Czysty JavaScript. Pobieramy listę składników z pliku JSON
//  (wwwroot/data/ingredients.json) i wstawiamy ją jako podpowiedzi do
//  pola tekstowego. Backend może później podmienić ten plik na prawdziwą
//  bazę z tysiącami składników (wystarczy ten sam format JSON).
// ============================================================

document.addEventListener("DOMContentLoaded", function () {
    const datalist = document.getElementById("ingredientsDatalist");

    // Jeśli na stronie nie ma listy podpowiedzi, nic nie robimy.
    if (!datalist) {
        return;
    }

    // Pobieramy plik z bazą składników
    fetch("/data/ingredients.json")
        .then(function (response) {
            return response.json();
        })
        .then(function (ingredients) {
            // Dla każdego składnika tworzymy <option> w datalist
            ingredients.forEach(function (name) {
                const option = document.createElement("option");
                option.value = name;
                datalist.appendChild(option);
            });
        })
        .catch(function () {
            // Gdyby plik się nie wczytał — po prostu nie pokazujemy podpowiedzi
            console.warn("Nie udało się wczytać bazy składników.");
        });
});
