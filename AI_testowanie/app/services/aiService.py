import json
import multiprocessing
import os
from typing import Any, Dict, Type
import requests
from pydantic import BaseModel
from app.config import settings


try:
    from llama_cpp import Llama
except ImportError:
    print("llama-cpp-python not installed - pip install llama-cpp-python")
    Llama = None

_llmInstance = None


def get_llm():

    global _llmInstance
    if _llmInstance is None:
        if Llama is None:
            raise ImportError("llama-cpp-python missing")

        model_path_str = str(settings.MODEL_PATH)

        if not os.path.exists(model_path_str):
            raise FileNotFoundError(f"AI Model not found at: {model_path_str}")

        print(f"Loading AI Model: {settings.MODEL_PATH} ...")
        threads = max(1, multiprocessing.cpu_count() - 2)

        _llmInstance = Llama(
            model_path=model_path_str,
            n_ctx=settings.N_CTX,
            n_gpu_layers=settings.N_GPU_LAYERS,
            n_threads=threads,
            n_batch=512,
            verbose=False
        )
        print("loaded successfully!")

    return _llmInstance


def generateStructuredOutput(
    systemPrompt: str,
    userPrompt: str,
    responseModel: Type[BaseModel]
) -> Dict[str, Any]:

    llm = get_llm()

    jsonSchema = json.dumps(
        responseModel.model_json_schema(), ensure_ascii=False)

    strict_system_prompt = f"""{systemPrompt}

    WYMAGANY FORMAT ODPOWIEDZI (JSON SCHEMA):
    {jsonSchema}
    
    Odpowiadaj TYLKO czystym JSON zgodnym z tą schemą.
    """
    messages = [
        {"role": "system", "content": strict_system_prompt},
        {"role": "user", "content": userPrompt}
    ]

    try:
        output = llm.create_chat_completion(
            messages=messages,
            response_format={"type": "json_object"},
            temperature=0.2,
            max_tokens=256
        )

        content = output["choices"][0]["message"]["content"]

        parsedJson = json.loads(content)

        validated = responseModel.model_validate(parsedJson)

        return validated.model_dump()

    except Exception as e:
        print(f"AI error: {e}")
        return {"speech": "...", "error": str(e)}
