.\bin\Debug\DOGen.exe `
    "/mappingFile:Domain\SecurityManagerMapping.xml" `
    "/storageProvidersFile:Domain\SecurityManagerStorageProviders.xml" `
    "/sql" `
    "/sqloutput:Database"
    
get-content Database\SetupDB.sql | foreach { $_.Replace(" nvarchar (2147483647) ", " text ") } | set-content Database\SecurityManagerSetupDB.sql

remove-item Database\SetupDB.sql