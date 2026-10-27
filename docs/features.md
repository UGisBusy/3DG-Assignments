- Introduction Screen
  - Display text
  - Press Space to Start
  - Press ESC to Quit (works everywhere)

- Player Character 
  - Move using WASD 
  - Rotate with the mouse 
  - Attack nearby monsters  (Left Mouse Button)
    - sweep sword
  - Timer decrease if Player get hurt
  - Look Around: Mouse

- Monsters 
  - At least 5 monsters
  - stationary
  - Cast simple spell projectiles toward the player
  - Lose health when attacked
  - Life bar above its head

- Misc
  - Collectible
  - Breakable boxes
  - Lever 

- Puzzle System
  - Puzzle design:
    - Each Level is a Room
    - Locked Gate to Next Level
    - Complete task to unlock Gate
  
  - Puzzle 1:
    - Breakable boxes in the room
    - 1 of the box drop key
    - task: Collect the key
  
  - Puzzle 2:
    - room is a Maze
    - task: reach end
  
  - Puzzle 3:
    - 5 monsters in the room
    - defeat monster drop crystals
    - task: collect all 5 crystals
  
  - Puzzle 4: 
    - room has a gap
    - All 4 platforms need to be raised to form a bridge
    - 4 Lever each toggle their set of platforms
    - task: form the bridge and get to another side
  
  - Puzzle 5: 
    - 9 monster stand in the center
    - gap around the center
    - 4 corner have Lever which each activate 1 platform
    - activate 4 platform to reach the center
    - task: defeat all 9 monster and collect their crystals

- Audio
  - Sound Effect:
    - Play a hit sound when Player attacks monster
  - Background Music:
    - Start when gameplay begins
    - Continue until winning or losing

- Score System
  - Display current score
  - Recover monster: +100
  - Solve puzzle: +200

- Countdown Timer 
  - Display timer in the upper-left corner. 
  - Initial time: 5 minutes 
  - Format: MM:SS 
  - Timer continuously decreases

- Mini-map
  - Shows player location
  - Shows puzzle locations
  - Toggle with the M key

- Win Condition
  - All five puzzles are solved
  - Press R to Restart after win
  - Display

- Lose Condition
  - Countdown timer reaches zero
  - Press R to Restart after lose
  - Display

- Hint
  - show current level
  - show current task
