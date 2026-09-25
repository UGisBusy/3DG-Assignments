# Task List — The Boomerang Guardian (for Kanban import)

Source: `3DGP_PA_01_Instruction.pdf`. Each bullet below is one Kanban card/task.

## Environment & Scene
- Create ground plane
- Build boundary walls (cubes around playable region)
- Place center platform object
- Set up scene hierarchy & folder structure
- Set up tags/layers (Target, Obstacle, Player, Collectible, Ground)

## Player Control
- Player movement forward/backward (W/S)
- Camera-relative movement direction
- Smooth movement (acceleration/damping)
- Speed toggle: normal/fast (Space)
- Jump (F key)
- Gravity (fall back to ground)
- Projectile motion during jump
- Ground check (prevent double jump)

## Camera System
- First-person camera setup
- First-person pitch/yaw look
- Third-person camera setup
- Third-person pitch/yaw
- Third-person zoom (adjustable distance)
- Toggle first-person/third-person view
- Camera collision (avoid clipping through walls)

## Object Selection (Ray Casting)
- Left-click raycast picking
- Valid target detection → throw boomerang
- Invalid selection → play error sound
- Layer mask / max ray distance setup

## Boomerang System
- Boomerang prefab + script
- Throw boomerang toward selected target
- Curved/arc flight path
- Return boomerang to player after attack
- Re-attach boomerang to player on return
- 3-second throw cooldown
- Cooldown UI indicator

## Spawn Manager
- SpawnManager script (random object generation)
- Guarantee 100+ target objects
- Guarantee 100+ obstacle objects
- Total object count 200–500
- Random position generation within bounds
- Overlap/collision check on spawn
- Retry logic for failed spawn positions
- Object pooling for targets/obstacles

## Physics & Collision Response
- Boomerang hit pushes target (impulse force)
- Increase score on target hit
- Target stays visible 2s after hit, then disappears
- Player can push obstacle objects
- Tune Rigidbody mass/drag for believable pushes

## Interactive Platform
- Platform trigger collider (enter/exit detection)
- On enter: hide all targets and obstacles
- On exit: respawn all targets and obstacles
- Platform enter sound
- Platform exit sound

## Collectible Items
- Collectible prefab
- Pickup trigger logic (item disappears + triggers effect)
- Spawn collectibles in the world

## User Interface
- Score display (UI text)
- Score updates on target hit
- Minimap (upper-right corner)
- Minimap shows player position and nearby objects

## Audio System
- AudioManager setup
- Sound: throwing boomerang
- Sound: boomerang hits object
- Sound: invalid selection (error)
- Sound: entering platform
- Sound: leaving platform

## Program Control & Polish
- ESC key quits application
- Control hint / help overlay
- Bug fixing & stability pass

## Report (Bonus 10%)
- Log AI tool usage during development
- Screenshot backlog/kanban board for evidence
- Write AI tools & Scrum reflection report (500+ words, PDF)
- List & credit third-party assets used
