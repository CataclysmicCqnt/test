import json
import multiprocessing
import os
from typing import Any, Dict, Iterator, Type
import requests
from pydantic import BaseModel
from app.config import settings


try:
    from llama_cpp import Llama
except ImportError:
    print("llama-cpp-python not installed - pip install llama-cpp-python")
    Llama = None

_llmInstance = None


def getLlm():

    global _llmInstance
    if _llmInstance is None:
        if Llama is None:
            raise ImportError("llama-cpp-python missing")

        modelPathStr = str(settings.MODEL_PATH)

        if not os.path.exists(modelPathStr):
            raise FileNotFoundError(f"AI Model not found at: {modelPathStr}")

        print(f"Loading AI Model: {settings.MODEL_PATH} ...")
        threads = max(1, multiprocessing.cpu_count() - 2)

        _llmInstance = Llama(
            model_path=modelPathStr,
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

    llm = getLlm()

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
            temperature=0.4,
            max_tokens=512
        )

        content = output["choices"][0]["message"]["content"]

        parsedJson = json.loads(content)

        validated = responseModel.model_validate(parsedJson)

        return validated.model_dump()

    except Exception as e:
        print(f"AI error: {e}")
        return {"speech": "...", "error": str(e)}


def generateStream(systemPrompt: str, userPrompt: str) -> Iterator[str]:

    llm = getLlm()

    messages = [
        {"role": "system", "content": systemPrompt},
        {"role": "user", "content": userPrompt}
    ]

    stream = llm.create_chat_completion(
        messages=messages,
        temperature=0.6,  
        max_tokens=512,
        stream=True      
    )

    for chunk in stream:
        delta = chunk['choices'][0]['delta']
        content = delta.get('content', "")

        if content:
            jsonData = json.dumps({"token": content}, ensure_ascii=False)
            yield f"data: {jsonData}\n\n"
