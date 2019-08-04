/* BUGS
 *
 * Twisting-Bug -> Frenet Frame?
 * Manche kleinen Äste flippen immer noch over
 *
 *
 *
 * bei dickem Stamm und zu vielen Subnodes, kann es sein, dass der Übergang nicht mehr gut ist wegen minRadiusDifference
 *
 */


/* Perception Angle: 90°, Density: 30 -> braucht hohe Density damit initial ein Stamm machbar ist
 *
 * Vorteil: relative gerader Stamm mit theoretisch nur zwei Parametern realisierbar (niedrige d_c_begin, höhere d_c_end)
 * Nachteile:
 * - Äste winden sich um Baum im unteren Bereich / wachsen nach unten im Kreis
 * - Hängende Äste nicht einfach umsetzbar, da im oberen Bereich einfach keine Attraction Points mehr vorhanden sind
 * - CrownShape wird nicht voll ausgenutzt, der kleine Perception Angle macht einen umgedrehten Konus
 *
 *
 *
 */

/* Perception Angle 160, Density: 15
 * funktioniert gut
 *
 * angenommmen wir würden den PerceptionAngle trotzdem anpassbar machen, für etwas andere Formen, dann muss
 * -> die Density automatisch angepasst werden
 * -> die mindest Branch density automatisch angepasst werden ...
 *
 */


/* -> Attractionpoints near the envelope?
 * erfordert hohe Influence Distance, macht die Distanzberechnungen unmöglich zu optimieren
 * erfordert außerdem eine relativ spezifische Anpassung der Krone (cutoffRatio bottom sollte relativ groß sein)
 * Äste wachsen oft sehr zur Seite und dann nach oben
 *
 */

/* Hohe Influence Distance für exkurrente Bäume
 * -> macht Distanzeberechnungen unmöglich zu optimieren
 *
 */

/* NEXT STEPS
 *
 *
 * Surprise Parameters:
 * - Perception Angle (90 - 160) -> verändert außerdem Density (30 - 10)
 * 
 * 
 * Influence Distance anpassbar machen? -> weniger wobble -> vielleicht auch nur von 0.9 - 1.3
 * ->> Density muss von Influence Distance abhängen, irgendwas anderes auch ... (d_i 0.8, hohe Density bringt nix)
 *
 * Wichtigkeit des Lichts
 *
 * Seitenwind
 * 
 * Random Stamm mit Länge
 *
 *
 *
 * Hängende Äste mittels extra GrowthPhase mit extra AttractionPoints???
 * 
 *
 * bei höherem Alter bleiben die Astdichte-Parameter auf der Strecke
 * -> das liegt daran, dass der Baum den verfügbaren Platz "zu schnell" eingenommen hat
 *
 *
 *
 * Tropismen nicht von der Iteration abhängig machen
 * -> x, y, z auf jeweils auf bestimmten Wertebereich bringen, sonst wird beispielsweise (0,0.2,0) am Ende doch wieder (0,1,0)
 * --> oder Realisierung mittels Weights?
 *
 * Hanging Branches werden vermutlich nichts bringen, wenn die Branch Density um Zweige sehr hoch ist
 *
 * Simply: Stammlänge als extra Parameter mit extra Wachstumsphase
 * ->> benötigt dann extra Punktwolke für den Stamm / Prozedurales Wachstum
 * -> Retry Pruning
 *
 * Tannen ausgehend von einem Prozeduralen Stamm + Apikalkontrolle über Zweigordnungen?
 *
 * kleinere Leaves / globale Leafdensity
 * 
 * 
 * Pruning
 * - active
 * : NearestNode for AttractionPoint
 * : Grower
 * -> hat nicht funktioniert
 *
 * PerceptionAngle auch abhängig von BranchDensity machen
 * -> wird wahrscheinlich schwierig mit Density
 * 
 * Distanzberechnungen optimieren
 *
 * Leaves nur an Tipnodes und Supertipnode setzen?
 * 
 * Middleware in Core verschieben
 * AttractionPoints aus GrowthProperties nehmen?
 * CrownShape -> SliderScript
 * GrowthCamera smoother machen
 * 
 * Seitenwind
 *
 * Stammdicke
 *
 * TanH squishen (Sigmoid Ersatz)
 *
 * 
 *
 * Shadowgrid
 * -> behebt evtl. perfekte Rundungen (Astdichte am Ende reduzieren könnte auch helfen)
 *
 * Äste wachsen zum Schluss oft nach unten/um den Baum herum (insbesondere sichtbar, wenn man den Transition Parameter aktiviert)
 * -> ShadowGrid
 * 
 * Exkurrente Bäume über Ordnung der Zweige?
 * Radiergummi
 * LIVE PARAMETER!!!
 *
 *
 * 
 * AttractionPoints nach Entfernung gewichten -> behebt eventuell (je weiter weg, desto wichtiger!(?))
 * :: Bei sehr hohen Bäumen fangen die Äste in späteren Iterationen an, nach unten zu wachsen -> vermeiden (und dabei hanging branches offen lassen!)
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
 * Wenn die Breite/Tiefe der Krone größer als die Höhe ist, sollten die Tropismen nach oben entfernt werden?
 *
 * 
 * investigate on .mtl -> mehrere Texturen sind so referenzierbar
 *
 *
 * 
 * fester Perception Angle von 90, hohe Density (50)
 * ClearDistance wird angepasst
 * -> muss maximalwert von ca. InfluenceDistance - 0.075 haben
 * -> Anpassbarer Bereich ist der Abstand zur InfluenceDistance
 * -> Astdichte am Anfang
 * -> Astdichte am Ende
 * -> IterationMapping Threshhold (Sigmoid Übergang) bestimmt Verhältnis zwischen Stammlänge / Krone
 * >>> PerceptionAngle lockern für größere Werte :D:D:D:D
 * Evtl. später Perception Angle bei diesem Threshhold auch wieder "freigeben" -> 160 (wird vermutlich wegen hoher Density nicht gut aussehen)
 */
