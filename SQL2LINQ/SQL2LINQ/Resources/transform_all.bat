@echo off
SETLOCAL ENABLEDELAYEDEXPANSION
setx VS140COMNTXTTRANSFORM "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\Entity Framework Tools\Templates\Includes" /m
:: set the working dir (default to current dir)
set wdir=%cd%
if not (%1)==() set wdir=%1

:: set the file extension (default to vb)
set extension=cs
if not (%2)==() set extension=%2

echo executing transform_all from %wdir%
:: create a list of all the T4 templates in the working dir
dir %wdir%\*.tt /b /s > t4list.txt

echo the following T4 templates will be transformed:
type t4list.txt

:: transform all the templates
for /f %%d in (t4list.txt) do (
set file_name=%%d
set file_name=!file_name:~0,-3!.%extension%
echo:  \--^> !file_name!    
TextTransform.exe -I "%VS140COMNTXTTRANSFORM%" -out !file_name! %%d
)

echo transformation complete