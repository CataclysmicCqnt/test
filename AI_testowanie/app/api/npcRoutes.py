import json
from pathlib import Path
from fastapi import APIRouter, HTTPException
from fastapi.responses import StreamingResponse

from app.services.gameState import gameState
from app.schema import NPCChatRequest, NPCChatResponse, VerdictRequest, VerdictResponse
from app.services.aiService import generateStream, generateStructuredOutput


npcRouter = APIRouter(prefix="/npc")


@npcRouter.post("/chat", response_model=NPCChatResponse)
def chatWithNpc(data: NPCChatRequest):

    if not gameState.isSceneLoaded():
        raise HTTPException(
            status_code=400,
            detail="Błąd: Scena nie została załadowana. Użyj /scene/load."
        )
    transcript = gameState.getSceneTranscript()
    gameState.addMessage(speaker="Player", text=data.userText)

    currentNpcDesc = ""
    otherPeopleList = ""
    foundNpc = False

    for npc in gameState.currentNpcs:
        if npc.name == data.npcName:
            currentNpcDesc = f"Rola: {npc.role}. Opis: {npc.description}"
            foundNpc = True
        else:
            otherPeopleList += f"- {npc.name} ({npc.role})\n"

    if not foundNpc:
        print(f"[WARNING] NPC '{data.npcName}' nie znaleziono w scenie!")
        currentNpcDesc = "Rola: Nieznana. Opis: Brak danych."

    if not otherPeopleList:
        otherPeopleList = "Nikogo innego tu nie ma."

    itemsList = ""
    for item in gameState.currentItems:
        itemsList += f"- {item.name}: {item.description} (Wiedza/Wskazówka: {item.hints})\n"

    if not itemsList:
        itemsList = "Brak przedmiotów."

    systemPrompt = f"""
   Jesteś postacią w grze detektywistycznej.
    Twoje imię: {data.npcName}
    Twoje dane: {currentNpcDesc}

    KIM JEST TWÓJ ROZMÓWCA:
    Rozmawiasz z Detektywem, który prowadzi śledztwo w sprawie zbrodni.
    Traktuj go odpowiednio do swojej roli (np. jeśli jesteś podejrzany - bądź ostrożny, jeśli świadek - pomocny).

    Lokalizacja: {gameState.currentSceneDescription}

    Kto jest obok:
    {otherPeopleList}

    Widoczne przedmioty (i twoje myśli o nich):
    {itemsList}

    Historia rozmowy:
    {transcript}
   ZASADY (BARDZO WAŻNE):
    1. Jesteś żywym człowiekiem w tej sytuacji, a nie bazą danych. Możesz improwizować drobne szczegóły (np. nastrój, odczucia), aby budować klimat, o ile nie przeczy to faktom.
    2. NIGDY nie tłumacz się ze swojej roli (nie pisz zdań typu "To jest zgodne z moją rolą" ani "Jako AI"). Po prostu graj.
    3. Korzystaj z sekcji "OTOCZENIE". Jeśli Detektyw pyta "kto tu jest?", musisz wymienić osoby z listy powyżej.
    4. Jeśli gracz pyta o przedmiot, użyj "Wskazówki" (Hints) z opisu, aby subtelnie naprowadzić Detektywa.
    5. Mów krótko, naturalnie i tylko po polsku bez innych języków.
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


@npcRouter.post("/chat/stream")
def chatWithNpcStream(data: NPCChatRequest):

    if not gameState.isSceneLoaded():
        raise HTTPException(
            status_code=400,
            detail="Błąd: Scena nie została załadowana. Użyj /scene/load."
        )

    gameState.addMessage(speaker="Player", text=data.userText)
    transcript = gameState.getSceneTranscript()

    currentNpcDesc = ""
    otherPeopleList = ""
    foundNpc = False

    for npc in gameState.currentNpcs:
        if npc.name == data.npcName:
            currentNpcDesc = f"Rola: {npc.role}. Opis: {npc.description}"
            foundNpc = True
        else:
            otherPeopleList += f"- {npc.name} ({npc.role})\n"

    if not foundNpc:
        currentNpcDesc = "Rola: Nieznana. Opis: Brak danych."

    if not otherPeopleList:
        otherPeopleList = "Nikogo innego tu nie ma."

    itemsList = ""
    for item in gameState.currentItems:
        itemsList += f"- {item.name}: {item.description} (Wiedza/Wskazówka: {item.hints})\n"

    if not itemsList:
        itemsList = "Brak przedmiotów."

    systemPrompt = f"""
    Jesteś postacią w grze detektywistycznej.
    Twoje imię: {data.npcName}
    Twoje dane: {currentNpcDesc}

    KIM JEST TWÓJ ROZMÓWCA:
    Rozmawiasz z Detektywem, który prowadzi śledztwo w sprawie zbrodni.
    Traktuj go odpowiednio do swojej roli (np. jeśli jesteś podejrzany - bądź ostrożny, jeśli świadek - pomocny).

    Lokalizacja: {gameState.currentSceneDescription}

    Kto jest obok:
    {otherPeopleList}

    Widoczne przedmioty (i twoje myśli o nich):
    {itemsList}

    Historia rozmowy:
    {transcript}

    ZASADY (BARDZO WAŻNE):
    1. Jesteś żywym człowiekiem, improwizuj.
    2. NIGDY nie pisz, że jesteś AI.
    3. Mów krótko, naturalnie i tylko po polsku.
    4. WAŻNE: NIE UŻYWAJ FORMATU JSON. Odpowiadaj zwykłym tekstem, tak jakbyś mówił.
    """

    userPrompt = f"Gracz pyta: \"{data.userText}\". Odpowiedz jako {data.npcName}."

    def response_generator():
        fullResponseText = ""

        streamIterator = generateStream(systemPrompt, userPrompt)

        for chunk in streamIterator:
            yield chunk

            try:
                if chunk.startswith("data: "):
                    jsonStr = chunk.replace("data: ", "").strip()
                    dataObj = json.loads(jsonStr)

                    if "token" in dataObj:
                        fullResponseText += dataObj["token"]
            except Exception:
                pass

        if fullResponseText:
            gameState.addMessage(speaker=data.npcName, text=fullResponseText)

    return StreamingResponse(
        response_generator(),
        media_type="text/event-stream"
    )


@npcRouter.post("/verdict", response_model=VerdictResponse)
def getGameVerdict(request: VerdictRequest):

    selectedEnding = None

    for ending in request.endings:
        if ending.accusedName.lower() in request.accusedName.lower() or \
                request.accusedName.lower() in ending.accusedName.lower():
            selectedEnding = ending
            break

    if not selectedEnding:
        return {
            "speech": f"Nie znaleziono zakończenia dla osoby: {request.accusedName}. Sprawdź dane.",
            "isPlayerRight": False
        }

    systemPrompt = f"""
    Jesteś sędzią i narratorem finału gry kryminalnej.
    
    DANE ZAKOŃCZENIA:
    Oskarżony: {selectedEnding.accusedName}
    Czy to poprawny sprawca? {'TAK (WYGRANA)' if selectedEnding.isMurderer else 'NIE (PRZEGRANA)'}
    Opis sytuacji: {selectedEnding.description}
    
    ZADANIE:
    Napisz krótkie podsumowanie dla gracza (maksymalnie 3 zdania).
    Opisz konsekwencje wyboru na podstawie "Opisu sytuacji".
    Bądź surowy i klimatyczny.
    
    Odpowiedz JSONem: {{"speech": "Twoje podsumowanie...", "isPlayerRight": {str(selectedEnding.isMurderer).lower()}}}
    """

    userPrompt = "Wydaj werdykt."

    try:
        response = generateStructuredOutput(
            systemPrompt,
            userPrompt,
            VerdictResponse
        )

        response['isPlayerRight'] = selectedEnding.isMurderer
        return response

    except Exception:

        return {
            "speech": selectedEnding.description,
            "isPlayerRight": selectedEnding.isMurderer
        }
