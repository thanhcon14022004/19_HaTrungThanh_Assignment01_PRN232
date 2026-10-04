@echo off
cd /d "%~dp0"
title DAY DU AN LEN GITHUB - 19_HaTrungThanh
echo ===================================================
echo   DANG DAY TOAN BO DU AN LEN GITHUB...
echo   Repo: https://github.com/thanhcon14022004/19_HaTrungThanh_AssignmentPRN232.git
echo ===================================================
echo.
git push -u origin main
echo.
if %ERRORLEVEL% equ 0 (
    echo ===================================================
    echo   DA DAY LEN GITHUB THANH CONG!
    echo ===================================================
) else (
    echo [!] Neu GitHub yeu cau xac thuc, vui long chon 'Sign in with your browser' tren cua so hien len.
)
pause
