from pathlib import Path
import sys
import os
from pathlib import Path


class Settings:
    def __init__(self):
        if getattr(sys, 'frozen', False):
            self.BASE_DIR = Path(sys.executable).parent
        else:
            self.BASE_DIR = Path(__file__).resolve().parent.parent

        self.PARENT_DIR = Path(__file__).resolve().parent

        self.MODEL_PATH = self.BASE_DIR / "models" / \
            "qwen2.5-7b-instruct-q4_k_m-00001-of-00002.gguf"

        # self.MODEL_PATH = self.BASE_DIR / "models" / "qwen2.5-3b-instruct-q4_k_m.gguf"
        self.N_CTX = 4096
        self.N_GPU_LAYERS = -1
        self.USE_MOCK = False


settings = Settings()
