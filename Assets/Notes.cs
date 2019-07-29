/* NOTIZEN 
 * Random Parameter Configurations
 *
 * 
 *
 * CameraMovement
 * -> PointCloud CameraView
 * -> TreeGrowth CameraView
 * 
 * 
 * Punktewolkengenerator
 * - DONE was überlegen für die Form der Basis-Baumkrone
 * - DONE in die Höhe ziehen
 * - DONE in die Breite ziehen
 * - Punkte überwiegend am Rand
 * - Punkte während des Wachstums hinzufügen
 * - density, influenceDistance und ClearDistance davon abhängig machen
 * 
 * - Verformung der Krone zur Laufzeit bei schon schöner Form
 * -> die alten Punkte sollten möglichst behalten werden, vermutlich nicht so einfach
 *
 * - genau so: Änderungen der cutoffRatio zur Laufzeit?
 *
 * - Sinnhaftigkeit des density Parameters überdenken
 * 
 * - Kugelschnitt anpassbar machen
 * 
 * - Wenn die Breite/Tiefe der Krone größer als die Höhe ist, sollten die Tropismen nach oben entfernt werden?
 *
 * - Punktwolke sichtbarer darstellen
 * 
 * 
 * Geometrie
 * - Verdrehung fixen
 * - Curve-Resolution
 * - Algorithmus für Dreiecke an Gabelungen überlegen, maximale Anzahl an Gabelungen zulassen? (auch in Anlehnung an Realität)
 *
 * //- Stammdicke interpolieren für flüssigere Übergänge bei kleinen Werten für nth_root
 * 
 * - Blätter als Tetraeder oder ein paar senkrecht zueinander stehende Bilder?
 * - Blätter besser verteilen an Nodes?
 * - mehrere Farben auswählbar machen für Blätter
 * - Farbenverläufe
 *
 * 
 * Model
 * - Konifären mittels Apical Control?
 * - hängende Äste
 * - - über Änderung des PerceptionVolumes -> Achse in Abhängigkeit der Noderichtung
 * - - bei Wachstum nach oben: Rotation um Winkel nach unten statt "Verschiebung der Wuchsrichtung" nach unten
 * - - über neue AttractionPoints, Persistenz durch Seed?
 *
 * 
 *
 * Space Colonization
 * - nächsten Punkt finden über
 * - - Voxelgrid
 * - - DONE Sortierung und dann Suchen
 * - - Octree
 * 
 * - abbrechen, wenn die neuen Nodes immer wieder entfernt werden
 * - neuer Seed, wenn der Baum schon nach wenigen Iterationen aufhört zu wachsen?
 *
 *
 * - neuer Seed <-> Würfeln
 * //- Smooth-Slider Age
 * 
 * 
 * 
 * UI
 * - Save Feedback
 * 
 *
 * 
 *
 */