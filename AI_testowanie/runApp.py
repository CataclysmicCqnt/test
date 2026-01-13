import os
import subprocess
import sys
import time
import threading
import socket
import uvicorn
from app.main import app
from app.config import settings

IS_TEST_MODE = True


class NullWriter:
    def write(self, text): pass
    def flush(self): pass
    def isatty(self): return False


if sys.stdout is None:
    sys.stdout = NullWriter()
if sys.stderr is None:
    sys.stderr = NullWriter()

if getattr(sys, 'frozen', False):
    BASE_DIR = os.path.dirname(sys.executable)
    INTERNAL_DIR = sys._MEIPASS
else:
    BASE_DIR = os.path.dirname(os.path.abspath(__file__))
    INTERNAL_DIR = BASE_DIR

UI_DIR = os.path.join(INTERNAL_DIR, "UI_DEV")
HTML_FILE = os.path.join(UI_DIR, "index.html")


def is_port_open(port):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        return s.connect_ex(('127.0.0.1', port)) == 0


def start_ollama():
    if is_port_open(11434):
        return None

    startupinfo = None
    if os.name == 'nt':
        startupinfo = subprocess.STARTUPINFO()
        startupinfo.dwFlags |= subprocess.STARTF_USESHOWWINDOW

    try:
        return subprocess.Popen(
            ["ollama", "serve"],
            cwd=BASE_DIR,
            shell=True,
            startupinfo=startupinfo,
            creationflags=subprocess.CREATE_NO_WINDOW if os.name == 'nt' else 0,
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL
        )
    except:
        return None


def open_browser():
    if os.path.exists(HTML_FILE):
        try:
            os.startfile(HTML_FILE)
        except:
            pass


def main():
    import multiprocessing
    multiprocessing.freeze_support()

    # 1. WYŚWIETLENIE MENU
    print("\n" + "="*50)
    print("   🕵️  NARRATOR AI - PANEL STARTOWY")
    print("="*50)

    # --- Pytanie 1: Przeglądarka ---
    print("\n[1/2] CZY OTWORZYĆ OKNO GRY?")
    print("   1. TAK - Uruchom grę w przeglądarce")
    print("   2. NIE - Tylko serwer")
    wybor_browser = input("   Twój wybór (domyślnie 1): ").strip()

    should_open_browser = True
    if wybor_browser == "2":
        should_open_browser = False
        print("   >> OK, przeglądarka nie zostanie otwarta.")
    else:
        print("   >> OK, otwieram grę automatycznie.")

    # --- Pytanie 2: Tryb AI (Mock vs Live) ---
    print("\n[2/2] JAKI TRYB AI URUCHOMIĆ?")
    print("   1. MOCK MODE (Bez AI) - Szybki test, stałe odpowiedzi.")
    print("   2. LIVE AI (Pełne AI) - Generowanie na żywo (wymaga Ollama).")
    wybor_ai = input("   Twój wybór (domyślnie 1): ").strip()

    if wybor_ai == "2":
        settings.USE_MOCK = False
        print("   >> TRYB: LIVE AI 🧠 (Upewnij się, że Ollama działa!)")
    else:
        settings.USE_MOCK = True
        print("   >> TRYB: MOCK 🎭 (Testowe odpowiedzi)")

    print("="*50 + "\n")
    print("Uruchamiam serwer... (Możesz zminimalizować to okno)")

    ollama_proc = start_ollama()
    time.sleep(2)

    if should_open_browser:
        threading.Timer(1.5, open_browser).start()

    try:
        uvicorn.run(
            app,
            host="127.0.0.1",
            port=8000,
            log_level="critical",
            use_colors=False,
            ws_ping_interval=None,
            ws_ping_timeout=None
        )
    finally:
        if ollama_proc:
            ollama_proc.terminate()


if __name__ == "__main__":
    main()
