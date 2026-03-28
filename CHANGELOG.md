### v0.8
* Optimized layered rendering
* Switched from ray to segment for sight again
* Added `Grid` and `Key` entities
* Added `Face` geometry and returned `Circle` geometry

### v0.7
* Switched to real-time game cycle
* Added camera bobbing effect

### v0.6
* Added layered rendering and transparent walls
* Reduced player speed by half

### v0.5
* Changed screen resolution to 180x120 chars
* Added an alternative color palette at the cost of reducing brightness level number to 15
* Added invisible walls and ghost-walls

### v0.4
* Switched to WinAPI to update console area which greatly improved performance.

### v0.3
* Split wall ray scanning and wall collision logic
* Reworked wall collision: player got a hitbox
* Added support for 31 char brightness levels
* Switched back from segment to ray for sight

### v0.2
* Replaced `Rect` and `Circle` obstacles with `VertSeg` and `HorSeg` obstacles
* Added wall collision
* Switched from ray to segment for sight

### v0.1
* 120x80 char screen resolution
* 7 char brightness levels
* 90 degrees FOV
