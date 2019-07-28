/* NOTIZEN 
 * nächste Schritte:
 * - Blätter dreieckig machen / austauschbar machen
 * >>!! Anzahl der Leaves im Nachhinein änderbar machen
 * - Texturen -> Farben
 * 
 * - UI
 * - - Alter
 * >> Settings in zentralem Objekt speichern
 * - - Speichern
 * - - Randomize
 * - Vertices und Triangles anzeigen
 *
 * - automatisches / besseres Camera-Movement
 * 
 * >> EventSystem für Änderungen einzelner Werte
 * 
 * 
 * - Punktewolkengenerator
 * - - DONE was überlegen für die Form der Basis-Baumkrone
 * - - DONE in die Höhe ziehen
 * - - DONE in die Breite ziehen
 * - - Punkte überwiegend am Rand
 * - - Punkte während des Wachstums hinzufügen
 * - - density, influenceDistance und ClearDistance davon abhängig machen
 *
 * 
 * - bessere Geometrie
 * - - DONE dünne Äste an dicken Ästen fixen
 * - - Verdrehung fixen
 * - Konifären mittels Apical Control?
 * - hängende Äste
 * - - über Änderung des PerceptionVolumes -> Achse in Abhängigkeit der Noderichtung
 * - - bei Wachstum nach oben: Rotation um Winkel nach unten statt "Verschiebung der Wuchsrichtung" nach unten
 * - - über neue AttractionPoints, Persistenz durch Seed?
 *
 * - 
 * 
 *
 * - middleware für Buttons?
 *
 * - nächsten Punkt finden über
 * - - Voxelgrid
 * - - DONE Sortierung und dann Suchen
 * - - Octree
 * 
 *
 * - Blätter als Tetraeder oder ein paar senkrecht zueinander stehende Bilder?
 * - Blätter besser verteilen an Nodes?
 * - mehrere Farben auswählbar machen für Blätter
 * - Farbenverläufe
 *
 *
 * - neuer Seed <-> Würfeln
 * - Smooth-Slider Age
 *
 *
 * 
 * - Verformung der Krone zur Laufzeit bei schon schöner Form
 * -> die alten Punkte sollten möglichst behalten werden, vermutlich nicht so einfach
 *
 * - genau so: Änderungen der cutoffRatio zur Laufzeit?
 *
 * - Sinnhaftigkeit des density Parameters überdenken
 *
 * 
 *
 * 
 *
 */

/* TODO
 * Wenn die Breite/Tiefe der Krone größer als die Höhe ist, sollten die Tropismen nach oben entfernt werden?
 * Kugelschnitt anpassbar machen
 * 
 * Stammdicke interpolieren für flüssigere Übergänge bei kleinen Werten für nth_root
 * 
 * Stamm
 * 
 * Curve-Resolution
 *
 * Distanzberechnungen parallelisieren VS Berechnungen für Subnodes parallelisieren
 *
 * sinnvolle Werte finden
 * - tipRadius
 * - nth_root
 *
 * 
 * 
 * Algorithmus für Dreiecke an Gabelungen überlegen, maximale Anzahl an Gabelungen zulassen? (auch in Anlehnung an Realität)
 *
 *
 * Vector3 newNodePosition = new Vector3(1000, 1000, 1000);
 * -> sinnvollen Default-Wert finden?
 * 
 * abbrechen, wenn die neuen Nodes immer wieder entfernt werden
 * 
 */

/* GELÖST
 *
 * zu viele Dreiecke werden buggy -> max 2^16 -1 Dreiecke erlaubt...
 *
 * uvs out of bounds fixen
 *
 *!! über Thread.Abort nachdenken, in Besitz genommene Locks werden nicht freigegeben!
 */

/* BEACHTEN
 * Resolution muss fuer alle Zylinder gleich sein
 * 
 * 
 */