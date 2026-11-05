# Mini-exercise 4.B.1 – A* algorithm, path finding

Click the floor and a primitive character walks there along an A* path that goes around the walls, rocks and trees.
The path is shown as a line of small yellow spheres, with a red sphere at the target.

## Setup

1. Open `Mini2_AStarPathfinding.unity` in this folder and press **Play**.
2. **Left-click** anywhere on the floor. Click again at any time to change the target while the character is walking.

The status line in the top-left shows the last search: cells on the path, path length, number of spheres and search time.
Select **Grid** in the Hierarchy during Play to see the blocked cells (red) in the Scene view.

## Components

| Component | What it does | On |
| --- | --- | --- |
| `GridMap` | Splits the 40 x 40 m floor into 1 m cells. On Start, `Physics.CheckBox` tests each cell for colliders and marks it walkable or blocked. Obstacles are grown by `agentRadius` so the character keeps clear of corners. Converts world position ↔ cell, and finds the nearest free cell. | Grid |
| `AStar` | Static A* search on the grid (8 directions, no corner cutting). See below. | – |
| `ClickToMove` | Left-click → ray from the mouse → hit point sent to `PathFollower`. Draws the status line. | Character |
| `PathFollower` | Runs A* from the character's cell to the clicked cell, turns the cells into waypoints and walks them at `speed`. Turns with `RotateTowards` (`turnSpeed` deg/s) and walks slower while it is still turning. | Character |
| `PathVisualizer` | Places a sphere every `spacing` (0.6 m) along the path, so the number of spheres depends on the path length. Spheres are reused and hidden once the character passes them. | PathVisualizer |
| `WalkAnimator` | Code-driven walk: hips and shoulders swing with `sin(phase)`, legs and arms in opposite phase, plus a small body bob. The phase advances with the measured speed (`speed / strideLength`), and the swing fades in or out when walking starts or stops. | Character |
| `EnvironmentBuilder` | On Play, builds 6 wall segments and scatters 35 rocks and 25 trees in loops at random spots that do not overlap each other, the walls, or the start area. | Environment |

## How A* works here

Each cell `n` has
- `g(n)`: the cost of the best path found so far from the start (1 per straight step, √2 per diagonal step),
- `h(n)`: the octile distance to the goal, `(dx + dz) + (√2 − 2) · min(dx, dz)`. This is the exact cost on an empty grid, so it never overestimates and A* returns a shortest path,
- `f(n) = g(n) + h(n)`.

Loop: take the open cell with the lowest `f` → if it is the goal, follow the `parent` links back to the start → otherwise close it and, for each walkable neighbour, keep the new `g` if it is lower than the old one and set the neighbour's parent.
A diagonal step is only allowed when both side cells are free, so the path never cuts a wall corner.

Turning the cells into waypoints (`PathFollower.SetDestination`):
- The first waypoint is the character's current position, not its cell centre. A re-click mid-walk continues from where it stands, so the character never jumps.
- The last waypoint is the exact click point when that cell is free. Otherwise it is the centre of the nearest free cell.

## Character hierarchy

Moving parts are empty pivots (scale 1) and the shapes hang under them, as in mini-1. The character has no colliders, so it never blocks clicks or grid cells.

```
Character                 PathFollower, ClickToMove, WalkAnimator, start (0, 0, -16)
└── Body                  pivot, bobs up and down
    ├── Torso             Capsule
    ├── Head              Sphere
    ├── Visor             Cube on the face, shows which way it faces
    ├── LeftHip / RightHip             pivot at y 0.6, swings about local x
    │   └── Leg           Cube, hangs below the pivot
    └── LeftShoulder / RightShoulder   pivot at y 1.38, swings about local x
        └── Arm           Cube
```

## Requirement mapping

- **Environment has obstacles:** 6 walls, about 35 rocks and 25 trees, placed in loops (more than 50 objects).
- **Mouse sets the target on the floor:** `ClickToMove`.
- **Compute a path:** `AStar.FindPath` on `GridMap`.
- **Path shown with small spheres, count depends on length:** `PathVisualizer`, one sphere every 0.6 m.
- **Character animated while walking:** `WalkAnimator`.
- **Moves continuously to the target:** `PathFollower` moves a small step each frame.
- **New target at any time:** a click re-plans from the current position.
- **Orientation changes smoothly, no jumps:** `RotateTowards` at a limited turn rate, and the new path starts at the current position.

## New vs reused

- **New for this exercise:** `GridMap`, `AStar`, `PathFollower`, `PathVisualizer`, `ClickToMove`, `WalkAnimator`, the primitive character.
- **Reused from mini-1:** `EnvironmentBuilder` (changed: square area, wall segments, keep-clear checks against walls), and the Rock / Tree prefabs with their materials (kept at `Assets/Mini1_HierarchicalCrane/`).
