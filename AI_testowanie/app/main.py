from contextlib import asynccontextmanager
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.api.npcRoutes import npcRouter
from app.api.sceneRoutes import sceneRouter
from app.config import settings

from app.services.aiService import getLlm


@asynccontextmanager
async def lifespan(app: FastAPI):

    try:

        model_instance = getLlm()
        if model_instance:
            print("AI Model loaded successfully")
        else:
            print("Model instance is None")

    except Exception as e:
        print(f"Failed to load AI Model: {e}")

    yield

    print("Shutting down ")


app = FastAPI(lifespan=lifespan)


app.include_router(npcRouter)
app.include_router(sceneRouter)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.get("/health")
def health():
    return {
        "status": "online",
        "model_path": str(settings.MODEL_PATH),
    }
