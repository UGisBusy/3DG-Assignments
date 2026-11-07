using TMPro;
using UnityEngine;

// Keeps and displays the score:
// +100 for each recovered (defeated) monster, +200 for each solved puzzle.
public class ScoreSystem : MonoBehaviour
{

    [SerializeField]
    private TMP_Text uiText;

    [SerializeField]
    private int score = 0;

    [SerializeField]
    private int monsterScore = 100;

    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("levelScore")]
    private int puzzleScore = 200;

    public int Score => score;

    private void Awake()
    {
        if (uiText == null)
            throw new System.NullReferenceException("ScoreSystem has no uiText");

        GameplayEvents.MonsterDied += OnMonsterDied;
        SequenceEvents.PuzzleSolved += OnPuzzleSolved;
    }

    private void OnDestroy()
    {
        GameplayEvents.MonsterDied -= OnMonsterDied;
        SequenceEvents.PuzzleSolved -= OnPuzzleSolved;
    }

    private void Start()
    {
        UpdateText();
    }

    private void OnMonsterDied() => AddScore(monsterScore);

    private void OnPuzzleSolved() => AddScore(puzzleScore);

    private void AddScore(int amount)
    {
        score += amount;
        UpdateText();
    }

    private void UpdateText()
    {
        uiText.text = $"Score: {score}";
    }
}
