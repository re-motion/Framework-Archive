Verwendung des Generators
-----------------------------

1. Generieren der stub-Klassen
   Aufruf mit -stubs erzeugt aus der mapping.xml ein C# codefile mit Klassenrümpfen für alle im mapping angegebenen Klassen.
   
   mapping.xml muss im aktuellen Verzeichnis liegen (über -i kann ein anderer Pfad und Dateiname spezifiziert werden).
   
2. Kompilieren des Projekts
   Das Stubs-File muss nun in das Projekt eingebunden, und das Assembly kompiliert werden.
   Nun kann das Mapping vom RPF gelesen, und daraus der Code generiert werden.
   
3. Generieren von setupDB.sql
   Aufruf mit -sql erzeugt aus der mapping.xml ein sql script zum Aufbau der Datenbank.
   
   mapping.xml muss im aktuellen Verzeichnis liegen (über -i kann ein anderer Pfad und Dateiname spezifiziert werden).
   mapping.xsd muss im aktuellen Verzeichnis liegen.
   storageProviders.xml muss im aktuellen Verzeichnis liegen.
   storageProviders.xsd muss im aktuellen Verzeichnis liegen.
   types.xsd muss im aktuellen Verzeichnis liegen.

4. Generieren des Domain Models
   Aufruf mit -dom erzeugt aus der mapping.xml das DomainModel.
      
   mapping.xml muss im aktuellen Verzeichnis liegen (über -i kann ein anderer Pfad und Dateiname spezifiziert werden).
   mapping.xsd muss im aktuellen Verzeichnis liegen.
   storageProviders.xml muss im aktuellen Verzeichnis liegen.
   storageProviders.xsd muss im aktuellen Verzeichnis liegen.
   types.xsd muss im aktuellen Verzeichnis liegen.
