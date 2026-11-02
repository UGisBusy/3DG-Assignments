using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{

    [SerializeField]
    private TMP_Text uiText; 

    [SerializeField]
    private int score = 0;
    
    [SerializeField]
    private int monsterScore = 100;

    [SerializeField]
    private int levelScore = 200;

    public int Score => score;

    private void Awake()
    {
        if (uiText == null)
            throw new System.NullReferenceException("Timer has no uiText");
        
        GameplayEvents.MonsterDied += onMonsterDied;
        SequenceEvents.CompleteLevel1 += onCompleteLevel;
        SequenceEvents.CompleteLevel2 += onCompleteLevel;
        SequenceEvents.CompleteLevel3 += onCompleteLevel;
        SequenceEvents.CompleteLevel4 += onCompleteLevel;
        SequenceEvents.Win += onCompleteLevel;
    }

    private void OnDestroy()
    {
        GameplayEvents.MonsterDied -= onMonsterDied;
        SequenceEvents.CompleteLevel1 -= onCompleteLevel;
        SequenceEvents.CompleteLevel2 -= onCompleteLevel;
        SequenceEvents.CompleteLevel3 -= onCompleteLevel;
        SequenceEvents.CompleteLevel4 -= onCompleteLevel;
        SequenceEvents.Win -= onCompleteLevel;
    }
    private void Start()
    {
        uiText.text = $"{score}";
    }

    private void onMonsterDied()
    {
        score += monsterScore;
        uiText.text = $"{score}";   
    }

    private void onCompleteLevel()
    {
        score += levelScore;
        uiText.text = $"{score}";
    }
}
