rem nant incversion
nant clean
  if errorlevel 1 pause "build error"
nant setdebug all alltests
  if errorlevel 1 pause "build error"
nant setrelease all alltests
  if errorlevel 1 pause "build error"
nant doc-public doc-internal
  if errorlevel 1 pause "build error"
nant cleantemp zip sourcezip
  if errorlevel 1 pause "build error"
nant getcurrent
  if errorlevel 1 pause "build error"
