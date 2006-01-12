@set target=%1
@if "%1"=="" set target=all
nant local clean resources && nant -t:net-1.1 local setdebug %target% && nant local cleantemp 