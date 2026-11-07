using TMPro;
using UnityEngine;

// Shows the current level and its objective on the HUD,
// then "go to the next level" once that level's puzzle is solved.
// Lives on a root object (like Timer / ScoreSystem) so it is awake to catch
// events even while the GamePanel holding the text is hidden.
public class HintSystem : MonoBehaviour
{
    const int LEVEL_COUNT = 5;

    [SerializeField]
    private TMP_Text uiText;

    [Tooltip("Objective for each level, in order (Level 1 first).")]
    [SerializeField, TextArea]
    private string[] objectives =
    {
        "Break the boxes and find the key.",
        "Find your way through the maze.",
        "Defeat the 5 monsters and collect their crystals.",
        "Hit the levers to raise all 4 platforms, then cross the bridge.",
        "Hit the corner levers to reach the center.\nDefeat all 9 monsters and collect their crystals.",
    };

    [SerializeField, TextArea]
    private string levelCompleteText = "Gate open! Go to the next level.";

    private int currentLevel;

    private void Awake()
    {
        if (uiText == null)
            throw new System.NullReferenceException("HintSystem has no uiText");

        SequenceEvents.LevelStarted += OnLevelStarted;
        SequenceEvents.PuzzleSolved += OnPuzzleSolved;
    }

    private void OnDestroy()
    {
        SequenceEvents.LevelStarted -= OnLevelStarted;
        SequenceEvents.PuzzleSolved -= OnPuzzleSolved;
    }

    private void OnLevelStarted(int level)
    {
        currentLevel = level;

        string objective = level >= 1 && level <= objectives.Length ? objectives[level - 1] : "";
        Show(objective);
    }

    private void OnPuzzleSolved()
    {
        // the last puzzle goes straight to the win screen, nothing to point to
        if (currentLevel >= LEVEL_COUNT) return;

        Show(levelCompleteText);
    }

    private void Show(string message)
    {
        uiText.text = $"Level {currentLevel}/{LEVEL_COUNT}\n{message}";
    }
}
