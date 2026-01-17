# 🧠 Gra Detektywistyczna — API z LLM NPC

System wspomagania Mistrza Gry oparty na Sztucznej Inteligencji.

---

## 📦 DLA INTEGRACJI

### 1. Instalacja

Skopiuj cały folder `GameAI_Backend` do katalogu z grą

Wymagana struktura:

```
GameAI_Backend/
├── GameAI_Server.exe
└── models/
    └── qwen2.5-3b-instruct-q4_k_m.gguf
```

### 2. Uruchomienie

- **Port:** `8000`
- **Adres:** http://127.0.0.1:8000
- **Dokumentacja:** http://127.0.0.1:8000/docs

### 3. Endpointy

| Endpoint         | Metoda | Opis                                                        |
| -----------------| ------ | ----------------------------------------------------------- |
| /scene/load      | POST   | Ładuje scenę, NPC i przedmioty. Czyści pamięć AI            |
| /npc/chat        | POST   | Wysyła wiadomość gracza i zwraca odpowiedź NPC              |
| /npc/summary     | POST   | Generuje krótki werdykt końcowy na podstawie historii sesji |
| /npc/chat/stream | POST   | Wysyła wiadomość gracza i zwraca odpowiedź NPC w formacie text stream |
---

## 👨‍💻 STREFA DEVELOPERA

### ✅ KROK 1: C++ Build Tools 2022

1. Pobierz: https://visualstudio.microsoft.com/downloads/ → Visual Studio Build Tools 2022
2. Zaznacz: **C++ build tools**
3. Zainstaluj i zrestartuj

### ✅ KROK 2: Python 3.14

1. Pobierz: https://www.python.org/downloads/

### ✅ KROK 3: Projekt

venv (opcjonalny)

### ✅ KROK 4: Zależności

```bash
pip install -r requirements.txt
```

### ✅ KROK 5: Model

1. Pobierz z https://huggingface.co/:
   - **3B:** `qwen2.5-3b-instruct-q3_k_m.gguf` (~2 GB)
   - **7B:** `qwen2.5-7b-instruct-q4_k_m.gguf` (~6 GB, lepszy)

2. Umieść w: `models/`

### ✅ KROK 6: Konfiguracja

Otwórz `app/config.py` i zmień:

```python
MODEL_PATH = "..."
```

na nazwę Twojego pobranego modelu.

### ✅ KROK 7: Uruchomienie

```bash
python runServer.py
```

Test: http://127.0.0.1:8000/docs

### ✅ KROK 8: Build .EXE

```bash
python buildGame.py
```

---
