

from typing import Any, Dict, List

from pydantic import BaseModel

from app.schema import SceneItem, SceneNPC


class ChatMessage(BaseModel):
    speaker: str
    text: str


class GameStateManager:
    def __init__(self):
        self.currentSceneName: str = ""
        self.currentSceneDescription: str = ""

        self.currentNpcs: List[SceneNPC] = []
        self.currentItems: List[SceneItem] = []

        self.chatHistory: List[ChatMessage] = []
        self.globalClues: List[str] = []  # мб ост

    def setScene(self, name: str, description: str, npcs: List[SceneNPC], items: List[SceneItem]):
        self.currentSceneName = name
        self.currentSceneDescription = description
        self.currentNpcs = npcs
        self.currentItems = items

        self.chatHistory = []

    def addMessage(self, speaker: str, text: str):
        self.chatHistory.append(ChatMessage(speaker=speaker, text=text))

    def getSceneTranscript(self) -> str:
        transcript = ""
        for msg in self.chatHistory[-100:]:
            transcript += f"{msg.speaker}: {msg.text}\n"

        return transcript

    def isSceneLoaded(self) -> bool:
        return bool(self.currentSceneName)


gameState = GameStateManager()
