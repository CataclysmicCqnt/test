@echo off
Title Narrator AI - Launcher
cls

echo ========================================================
echo    NARRATOR AI - AUTOMATYCZNY STARTER
echo ========================================================
echo.
echo Co chcesz zrobic?
echo.
echo 1. PIERWSZE URUCHOMIENIE (Instalacja bibliotek i modelu AI)
echo 2. START (Tylko uruchom aplikacje)
echo.
set /p wybor="Wybierz opcje [1 lub 2]: "

if "%wybor%"=="1" goto INSTALL
if "%wybor%"=="2" goto RUN
goto END

:INSTALL
cls
echo --------------------------------------------------------
echo KROK 1/3: Instalowanie bibliotek Python...
echo --------------------------------------------------------
pip install -r requirements.txt
if %errorlevel% neq 0 (
    echo BLAD: Nie udalo sie zainstalowac bibliotek. Sprawdz czy masz Pythona.
    pause
    exit
)

echo.
echo --------------------------------------------------------
echo KROK 2/3: Pobieranie modelu bazowego (gpt-oss:20b)...
echo (To moze potrwac chwile - ok. 10-15 GB do pobrania)
echo --------------------------------------------------------
ollama pull gpt-oss:20b

echo.
echo --------------------------------------------------------
echo KROK 3/3: Tworzenie modelu gry (game-npc-model)...
echo --------------------------------------------------------
ollama create game-npc-model -f Modelfile

echo.
echo SUKCES! Wszystko zainstalowane.
echo Przechodze do uruchamiania...
timeout /t 3 >nul
goto RUN

:RUN
cls
echo Uruchamianie glownego programu...
echo.
:: To polecenie uruchamia Twoj skrypt Python
python runApp.py
pause
goto END

:END