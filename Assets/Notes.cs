/* BUGS
 *
 * Twisting-Bug -> Frenet Frame?
 * Manche kleinen Äste flippen immer noch over
 *
 */


/* NEXT STEPS
 *
 * fester Perception Angle von 90, hohe Density (50)
 * ClearDistance wird angepasst
 * -> muss maximalwert von ca. InfluenceDistance - 0.075 haben
 * -> Anpassbarer Bereich ist der Abstand zur InfluenceDistance
 * -> Astdichte am Anfang
 * -> Astdichte am Ende
 * -> IterationMapping Threshhold (Sigmoid Übergang) bestimmt Verhältnis zwischen Stammlänge / Krone
 * Evtl. später Perception Angle bei diesem Threshhold auch wieder "freigeben" -> 160 (wird vermutlich wegen hoher Density nicht gut aussehen)
 *
 * Pruning
 * 
 * 
 * 
 * Distanzberechnungen optimieren
 *
 * Shadowgrid
 * -> behebt evtl. perfekte Rundungen (Astdichte am Ende reduzieren könnte auch helfen)
 *
 * Äste wachsen zum Schluss oft nach unten
 * -> ShadowGrid
 *
 * Exkurrente Bäume über Ordnung der Zweige?
 * 
 * Middleware in Core verschieben
 * 
 * Radiergummi
 * Seitenwind
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
 * Sigmoid Clear Distance parametrisieren -> evtl. gar nicht notwendig mit Shadowgrid Grower
 *
 *
 * LeafType:Triangle -> Anzahl der Blätter einstellbar machen
 * Ansonsten:
 * - Triangle: Anzahl Blätter / Blattgröße
 * - Particle: Anzahl Blätter, Lappengröße, Blattgröße
 * - +++ Einfach nur in der Krone verteilen? -> Gar nicht mal an spezifischen Nodes?
 * 
 * Elemente die zusammengehören in Container packen und erst ausklappen wenn gewünscht?
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