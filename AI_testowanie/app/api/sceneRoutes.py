from pathlib import Path
from fastapi import APIRouter, HTTPException

from app.services.gameState import gameState
from app.schema import SceneLoadRequest, SceneLoadResponse
from app.services.aiService import generateStructuredOutput
from app.config import settings


sceneRouter = APIRouter(prefix="/scene")


@sceneRouter.post("/load", response_model=SceneLoadResponse)
def loadScene(data: SceneLoadRequest):

    gameState.setScene(
        name=data.name,
        description=data.description,
        npcs=data.npcs,
        items=data.items
    )
    # return response
    return {"description": data.description}
