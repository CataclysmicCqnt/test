from pathlib import Path
from fastapi import APIRouter, HTTPException

from app.services.gameState import gameState
from app.schema import NPCChatRequest, NPCChatResponse
from app.services.aiService import generateStructuredOutput
from app.config import settings


npcRouter = APIRouter(prefix="/npc")


@npcRouter.post("/chat", response_model=NPCChatResponse)
def chatWithNpc(data: NPCChatRequest):
    @npcRouter.post("/summary", response_model=NPCChatResponse)
def getGameSummary():

    transcript = gameState.getSceneTranscript()

    if not transcript:
        return {"speech": "Nie podjęto żadnych znaczących działań, które można by ocenić."}

    system_prompt = """
    Jesteś sędzią i narratorem gry detektywistycznej. 
    Przeanalizuj historię śledztwa i wydaj krótki, surowy, ale sprawiedliwy werdykt (maksymalnie 3 zdania).
    Oceń logikę gracza, jego podejście do świadków i czy udało mu się odkryć prawdę.
    Twoja odpowiedź musi być w formacie JSON: {"speech": "treść werdyktu"}.
    Używaj języka polskiego.
    """

    user_prompt = f"Oto historia śledztwa:\n{transcript}\n\nWydaj ostateczny werdykt na temat działań Detektywa."

    response = generateStructuredOutput(
        system_prompt,
        user_prompt,
        NPCChatResponse
    )

    return response

    if not gameState.isSceneLoaded():
        raise HTTPException(
            status_code=400,
            detail="Błąd: Scena nie została załadowana. Użyj /scene/load."
        )
    transcript = gameState.getSceneTranscript()
    gameState.addMessage(speaker="Player", text=data.userText)

    current_npc_desc = ""
    other_people_list = ""
    found_npc = False

    for npc in gameState.currentNpcs:
        if npc.name == data.npcName:
            current_npc_desc = f"Rola: {npc.role}. Opis: {npc.description}"
            found_npc = True
        else:
            other_people_list += f"- {npc.name} ({npc.role})\n"

    if not found_npc:
        print(f"[WARNING] NPC '{data.npcName}' nie znaleziono w scenie!")
        current_npc_desc = "Rola: Nieznana. Opis: Brak danych."

    if not other_people_list:
        other_people_list = "Nikogo innego tu nie ma."

    items_list = ""
    for item in gameState.currentItems:
        items_list += f"- {item.name}: {item.description} (Wiedza/Wskazówka: {item.hints})\n"

    if not items_list:
        items_list = "Brak przedmiotów."

    systemPrompt = f"""
   Jesteś postacią w grze detektywistycznej.
    Twoje imię: {data.npcName}
    Twoje dane: {current_npc_desc}
    
    KIM JEST TWÓJ ROZMÓWCA:
    Rozmawiasz z Detektywem, który prowadzi śledztwo w sprawie zbrodni.
    Traktuj go odpowiednio do swojej roli (np. jeśli jesteś podejrzany - bądź ostrożny, jeśli świadek - pomocny).
    
    Lokalizacja: {gameState.currentSceneDescription}
    
    Kto jest obok:
    {other_people_list}
    
    Widoczne przedmioty (i twoje myśli o nich):
    {items_list}

    Historia rozmowy:
    {transcript}
   ZASADY (BARDZO WAŻNE):
    1. Jesteś żywym człowiekiem w tej sytuacji, a nie bazą danych. Możesz improwizować drobne szczegóły (np. nastrój, odczucia), aby budować klimat, o ile nie przeczy to faktom.
    2. NIGDY nie tłumacz się ze swojej roli (nie pisz zdań typu "To jest zgodne z moją rolą" ani "Jako AI"). Po prostu graj.
    3. Korzystaj z sekcji "OTOCZENIE". Jeśli Detektyw pyta "kto tu jest?", musisz wymienić osoby z listy powyżej.
    4. Jeśli gracz pyta o przedmiot, użyj "Wskazówki" (Hints) z opisu, aby subtelnie naprowadzić Detektywa.
    5. Mów krótko, naturalnie i tylko po polsku bez innych jenzykow.
    6. Zwracaj WYŁĄCZNIE czysty JSON (klucz "speech").
    """

    userPrompt = f"Gracz pyta: \"{data.userText}\". Odpowiedz jako {data.npcName}."

    response = generateStructuredOutput(
        systemPrompt,
        userPrompt,
        NPCChatResponse
    )

    if "error" in response:
        raise HTTPException(status_code=502, detail=response)

    gameState.addMessage(speaker=data.npcName, text=response['speech'])
    return response
