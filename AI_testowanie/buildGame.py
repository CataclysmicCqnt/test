import PyInstaller.__main__
import os
import sys
import shutil
import re
from pathlib import Path


sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

try:
    from app.config import settings
except ImportError:
 
    sys.exit(1)

PROJECT_DIR = Path(__file__).resolve().parent
OUTPUT_DIR = PROJECT_DIR.parent / "GameAI_Backend"
MODEL_PATH = settings.MODEL_PATH



for folder in ['build', 'dist']:
    shutil.rmtree(PROJECT_DIR / folder, ignore_errors=True)
if (PROJECT_DIR / "GameAI_Server.spec").exists():
    os.remove(PROJECT_DIR / "GameAI_Server.spec")

try:
    import llama_cpp
    llama_cpp_path = Path(llama_cpp.__file__).parent
except ImportError:
    print("Błąd: Brak biblioteki llama-cpp-python.")
    sys.exit(1)

llama_lib_src = llama_cpp_path / "lib"
if not llama_lib_src.exists():
    llama_lib_src = llama_cpp_path

add_data_llama = f"{llama_lib_src}{os.pathsep}llama_cpp/lib"
add_data_app = f"app{os.pathsep}app"


PyInstaller.__main__.run([
    'runServer.py',
    '--name=GameAI_Server',
    '--onefile',
    '--noconsole',  # <--- ОКНА НЕ БУДЕТ
    '--collect-all=llama_cpp',
    f'--add-data={add_data_llama}',
    f'--add-data={add_data_app}',
    '--hidden-import=uvicorn',
    '--hidden-import=fastapi',
    '--hidden-import=pydantic',
    '--hidden-import=starlette',
    '--hidden-import=sse_starlette',
    '--hidden-import=anyio',
    '--hidden-import=contextlib',
    '--hidden-import=typing_extensions',
    '--log-level=ERROR',
])


if OUTPUT_DIR.exists():
    shutil.rmtree(OUTPUT_DIR)

OUTPUT_DIR.mkdir()
(OUTPUT_DIR / "models").mkdir()

exe_source = PROJECT_DIR / "dist" / "GameAI_Server.exe"
exe_dest = OUTPUT_DIR / "GameAI_Server.exe"

if exe_source.exists():
    shutil.move(str(exe_source), str(exe_dest))
else:
    print("Błąd: Brak pliku EXE.")
    sys.exit(1)


files_to_copy = []
model_dir = MODEL_PATH.parent
model_name = MODEL_PATH.name

split_match = re.match(r"(.*)-\d{5}-of-\d{5}\.gguf", model_name)

if split_match:
    base_name = split_match.group(1)

    for file in model_dir.glob(f"{base_name}-*-of-*.gguf"):
        files_to_copy.append(file)
else:
    files_to_copy.append(MODEL_PATH)

if not files_to_copy:
    print(f"Błąd: Nie znaleziono modelu {model_name}")
else:
    for src in files_to_copy:
        if src.exists():
     
            shutil.copy2(str(src), str(OUTPUT_DIR / "models" / src.name))

shutil.rmtree(PROJECT_DIR / "build", ignore_errors=True)
shutil.rmtree(PROJECT_DIR / "dist", ignore_errors=True)
if (PROJECT_DIR / "GameAI_Server.spec").exists():
    os.remove(PROJECT_DIR / "GameAI_Server.spec")

print(f"Gotowe: {OUTPUT_DIR}")
