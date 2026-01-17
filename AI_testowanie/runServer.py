import webbrowser
import uvicorn
import os
import sys
import multiprocessing


sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))


def isFrozenExe():
    return getattr(sys, 'frozen', False)


def openUiDev():

    basePath = os.path.dirname(os.path.abspath(__file__))
    uiPath = os.path.join(basePath, "UI", "index.html")

    if os.path.exists(uiPath):
        try:
            choice = input(
                "Czy chcesz otworzyć panel testowy w przeglądarce? [y/n]: ").strip().lower()
            if choice == 'y':
                print("Otwieranie...")
                webbrowser.open(f"file://{uiPath}")
        except Exception as e:
            print(f"Nie udało się: {e}")
    else:
        print("\nNie znaleziono folderu UI/index.html")


if __name__ == "__main__":
    multiprocessing.freeze_support()

    if isFrozenExe():

        applicationPath = os.path.dirname(sys.executable)
        logFilePath = os.path.join(applicationPath, "serverLog.txt")

        sys.stdout = open(logFilePath, 'w', encoding='utf-8')
        sys.stderr = sys.stdout

        uvicornLogLevel = "info"

    else:

        print("\n" + "="*50)
        print("🚀 URUCHAMIANIE W TRYBIE DEVELOPERSKIM")
        openUiDev()
        print("Logi będą widoczne tutaj")
        print("=" * 50 + "\n")

        uvicornLogLevel = "info"
    try:
        from app.main import app

        uvicorn.run(app, host="127.0.0.1", port=8000,
                    reload=False, workers=1, log_level=uvicornLogLevel)

    except Exception:
        print(f"\nCRITICAL ERROR: {e}")
        import traceback
        traceback.print_exc()

        if not isFrozenExe():
            input("Naciśnij Enter aby zamknąć...")
