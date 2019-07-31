/* BUGS
 *
 * manche kleinen Äste flippen immer noch over
 *
 */


/* NEXT STEPS
 *
 * Distanzberechnungen optimieren
 *
 * Shadowgrid
 * -> behebt evtl. perfekte Rundungen
 *
 */

/* RENDERING
 * 
 *
 * Sehr große Meshes aufsplitten
 * 
 * DONE CameraMovement
 * DONE -> PointCloud CameraView
 * DONE -> TreeGrowth CameraView
 * 
 * DONE Punktwolke sichtbarer darstellen
 */

/* UI
 *
 * Elemente die zusammengehören in Container packen und erst ausklappen wenn gewünscht?
 * 
 * Sigmoid Clear Distance parametrisieren -> evtl. gar nicht notwendig mit Shadowgrid Grower
 *
 *
 * LeafType:Triangle -> Anzahl der Blätter einstellbar machen
 * Ansonsten:
 * - Triangle: Anzahl Blätter / Blattgröße
 * - Particle: Anzahl Blätter, Lappengröße, Blattgröße
 * 
 * Elemente mit Icons
 *
 * Color Picker: Texture2D.SetPixel auf Basis von Template
 * 
 * //- Smooth-Slider Age
 * 
 * DONE Kugelschnitt anpassbar machen
 */


/* UI & MODEL
 *
 * Random Parameter Configurations
 *
 * 
 * abbrechen, wenn die neuen Nodes immer wieder entfernt werden
 * + neuer Seed, wenn der Baum schon nach wenigen Iterationen aufhört zu wachsen?
 *
 */

/* MODEL
 * 
 * Punktewolkengenerator
 * - DONE was überlegen für die Form der Basis-Baumkrone
 * - DONE in die Höhe ziehen
 * - DONE in die Breite ziehen
 * // - Punkte überwiegend am Rand
 * // - Punkte während des Wachstums hinzufügen
 * - density, influenceDistance und ClearDistance davon abhängig machen
 * 
 * - Verformung der Krone zur Laufzeit bei schon schöner Form
 * -> die alten Punkte sollten möglichst behalten werden
 * --> jetzige Punkte zurücktransformieren und dann neue hinzufügen, bis es wieder genug sind (bzw. entfernen bis es wenig genug sind)
 * -> genau so: Änderungen der cutoffRatio zur Laufzeit?
 *
 * - Wenn die Breite/Tiefe der Krone größer als die Höhe ist, sollten die Tropismen nach oben entfernt werden?
 *
 * 
 * Konifären mittels Apical Control?
 * 
 * hängende Äste
 * - über Änderung des PerceptionVolumes -> Achse in Abhängigkeit der Noderichtung
 * - bei Wachstum nach oben: Rotation um Winkel nach unten statt "Verschiebung der Wuchsrichtung" nach unten
 * - über neue AttractionPoints, Persistenz durch Seed?
 * >kleinere ClearDistance macht Tropismen hinfällig, viele AttractionPoints auch
 * - vermutlich weil es einfach unten keine AttractionPoints mehr gibt
 * 
 *
 * Space Colonization
 * - nächsten Punkt finden über
 * - - Voxelgrid
 * - - DONE Sortierung und dann Suchen
 * - - Octree
 * 
 */

/* GEOMETRY
* Verdrehung fixen
* 
* Curve-Resolution
* 
* Algorithmus für Dreiecke an Gabelungen überlegen, maximale Anzahl an Gabelungen zulassen? (auch in Anlehnung an Realität)
*
* //- Stammdicke interpolieren für flüssigere Übergänge bei kleinen Werten für nth_root
* 
* - Blätter als Tetraeder oder ein paar senkrecht zueinander stehende Bilder?
* - Blätter auch zwischen zwei Nodes?
* - Farbverläufe
* 
*/


/* DONE
 * 
 * investigate on .mtl -> mehrere Texturen sind so referenzierbar
 * 
 */