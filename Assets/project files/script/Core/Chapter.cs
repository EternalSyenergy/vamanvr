using UnityEngine;
using static OVRPlugin;

public class Chapter : MonoBehaviour
{
    [Header("Chapter Settings")]
    [SerializeField] private string chapterName;

    [Header("Steps")]
    [SerializeField] private Step[] steps;

    private ChapterManager chapterManager;
    private int currentStepIndex = -1;
    private bool isRunning;

    public string ChapterName => chapterName;

    public Step CurrentStep
    {
        get
        {
            if (currentStepIndex < 0 ||
                currentStepIndex >= steps.Length)
                return null;

            return steps[currentStepIndex];
        }
    }

    public void Initialize(ChapterManager manager)
    {
        chapterManager = manager;

        currentStepIndex = -1;
        isRunning = false;

        // Stop all steps first
        if (steps != null)
        {
            foreach (Step step in steps)
            {
                if (step != null)
                {
                    step.StopStep();
                }
            }
        }
    }

    public void StartChapter()
    {
        if (steps == null || steps.Length == 0)
        {
            Debug.LogWarning($"Chapter '{chapterName}' has no steps.");
            CompleteChapter();
            return;
        }

        isRunning = true;
        StartStep(0);
    }

    public void StartStep(int stepIndex)
    {
        if (!isRunning)
            return;

        if (steps == null || steps.Length == 0)
            return;

        if (stepIndex < 0 || stepIndex >= steps.Length)
        {
            CompleteChapter();
            return;
        }

        // Stop previous step
        if (CurrentStep != null)
        {
            CurrentStep.StopStep();
        }

        currentStepIndex = stepIndex;

        Step step = steps[currentStepIndex];

        if (step == null)
        {
            StartNextStep();
            return;
        }

        step.Initialize(this);
        step.StartStep();
    }

    public void StartNextStep()
    {
        int nextStep = currentStepIndex + 1;

        if (nextStep >= steps.Length)
        {
            CompleteChapter();
            return;
        }

        StartStep(nextStep);
    }

    private void CompleteChapter()
    {
        if (!isRunning)
            return;

        isRunning = false;

        Debug.Log($"Chapter '{chapterName}' completed.");

        if (chapterManager != null)
        {
            chapterManager.StartNextChapter();
        }
    }

    public void StopChapter()
    {
        isRunning = false;

        if (CurrentStep != null)
        {
            CurrentStep.StopStep();
        }

        currentStepIndex = -1;
    }
}