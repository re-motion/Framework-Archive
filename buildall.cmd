rem nant incversion
nant clean
      if errorlevel 1 pause "build error"
nant setdebug all alltests
      if errorlevel 1 pause "build error"
nant setrelease all alltests
      if errorlevel 1 pause "build error"

if "%1"=="nodoc" goto nodoc  
nant doc
      if errorlevel 1 pause "build error"
:nodoc

nant cleantemp zip sourcezip getcurrent
      if errorlevel 1 pause "build error"