rem nant incversion
nant clean
  if errorlevel 1 pause "build error"
nant setdebug all alltests
  if errorlevel 1 pause "build error"
nant setrelease all alltests
  if errorlevel 1 pause "build error"
nant doc-public doc-internal
  if errorlevel 1 pause "build error"
nant cleantemp zip
  if errorlevel 1 pause "build error"
