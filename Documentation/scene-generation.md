
# Navigation segments

The scene controller handles the generation of the navigation segments.  

When placing the segments back-to-back, the scene controller expects the transforms anchor position to be at the front center of the segment.  It expects the overall prefab to have a _single_ box collider attached to the back-end of the segment which is used to trigger when a segment has been passed by the player.  __It is important to note that if there is not a box collider at the root of the object, or if there is more than 1 box collider attached to the root, the scene controller will fail__