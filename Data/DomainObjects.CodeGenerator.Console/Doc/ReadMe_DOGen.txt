DOGen.exe verwenden
-------------------


1. Stub-Klassen generieren:
   Aufruf mit /stubs erzeugt aus der mapping.xml ein C#-File mit Klassenr�mpfen f�r alle im Mapping angegebenen Klassen.
   
2. Zielprojekt kompilieren:
   Das Stubs-File muss in das Projekt eingebunden und die Assembly kompiliert werden.
   Nun kann das Mapping gelesen und daraus der Code generiert werden.
   
3. Datenbank-Setup-Script erzeugen:
   Aufruf mit /sql erzeugt aus der mapping.xml ein SQL-Script zum Aufbau der Datenbank.
   
4. Gesch�ftsobjekte generieren:
   Aufruf mit /classes erzeugt aus der mapping.xml die C#-Gesch�ftsobjekte.

   
Hinweis: Alternativ kann mit /full Punkt 3 und 4 in einem Schritt ausgef�hrt werden.  
