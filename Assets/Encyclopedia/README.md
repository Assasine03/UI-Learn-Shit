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
2. Ändere **Überschrift** und Artikeltext im Inspector.
   Leerzeilen erzeugen Absätze. Markiere Wörter und nutze die Formatierungsleiste.
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

## Artikel-Editor im Inspector

Wähle ein Eintrag-Asset aus. Der eigene Inspector bietet:

- **B**, **I**, **U**, **S** für Fett, Kursiv, Unterstreichen und Durchstreichen.
- **Farbe** auswählen und **Anwenden** drücken.
- **Größe** eingeben, `%` oder `pt` wählen und **Anwenden** drücken.
  `120 %` ist relativ zur Grundschriftgröße des Artikels.
- Ein **TMP Font Asset** in **Schrift** ziehen und **Anwenden** drücken.
  Die Schrift wird direkt im Eintrag referenziert und zur Laufzeit registriert;
  sie muss nicht in einem Resources-Ordner liegen.
- **Alle Formate entfernen** entfernt Formatierungen im gesamten Artikel.
- **Artikel speichern** speichert das ausgewählte Asset. Änderungen lassen sich
  wie andere Inspector-Änderungen mit Strg+Z rückgängig machen.

Markiere zuerst die gewünschten Wörter im Textfeld, dann verwende eine Taste
oder **Anwenden**. Ohne Auswahl wird ein Platzhalter `Text` eingefügt und markiert.
Die Auswahl bleibt nach dem Formatieren erhalten, sodass du mehrere Stile
kombinieren kannst. Im Bearbeitungsfeld bleiben die Tags sichtbar; darunter
zeigt eine echte, scrollbar gerenderte **TextMeshPro-Vorschau** das Ergebnis.
Diese Vorschau ist keine Word-Oberfläche: Text wird oben eingegeben, unten
wird seine Darstellung geprüft. Die Vorschau-Links sind nicht anklickbar.

Automatische Artikel-Links funktionieren auch, wenn ein Artikelname durch
Formatierungen unterbrochen ist, z.B. `Hunter <b>Drone</b>`. Explizit gesetzte
Textfarben bleiben erhalten; solche Links werden weiterhin unterstrichen.
Die Vorschau ermittelt den Wurzeleintrag anhand der Children-Verweise, auch
bei Assets in Unterordnern. Bei einem noch nicht einsortierten neuen Artikel
kennt sie zunächst nur dessen eigenen Unterbaum.

Erlaubte Tags sind `b`, `i`, `u`, `s`, `size`, `color` und `font`.
Falsch geschlossene oder verschachtelte Formatierungen zeigt die Vorschau
als Fehler an. Mit Strg+Z oder **Alle Formate entfernen** kannst du sie korrigieren.
HTML und manuelle `link`-Tags werden nicht ausgeführt; Links werden weiterhin
automatisch aus Titeln und Suchbegriffen erstellt. Titel selbst bleiben reiner Text.

**Tools > Validate Encyclopedia Formatting** prüft Formatierung und Verlinkung
im Unity-Editor mit temporären Einträgen, ohne gespeicherte Artikel zu ändern.
