# 🧠 Gra Detektywistyczna — API z LLM NPC

System wspomagania Mistrza Gry oparty na Sztucznej Inteligencji.

---

## 📦 DLA INTEGRACJI

### 1. Instalacja

Skopiuj cały folder `GameAI_Backend` do katalogu z grą

Wymagana struktura:

```
GameAI_Backend/
├── GameAI_Server.exe       # Serwer API
└── models/
    └── qwen2.5-3b-instruct-q4_k_m.gguf
```

---

### 2. Uruchomienie (z poziomu gry)

- **Port:** `8000`
- **Adres:** http://127.0.0.1:8000

---

### 3. API

Po uruchomieniu serwer nasłuchuje na porcie `8000`.

Dokumentacja Swagger:  
http://127.0.0.1:8000/docs

#### Endpointy

| Endpoint    | Metoda | Opis                                                               |
| ------------| ------ | -------------------------------------------------------------------|
| /scene/load | POST   | Ładuje scenę, NPC i przedmioty. Czyści pamięć AI                   |
| /npc/chat   | POST   | Wysyła wiadomość gracza i zwraca odpowiedź NPC                     |
| /npc/summary| POST   | Generuje krótki werdykt końcowy na podstawie historii całej sesji  |
---

## 👨‍💻 STREFA DEVELOPERA (Rozwój kodu Python)

Dla osób chcących modyfikować serwer lub budować własne wersje `.exe`.

### 🔧 Wymagania

- Python **3.14**
- C++ Build Tools 2022 (wymagane przez `llama-cpp-python`)

---

### 🚀 Instalacja środowiska

##### 1. Zainstaluj zależności:

```bash
pip install -r requirements.txt
```

##### 2. Instalacja modelu (Hugging Face)

Pobierz model z **https://huggingface.co** i umieść pliki w folderze `models/`.

**Rekomendowane warianty:**

- **3B**  
  `qwen2.5-3b-instruct-q3_k_m.gguf`

- **7B (lepsza jakość odpowiedzi)**  
    `qwen2.5-7b-instruct-q4_k_m-00001-of-00002.gguf`
    `qwen2.5-7b-instruct-q4_k_m-00002-of-00002.gguf`
    > `W przypadku modeli wieloczęściowych (*7B*) wszystkie pliki muszą znajdować się w tym samym folderze`.

- Jeśli użyjesz innego modelu lub nazwy pliku, zaktualizuj konfigurację:
    `app/config.py → MODEL_PATH`
---

### ▶️ Uruchomienie lokalne (testy)

Uruchom serwer bez kompilacji:

```bash
python runServer.py
```

---

## 🔨 Budowanie wersji RELEASE (.EXE)

Projekt zawiera automatyczny skrypt, który:

- kompiluje serwer do jednego pliku `.exe`
- dołącza plik modelu
- tworzy gotowy folder `GameAI_Backend`

### Budowanie:

```bash
python buildGame.py
```

---

