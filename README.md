# SmaczaPL — portal kulinarny

Projekt na przedmiot **Techniki Internetowe**. Aplikacja webowa do przeglądania
przepisów kulinarnych, napisana w **ASP.NET Core MVC (.NET 8)**.

## Funkcje (warstwa prezentacji / frontend)

- Strona główna z przepisami w formie kart (zdjęcie, tytuł, kategoria, czas).
- Filtrowanie po kategorii oraz wyszukiwarka po tytule — w czystym JavaScript.
- Widok pojedynczego przepisu (składniki, kroki, czas przygotowania).
- Responsywny układ (Bootstrap 5 + CSS Grid/Flexbox + zapytania `@media`).
- Semantyczny HTML (header, nav, main, section, article, footer).

## Uruchomienie

```bash
dotnet run
```

Następnie otwórz adres pokazany w konsoli (np. http://localhost:5080).

## Uwaga

Dane przepisów są **tymczasowo** zapisane na sztywno w
`Controllers/HomeController.cs`. Docelowo zostaną podmienione na bazę danych
(Entity Framework Core) przez część backendową zespołu.
