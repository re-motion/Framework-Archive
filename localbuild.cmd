@set target=%1
@if "%1"=="" set target=all
nant local clean resources && nant local setdebug %target% && nant local cleantemp 