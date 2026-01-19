# Gra Detektywistyczna AI - Instrukcja

Gra detektywistyczna zrealizowana w silniku Unity, wykorzystująca zaawansowane modele językowe do generowania dynamicznych dialogów z postaciami niezależnymi (NPC). Gracz wciela się w rolę detektywa, przesłuchuje świadków i analizuje ich wypowiedzi, aby rozwiązać zagadkę kryminalną.

## O projekcie

W przeciwieństwie do tradycyjnych gier z gotowymi drzewkami dialogowymi, tutaj każda rozmowa jest unikalna. Silnik AI odgrywa role świadków, podejrzanych i narratora w czasie rzeczywistym, reagując na pytania gracza kontekstowo.

### Kluczowe funkcjonalności:
- **Dynamiczne dialogi AI**: Możesz zapytać o cokolwiek, NPC odpowie zgodnie ze swoją wiedzą i charakterem.
- **Otwarta struktura śledztwa**: To Ty decydujesz o wyniku dochodzenia.

---

## Instrukcja Uruchomienia

Gra składa się z dwóch części, które muszą działać jednocześnie:
1. **Backend AI** (serwer Python/exe obsługujący model językowy).
2. **Klient Gry** (aplikacja Unity).

### 1. Przygotowanie Backendu (AI)

Serwer AI odpowiada za generowanie odpowiedzi postaci. Znajduje się w folderze `AI_testowanie/`.

**Wymagania:**
- Python 3.10+ (zalecane stworzenie wirtualnego środowiska).
- Zainstalowane zależności (`pip install -r requirements.txt`).
- Pobrany model językowy w formacie `.gguf`.

**Kroki instalacji:**
1. Przejdź do folderu `AI_testowanie/`.
2. Upewnij się, że w folderze `models/` znajduje się model (np. `qwen2.5-7b-instruct-q4_k_m-00001-of-00002.gguf`).
3. Zainstaluj biblioteki (w terminalu):
   ```bash
   cd AI_testowanie
   pip install -r requirements.txt
   ```
   *(Jeśli nie masz pliku requirements.txt, upewnij się że masz zainstalowane: `fastapi`, `uvicorn`, `llama-cpp-python`, `pydantic`)*
4. Uruchom serwer:
   ```bash
   python runServer.py
   ```
   Serwer powinien nasłuchiwać na porcie `8000` (np. `http://127.0.0.1:8000`).

 *Alternatywnie, jeśli posiadasz skompilowaną wersję `GameAI_Server.exe`, po prostu ją uruchom.*

### 2. Uruchomienie Gry (Unity)

1. Otwórz projekt w **Unity Hub** (wskazując folder `Gra_detektywistyczna`).
2. Upewnij się, że serwer AI działa w tle (krok powyżej).
3. Otwórz scenę startową w `Assets/Scenes`.
4. Naciśnij **Play** w edytorze lub zbuduj projekt (Build & Run).

---

### System Dialogów
Gdy rozpoczniesz rozmowę z NPC:
1. Pojawi się okno dialogowe.
2. Wpisz swoje pytanie lub wypowiedź w polu tekstowym.
3. Zatwierdź enterem.
4. Poczekaj chwilę na wygenerowanie odpowiedzi przez AI.

---

## Struktura Projektu

- **`AI_testowanie/`**: Kod źródłowy serwera AI (Python).
  - `app/`: Logika aplikacji backendowej (FastAPI).
  - `models/`: Miejsce na pliki modeli `.gguf`.
- **`DialogueEngine/`**: Biblioteki C# wspierające komunikację z API i obsługę dialogów.
- **`Gra_detektywistyczna/`**: Główny projekt Unity.
  - `Assets/Scripts`: Skrypty sterujące rozgrywką.
  - `Assets/InputSystem_Actions.inputactions`: Konfiguracja sterowania (Unity Input System).

---

## Rozwiązywanie problemów

- **Brak odpowiedzi od NPC?**
  - Sprawdź, czy konsola serwera Python nie wyświetla błędów.
  - Upewnij się, że gra próbuje łączyć się z odpowiednim adresem (domyślnie `localhost:8000`).
- **Problemy z `llama-cpp-python`?**
  - Instalacja tej biblioteki często wymaga **Visual Studio C++ Build Tools**. Upewnij się, że masz je zainstalowane, jeśli budujesz środowisko od zera.
