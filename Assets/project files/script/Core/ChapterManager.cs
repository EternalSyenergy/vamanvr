using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    [Header("Chapters")]
    [SerializeField] private Chapter[] chapters;


    [Header("Current Running")]
    [SerializeField] private Chapter currentChapterPlay;

    [SerializeField] private Step currentStepPlay;


    private int currentChapterIndex = -1;
    private bool isRunning;

    public Chapter CurrentChapter
    {
        get
        {
            if (currentChapterIndex < 0 ||
                currentChapterIndex >= chapters.Length)
                return null;

            return chapters[currentChapterIndex];
        }
    }

    private void Start()
    {
        StartChapter(0);
    }

    public void StartChapter(int chapterIndex)
    {
        if (chapters == null || chapters.Length == 0)
        {
            Debug.LogWarning("No chapters configured.");
            return;
        }

        if (chapterIndex < 0 || chapterIndex >= chapters.Length)
        {
            Debug.LogWarning($"Invalid chapter index: {chapterIndex}");
            return;
        }

        // Stop previous chapter
        if (CurrentChapter != null)
        {
            CurrentChapter.StopChapter();
        }


        currentChapterIndex = chapterIndex;
        isRunning = true;

        chapters[currentChapterIndex].Initialize(this);
        chapters[currentChapterIndex].StartChapter();


        currentChapterPlay = chapters[currentChapterIndex];
        currentStepPlay = currentChapterPlay.CurrentStep;
    }

    public void StartNextChapter()
    {
        int nextChapter = currentChapterIndex + 1;

        if (nextChapter >= chapters.Length)
        {
            CompleteAllChapters();
            return;
        }

        StartChapter(nextChapter);
    }

    private void CompleteAllChapters()
    {
        isRunning = false;

        Debug.Log("All chapters completed!");
    }

    public void StopCurrentChapter()
    {
        if (CurrentChapter != null)
        {
            CurrentChapter.StopChapter();
        }

        isRunning = false;
    }
}