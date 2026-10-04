@echo off
cd /d "%~dp0"
title HE THONG QUAN LY TIN TUC FUNEWS - 19_HaTrungThanh

echo ======================================================================
echo    HE THONG QUAN LY TIN TUC FUNEWS - 19_HaTrungThanh
echo    PRN232 Assignment 01: ASP.NET Core Web API and MVC
echo ======================================================================
echo.

echo [1/4] Dang kiem tra va tat tien trinh cu (neu co)...
taskkill /F /IM 19_HaTrungThanh_Assignment01_BackEnd.exe >nul 2>&1
taskkill /F /IM 19_HaTrungThanh_Assignment01_FrontEnd.exe >nul 2>&1

echo.
echo [2/4] Khoi dong BackEnd API (Port 5134)...
start "FUNews BackEnd API (Port 5134)" cmd /k dotnet run --project .\19_HaTrungThanh_Assignment01_BackEnd\19_HaTrungThanh_Assignment01_BackEnd.csproj --launch-profile http

echo Dang cho BackEnd API khoi dong...
ping 127.0.0.1 -n 4 >nul

echo.
echo [3/4] Khoi dong FrontEnd Web App (Port 5173)...
start "FUNews FrontEnd Web (Port 5173)" cmd /k dotnet run --project .\19_HaTrungThanh_Assignment01_FrontEnd\19_HaTrungThanh_Assignment01_FrontEnd.csproj --launch-profile http

echo Dang cho FrontEnd Web khoi dong...
ping 127.0.0.1 -n 4 >nul

echo.
echo [4/4] Dang mo trinh duyet Web...
start http://localhost:5173

echo.
echo ======================================================================
echo    KHOI DONG THANH CONG!
echo ======================================================================
echo    - FrontEnd Web: http://localhost:5173
echo    - Swagger API:  http://localhost:5134/swagger
echo.
echo    TAI KHOAN DANG NHAP:
echo    - Admin: admin@FUNewsManagementSystem.org / @@abc123@@
echo    - Staff: thanhhthe182267@fpt.edu.vn / Hathanh55
echo ======================================================================
echo Nhan phim bat ky de dong cua so dieu khien nay...
pause >nul
