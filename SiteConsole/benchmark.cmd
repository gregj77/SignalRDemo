@echo off
echo running benchmark for %1 clients

for /L %%i in (1,1,%1) do (
echo starting %%i client...
start chatclient.exe benchmark
)