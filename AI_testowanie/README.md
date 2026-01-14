# 🧠 Gra Detektywistyczna — API z LLM NPC

System wspomagania Mistrza Gry oparty na Sztucznej Inteligencji (Ollama + Python FastAPI).

---

## 🚀 SZYBKI START (Dla Zespołu)

Dla wygody przygotowaliśmy automatyczny starter. Nie musisz wpisywać komend ręcznie.

### 1. Uruchomienie
1. Wejdź do folderu z grą.
2. Kliknij dwukrotnie plik **`START_GAME.bat`**.
3. W czarnym oknie wybierz jedną z opcji:
    * **1. PIERWSZE URUCHOMIENIE** – Wybierz tylko za pierwszym razem na nowym komputerze. Skrypt sam pobierze biblioteki Python i zainstaluje model.
    * **2. START** – Wybierz, jeśli masz już wszystko zainstalowane i chcesz po prostu grać.

### 2. Menu w konsoli
Po uruchomieniu program zapyta Cię o konfigurację sesji:

**Pytanie 1: Czy otworzyć okno gry?**
* **TAK (1)** – Gra otworzy się sama w domyślnej przeglądarce.
* **NIE (2)** – Uruchomi się tylko serwer w tle (przydatne przy testach lub ręcznym otwieraniu).

**Pytanie 2: Jaki tryb AI?**
* **MOCK MODE (1)** – **Bez AI.** Gra działa błyskawicznie, postacie odpowiadają gotowymi tekstami testowymi. Nie wymaga mocnego komputera ani włączonej Ollamy.
* **LIVE AI (2)** – **Pełne AI.** Gra łączy się z Ollamą. Postacie generują odpowiedzi na żywo. Wymaga włączonej Ollamy i modelu `gpt-oss:20b`.

---

## 👨‍💻 STREFA DEVELOPERA (Informacje Techniczne)

Poniższe sekcje są przydatne przy ręcznej konfiguracji, debugowaniu lub budowaniu pliku .exe.

### 🔧 Ręczna instalacja i uruchomienie
Jeśli nie chcesz używać `START_GAME.bat`, wykonaj te kroki w terminalu:

1. **Instalacja zależności:**

   ```bash
   pip install -r requirements.txt
   ```

2. **Pobranie i stworzenie modelu (wymagane tylko raz):**

   ```bash
   ollama pull gpt-oss:20b
   ollama create game-npc-model -f Modelfile
   ```

3. **Uruchomienie serwera:**
   ```bash
   python runApp.py
   ```

---

## 📜 Punkty Końcowe API

| Endpoint      | Metoda | Opis                                               |
| ------------- | ------ | -------------------------------------------------- |
| `/npc/chat`   | POST   | Generuje odpowiedź od NPC (mowa, akcja, intencja). |
| `/scene/load` | POST   | Generuje nową scenę (opis, NPC, przedmioty).       |
| `/health`     | GET    | Sprawdza stan serwera.                             |

---

### 2️⃣ Kompilacja do pliku .EXE

```bash
pyinstaller --noconfirm --onefile --windowed --name "AI_Server" --hidden-import=uvicorn --add-data "app;app" --add-data "UI_DEV;UI_DEV" runApp.py
```