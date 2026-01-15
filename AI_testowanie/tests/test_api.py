import pytest
from fastapi.testclient import TestClient
from unittest.mock import patch, MagicMock
from fastapi import FastAPI
import sys
import os

# --- KONFIGURACJA ŚRODOWISKA ---
# Dodajemy folder nadrzędny do ścieżki systemowej, aby Python widział moduły z folderu 'app'.
# Bez tego testy nie widziałyby Twojego kodu aplikacji.
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

from app.api.npcRoutes import npcRouter
from app.api.sceneRoutes import sceneRouter
from app.config import settings

# Tworzymy "testową" instancję aplikacji FastAPI.
# Dołączamy routery, które chcemy przetestować.
app = FastAPI()
app.include_router(npcRouter)
app.include_router(sceneRouter)

# TestClient pozwala wysyłać żądania HTTP do naszej aplikacji bez uruchamiania serwera (uvicorn).
client = TestClient(app)


# --- FIXTURES (Elementy pomocnicze) ---

@pytest.fixture
def mock_files():
    """
    CEL: Odizolowanie testu od systemu plików.
    Zamiast szukać prawdziwych plików 'lore.txt' czy 'persona.txt', 
    ten mock sprawia, że każda próba odczytu pliku zwróci "DUMMY CONTENT FILE".
    Dzięki temu testy działają nawet jeśli brakuje plików z danymi.
    """
    with patch("pathlib.Path.read_text") as mock_read:
        mock_read.return_value = "DUMMY CONTENT FILE"
        yield mock_read

@pytest.fixture
def mock_ollama():
    """
    Coś tam, coś tam to jest tu, aby nie było ai (ollama) podłączonego do testów poniżej
    """
    with patch("app.api.npcRoutes.generateStructuredOutput") as mock_npc, \
         patch("app.api.sceneRoutes.generateStructuredOutput") as mock_scene:
        
        yield {"npc": mock_npc, "scene": mock_scene}

def test_npc_chat_success(mock_files, mock_ollama):
    """
    Sprawdza czy API poprawnie odbiera odpowiedź od AI i ją przekazuje
    """
    original_mock_setting = settings.USE_MOCK
    settings.USE_MOCK = False
    
    try:
        mock_ollama["npc"].return_value = {
            "speech": "Słucham cię uważnie.",
            "action": "patrzy w oczy",
            "intent": "listening"
        }

        payload = {
            "sceneDescription": "Biuro detektywa",
            "userText": "Masz chwilę?",
            "npcName": "Partner"
        }

        response = client.post("/npc/chat", json=payload)

        assert response.status_code == 200
        data = response.json()
        assert data["speech"] == "Słucham cię uważnie."
        assert data["intent"] == "listening"
        
        mock_ollama["npc"].assert_called_once()

    finally:
        settings.USE_MOCK = original_mock_setting

def test_npc_chat_mock_mode():
    """
    Sprawdza tryb deweloperski Mock, więc powinna być sztywna odpowiedź w tym trybie bez ai
    """
    original_mock_setting = settings.USE_MOCK
    settings.USE_MOCK = True # Włączamy tryb Mock
    
    try:
        payload = {
            "sceneDescription": "Dowolna",
            "userText": "Cokolwiek",
            "npcName": "Ktokolwiek"
        }
        
        with patch("app.api.npcRoutes.generateStructuredOutput") as mock_ai:
            response = client.post("/npc/chat", json=payload)
            
            assert response.status_code == 200
            data = response.json()
            assert data["speech"] == "Jesteś w trybie testowym"
            
            mock_ai.assert_not_called()

    finally:
        settings.USE_MOCK = original_mock_setting

def test_npc_chat_validation_error():
    """
    Sprawdza czy API odrzuci żądanie, któremu brakuje wymaganego pola 'npcName'
    """
    payload = {
        "sceneDescription": "Las",
        "userText": "Hej"
    }
    response = client.post("/npc/chat", json=payload)
    
    assert response.status_code == 422


def test_scene_load_success(mock_files, mock_ollama):
    """
    Sprawdza czy endpoint /scene/load poprawnie przetwarza odpowiedź z generatora scen
    """
    original_mock_setting = settings.USE_MOCK
    settings.USE_MOCK = False

    try:
        mock_ollama["scene"].return_value = {
            "extendedDescription": "Mroczny zaułek spowity mgłą. Słychać kapanie wody."
        }

        payload = {
            "name": "Zaułek",
            "description": "Ciemno i zimno",
            "npcs": [],
            "items": []
        }

        response = client.post("/scene/load", json=payload)

        assert response.status_code == 200
        data = response.json()
        assert "Mroczny zaułek" in data["extendedDescription"]
        
        mock_ollama["scene"].assert_called_once()

    finally:
        settings.USE_MOCK = original_mock_setting

def test_scene_load_ai_error(mock_files, mock_ollama):
    """
    Test tego co się stanie, jeśli Ollama zwróci błąd
    """
    original_mock_setting = settings.USE_MOCK
    settings.USE_MOCK = False

    try:
        mock_ollama["scene"].return_value = {"error": "Model overload"}

        payload = {
            "name": "Test",
            "description": "Test",
            "npcs": [],
            "items": []
        }

        response = client.post("/scene/load", json=payload)

        assert response.status_code == 502
        assert "Model overload" in response.json()["detail"]["error"]

    finally:
        settings.USE_MOCK = original_mock_setting