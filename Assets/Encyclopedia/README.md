# Enzyklopädie bearbeiten

Alle eigenen Bestandteile liegen in `Assets/Encyclopedia`. Das Prefab
`Encyclopedia.prefab` ist in `UI.prefab` eingebettet. In der Szene findest du
es unter `UI > Encyclopedia`. Der Controller bleibt aktiv; nur `Window`
wird ein- und ausgeschaltet. `TreeRowTemplate` ist absichtlich deaktiviert.

## Bedienung

- **J** öffnet und schließt die Enzyklopädie; **X** schließt sie ebenfalls.
- **ESC** schließt zuerst die Enzyklopädie. Erst ein weiterer Druck öffnet
  das normale Pausenmenü. Während das Pausenmenü offen ist, öffnet J nichts.
- Ein Klick auf eine Kategorie klappt ihre Kinder auf oder zu und zeigt
  zugleich ihre Übersichtsseite rechts.
- Gelbe Begriffe im Text und sämtliche Teile der Adresse öffnen den jeweiligen
  Artikel. Die Navigation klappt dabei dessen übergeordnete Kategorien auf.
- Beide Bereiche lassen sich mit dem Mausrad oder durch Ziehen scrollen.
- Das Lesen pausiert das Spiel nicht. Beim Schließen wird der vorherige
  Mauszeigerzustand wiederhergestellt.

## Vorhandenen Artikel ändern

1. Wähle unter `Assets/Encyclopedia/Entries` die passende `.asset`-Datei.
2. Ändere **Title** und **Body** im Inspector. Body ist reiner Text;
   Leerzeilen erzeugen Absätze. Kein TMP- oder HTML-Markup nötig.
3. **Keywords** enthält zusätzliche Schreibweisen. Beim Imperium ist dies
   beispielsweise `Imperium`, während der eigentliche Titel `Das Imperium`
   lautet. Titel und Keywords werden automatisch verlinkt, unabhängig von
   Groß-/Kleinschreibung und nur an Wortgrenzen.
4. Änderungen außerhalb des Play Mode speichern; beim nächsten Spielstart
   werden sie eingelesen. Die mitgelieferten Texte sind frei ersetzbare Beispiele.

## Neuen Artikel oder eine Kategorie anlegen

1. Rechtsklick im Ordner `Entries` → **Create > Encyclopedia > Eintrag**.
2. Dateinamen vergeben. **Id** eindeutig vergeben, z.B. `eiswolf`;
   erlaubt sind Buchstaben, Zahlen, Bindestrich und Unterstrich.
   Diese dauerhafte ID später möglichst nicht ändern.
3. **Title**, **Body** und gegebenenfalls **Keywords** ausfüllen.
4. Öffne das Asset der übergeordneten Kategorie, z.B. `tiere.asset`.
5. Füge dein neues Asset unter **Children** hinzu. Die Reihenfolge dieser
   Liste bestimmt die Reihenfolge in der Navigation.

Eine Kategorie ist ebenfalls ein Artikel, hat aber zusätzliche Einträge
unter **Children**. So kannst du beliebig weitere Ebenen ergänzen und
Kategorien über die Adresse öffnen. Ihre Kinder erscheinen zusätzlich als
Links am Ende der Übersichtsseite. Der Wurzeleintrag ist `encyclopedia.asset`.

Jeder Eintrag gehört an genau eine Stelle des Baums. Keine Kreisverweise,
keine doppelten IDs und keine identischen Titel/Keywords für verschiedene
Artikel verwenden: Die Datenprüfung meldet solche Fehler ausdrücklich.
Das Umbenennen einer Datei verändert deren Verweise nicht. Ein geänderter
Titel erfordert gegebenenfalls angepasste Erwähnungen in anderen Texten;
alte Schreibweisen lassen sich als Keywords beibehalten.

## Darstellung und Technik

Im Prefab Mode von `Encyclopedia.prefab` kannst du Farben, Abstände,
Schriftgrößen und den inaktiven `TreeRowTemplate` bearbeiten. Die Fenstergröße
verwendet relative Anker mit einem Canvas Scaler auf 1920 × 1080 (Expand).
Navigation und Artikel haben eigene ScrollRects; lange Texte bleiben lesbar.

`EncyclopediaEntry` speichert Inhalte als ScriptableObject.
`EncyclopediaIndex` prüft den Baum und erzeugt automatische TMP-Links.
`EncyclopediaRow` stellt die Navigation dar. `EncyclopediaLink` übersetzt
einen Textklick in eine Artikel-ID. `EncyclopediaController` verbindet
Ansicht, Daten und die J-Taste. Alle UI-Referenzen sind explizit serialisiert.
Die Verbindung zum bestehenden ESC-Controller ist am eingebetteten Prefab
in `UI.prefab` gesetzt; das Einzelprefab benötigt diese Verbindung ebenfalls,
wenn es unabhängig eingebaut werden soll.

**Tools > Validate Encyclopedia** prüft im Unity-Editor Daten, Verbindungen
und die Navigation mit ESC. Darstellung, Scrollen und Textklicks zusätzlich
im Play Mode bei verschiedenen Game-View-Auflösungen prüfen.
