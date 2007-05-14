.\..\..\Data\DomainObjects.CodeGenerator.Console\bin\Debug\DOGen.exe `
    "/mappingFile:Domain\SecurityManagerMapping.xml" `
    "/storageProvidersFile:Domain\SecurityManagerStorageProviders.xml" `
    "/sql" `
    "/sqloutput:Database"


remove-item Database\SecurityManagerSetupDB.sql

rename-item Database\SetupDB.sql SecurityManagerSetupDB.sql
