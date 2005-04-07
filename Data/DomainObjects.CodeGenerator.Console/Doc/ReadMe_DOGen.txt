Verwendung des Generators
-----------------------------

1. Generieren der stub-Klassen
   Aufruf mit /stubs erzeugt aus der mapping.xml ein C# codefile mit Klassenr�mpfen f�r alle im mapping angegebenen Klassen.
   
2. Kompilieren des Projekts
   Das Stubs-File muss nun in das Projekt eingebunden, und das Assembly kompiliert werden.
   Nun kann das Mapping gelesen, und daraus der Code generiert werden.
   
3. Generieren von setupDB.sql
   Aufruf mit /sql erzeugt aus der mapping.xml ein sql script zum Aufbau der Datenbank.
   
4. Generieren des Domain Models
   Aufruf mit /dom erzeugt aus der mapping.xml das DomainModel.
