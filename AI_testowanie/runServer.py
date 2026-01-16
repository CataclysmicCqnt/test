import uvicorn
import os
import sys
import multiprocessing

sys.stdout = open(os.devnull, 'w')
sys.stderr = open(os.devnull, 'w')

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

if __name__ == "__main__":
    multiprocessing.freeze_support()

    try:
        from app.main import app

        uvicorn.run(app, host="127.0.0.1", port=8000,
                    reload=False, workers=1, log_level="critical")

    except Exception:
        pass
