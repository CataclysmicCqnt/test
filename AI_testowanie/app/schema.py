from pydantic import BaseModel, Field
from typing import List, Optional


# Npc Schemas

class NPCChatRequest(BaseModel):
    npcName: str = Field()
    userText: str


class NPCChatResponse(BaseModel):
    speech: str


# Scene Schemas

class SceneItem(BaseModel):
    name: str
    description: str
    hints: str


class SceneNPC(BaseModel):
    name: str
    role: str
    description: str


class SceneLoadRequest(BaseModel):
    name: str
    description: str
    npcs: List[SceneNPC]
    items: List[SceneItem]


class SceneLoadResponse(BaseModel):
    description: str
