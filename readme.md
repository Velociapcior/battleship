
# Battleship

  

## Technologia

- .NET 5

- Blazor WebAssembly

  

## Wymagania

- ASP.NET core runtime 5.0 (najlepsza opcja to .NET SDK) [Pobierz](https://dotnet.microsoft.com/download/dotnet/5.0)

  

## Szybkie uruchomienie

1. Otwórz konsole systemu

  

2. Przejdź do folderu projektu, w którym znajduje się battleship.csproj

  

3. W konsoli wpisz `dotnet restore` i naciśnij enter

  

4. Po wykonaniu polecennia wpisz `dotnet run` i naciśnij enter

  

5. Gdy na konsoli pojawi się napis:

```

info: Microsoft.Hosting.Lifetime[0]

Application started. Press Ctrl+C to shut down.

```

otwórz dowolną przeglądarkę i w polu adresu wpisz http://localhost:5000/

(upewnij się, że przeglądarka obsługuje standard WebAssembly [Lista kompatybilnych przeglądarek](https://caniuse.com/wasm))

## Opis działania programu

### Opis typów

-  `Cell` - najmniejsza jednostka, przechowująca informacje o tym jaki jest stan danego pola oraz jaki statek jest na nim umieszczony

-  `CellState` - enum zawierający możliwe stany komórki

-  `ShipType` - enum zawierający typy statków

-  `Direction` - enum zawierający możlie kierunki ustawienia statków (poiomy, pionowy)

-  `GameField` - klasa opisująca pole gry. Zawiera tablicę dwuwymiarową komórek oraz metody dostępu do tej tablicy (`SetCell`, `GetCell`) oraz metodę `CheckIfShipCanBePlaced`, która sprawdza czy statek może zostać ustawiony na danych współżędnych x, y o danej długości i kierunku.

-  `CheckIfShipCanBePlaced` - algorytm najpierw sprawdza, czy dla odpowiedniego kierunku komórka przed i po statku są wolne, a następnie sprawdza, czy w komórkach, które ma zająć statek jest już inny statek. Jeżeli nie to zwraca `true`;

-  `Move` - klasa, w której zawarte są informacje o ruchu, czyli koordynaty x, y oraz komórka, której dotyczył (wraz z aktualnym stanem i informacją co w niej było)

-  `Player` - klasa opisująca gracza. Zawiera pole gry danego gracza, listę wykonanych do tej pory ruchów oraz liczbę pozostałych punktów

-  `IGameService` - interfejs opisjuący metody do zmiany stanu gry 
Dostępne metody:
	- `Fire` - oddanie strzału przez gracza, w stronę drugiego gracza
	- `ClearGameField` - wyczyszczenie pola gry
	- `PlaceShips` - ustaw podane statki dla podanego pola gry

- `GameService` - klasa implementująca interfejes `IGameService`.  
Opis implementacji:
	- `Fire` - pobierana jest komórka, która jest atakowana, a następnie sprawdzany jest jej stan, jeśli jest pusta, to jej stan jest zmieniany na Pudło i do listy ruchów jest dodawany nowy ruch.
	- `ClearGameField` - podwójna pętla for ustawia dla wszystkich komórek podanego pola gry stan pusty oraz typ statku `None`
	- `PlaceShips` - dla każdego statku ze słownika Ships generowane losowo są współżędne oraz kierunek, które następnie dla podanego pola gry są sprawdzane, czy można na nich umieścić statek, jeśli nie, wybierane są nowe wspórzędne, jeśli tak to statek jest dodawany do pola gry

- `Index.razor` i `Index.razor.cs` - komponent blazor odpowiedzialny za przebieg i sterowanie grą.  
Właściwości i pola:
	- `_areShipsPlaced` - pole zawierające informację, czy statki zostały już ustawione dla obu graczy
	- `RedPlayer` - informację o graczu czerwonym
	- `BluePlayer` - informację o graczu niebieskim
	- `Ships` - słownik, w którym kluczem jest typ statku, a wartością ilość statków danego typu do umieszczenia na polu gry
	- `FieldWidth` - szerokość pola
	- `FieldHeight` - wysokość pola
	- `MaxPoints` - Maksymalna liczba punktów przypadająca na gracza, odpowiadająca sumie wszystkich statków (typ statku * ilość statków)

Metody zawarte w `Index.razor.cs`:
- `OnInitialized` - metoda inicjalizująca właściwości i pola w niej nasępuje decyzja o rozmiarze pola gry oraz typach i ilości statków
- `PlaceShips` - Metoda wykonywana po kliknięciu w przycisk "Place ships". Czyści ona pola gry dla obydwu graczy oraz ustawia statki
- `StartGame` - metoda rozpoczynająca grę, w pętli wykonująca kolejne ruchy graczy, aż do momentu, kiedy któremuś graczowi skończą się punkty. Uruchamiana przyciskiem "Start game". Zawiera zabezpieczenie przed uruchomieniem gry bez ustawionych statków, jeżeli nie miało ono miejsca, to zostanie uruchomiona metoda `PlaceShips`
- `TakeShot` - metoda wywoływana w pętli w `StartGame`, lub przez kliknięcie któregoś z przycisków strzału gracza (czerwony, lub niebieski przycisk). Na podstawie podanego parametru, podejmowana jest decyzja, który gracz strzal, a który jest celem. W metodzie na podstawie poprzednich ruchów gracza podejmowana jest decyzja czy strzelać w odpowiedniej kolejności, czy wybrać losowe koordynaty. Po wybraniu koordynatów, sprawdzane jest, czy gracz oddał już dany, jeżeli tak, to wybierane są kolejne współrzędne, jeżeli nie, to oddawany jest strzał i zapisywany jest wynik strzału.
	- **Działanie AI** - oczywiście o żadnym AI nie ma tutaj mowy, jest to prosty algorytm, który tworzy listę 4 dostępnych ruchów, otaczających ostatni udany strzał. Następnie wybiera ruchy, które nie są po za granicami pola gry, a następnie ogranicza dostępne ruchy do tych, które nie zostały już wykonane. Jeżli po tych zabiegach lista jest pusta, to wybierane są losowe współrzędne. Jeżeli jednak lista nie jest pusta, to wybierany jest pierwszy element.

## Podjęte decyzje implementacyjne
- *Czemu blazor?*  
Po to żeby zademonstrować możliwości blazora, jako narzędzia do tworzenia bogatych interfejsów użytkownika, ale również moich umiejętności w tejże technologii. Możnaby zrobić kolejną nudną appke konsolową, ale po co?
- *Czemu tablica dwówymiarowa, zamiast np. listy list, bądź listy obiektów ze współrzędnymi?*  
Ponieważ wybranie tablicy dwówymiarowej wydawało się bardziej naturalnym rozwiązaniem dla gry w statki
- *Dlaczego typ Cell ma tylko dwie właściwości typów enum*  
Ponownie chodzi o prostotę. Wydaje mi się, żeby wykonać implementację gry statki nie trzeba tworzyć skomplikowanych obiektów opisującyh model gry.
- *Po co interfejs `IGameService?`*  
Teoretycznie mogłoby go nie być. Jest to kolejna warstwa abstrakcji, oddzielająca "logikę biznesową" od sterowania polem gry. Klasa `GameField` jednakże nie wydawała mi się odpowiednia na umieszczenie tam funkcjonalności `IGameService`. W teorii wszystkie rzeczy, które robi `IGameService`, możnaby umieścić w `Index.razor.cs`. Ale wtedy by się nam trochę nagmatwało w tej klasie.
- *Czemu statki są reprezentowane przez **Słownik**?*  
Ta strutktura wydawała mi się idealna w tej sytuacji, ponieważ ponownie chodzi o prostotę. Żeby opisać jakie statki uczestniczą w grze potrzebujemy tylko dwóch wartości. Ich długości oraz ilości. Długość jest wartością unikalną. Nie będzie dwóch rodzajów statków o tej samej długości, gdyż wtedy byłyby tym samym statkiem. W pierwszej implementacji ten słownik miał w kluczu oraz w wartości `int`, ale doszedłem do wniosku, że przydałoby się jakieś rozróżnienie, stąd powstał enum `ShipType`.
- *Czemu nie ma zliczania zatopionych statków?*  
Warunkiem wygranej każdej gry Battleship jest trafienie wszystkich kratek przeciwnika. Można je uznać jako punkty życia. Na potrzeby tej implementacji zostały pominięte kwestie pojedynczych statków, gdyż uznałem je za zbędne wodotryski.

### Co można usprawnić
- Można podjąć próbę refaktoryzacji niektórych pętli, któe zostały napisane w taki a nie inny sposób ze względu na chęć dostarczenia MVP w jak najszybszym czasie
- Możnaby poprawić działanie algorytmu "AI" - w tym momencie algorytm nie bierze pod uwagę jakiego typu statek jest atakowany, kierunku, w którym statek jest ustawiony oraz ile kratek zostało temu statkowi do zatonięcia. Na moment implementacji, aktualne rozwiązanie wydawało mi się *"good enough"* jak na pierwszą wersję aplikacji.
- Podczas rozmieszczania statków zdarzają się błędy gdy statki stykają się burtami. Niestety brakło czasu na zgłębienie tych błędów, a nie wpływały one krytycznie na działanie programu (W oryginalnej wersji battleship statki mogą się stykać)
