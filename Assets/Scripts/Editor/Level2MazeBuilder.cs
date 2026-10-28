using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Builds the level 2 maze from MAP. Run it again after editing MAP to rebuild.
public static class Level2MazeBuilder
{
    const string MENU_PATH = "3DG/Build Level 2 Maze";
    const string WALL_PREFAB = "Assets/Resources/Maze Wall.prefab";
    const string LEVEL_NAME = "Level2";
    const string MAZE_NAME = "Maze";
    const float CELL_SIZE = 2f;
    const float WALL_Y = 2f; // wall is 4 tall, so its center sits at y = 2

    // '#' = wall, '.' = open
    // top row is the far side (+z), bottom row faces level 1 (-z)
    // the outer ring is skipped, those walls are placed by hand
    static readonly string[] MAP =
    {
        "#########.#########",
        "#...........#...#.#",
        "###.#######.#.#.#.#",
        "#...#...#...#.#.#.#",
        "#.###.#.###.#.#.#.#",
        "#.....#...#...#.#.#",
        "#.#######.#####.#.#",
        "#.#.....#.#...#...#",
        "#.#.###.#.#.#.###.#",
        "#.#.#...#.#.#...#.#",
        "#.###.###.#.#.###.#",
        "#...#.#...#.#.....#",
        "###.#.#.#######.###",
        "#.#.#.#.......#...#",
        "#.#.#.#######.###.#",
        "#.#.#.......#...#.#",
        "#.#.#.#.#######.#.#",
        "#.....#.#.......#.#",
        "#########.#########",
    };

    [MenuItem(MENU_PATH)]
    private static void Build()
    {
        GameObject level = GameObject.Find(LEVEL_NAME);
        if (level == null)
        {
            EditorUtility.DisplayDialog("Build Level 2 Maze", $"No '{LEVEL_NAME}' found in the open scene.", "OK");
            return;
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(WALL_PREFAB);
        if (prefab == null)
            throw new System.Exception($"[Level2MazeBuilder] prefab not found at {WALL_PREFAB}");

        int rows = MAP.Length;
        int cols = MAP[0].Length;
        foreach (string row in MAP)
        {
            if (row.Length != cols)
                throw new System.Exception("[Level2MazeBuilder] every MAP row must be the same length");
        }

        // replace the maze from a previous run
        Transform oldMaze = level.transform.Find(MAZE_NAME);
        if (oldMaze != null)
            Undo.DestroyObjectImmediate(oldMaze.gameObject);

        GameObject maze = new GameObject(MAZE_NAME);
        maze.transform.SetParent(level.transform, false);

        // center the grid on Level2, so column 0 is x = -18 and row 0 is z = 18
        float originX = -(cols - 1) * CELL_SIZE / 2f;
        float originZ = (rows - 1) * CELL_SIZE / 2f;

        int count = 0;
        for (int r = 1; r < rows - 1; r++)
        {
            for (int c = 1; c < cols - 1; c++)
            {
                if (MAP[r][c] != '#')
                    continue;

                GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(prefab, maze.transform);
                wall.name = $"Maze Wall ({r},{c})";
                wall.transform.localPosition = new Vector3(
                    originX + c * CELL_SIZE,
                    WALL_Y,
                    originZ - r * CELL_SIZE
                );
                count++;
            }
        }

        // registered after the walls are added, so one undo removes the whole maze
        Undo.RegisterCreatedObjectUndo(maze, MENU_PATH);
        Selection.activeGameObject = maze;
        EditorSceneManager.MarkSceneDirty(level.scene);
        Debug.Log($"[Level2MazeBuilder] placed {count} maze walls, save the scene to keep them");
    }
}
