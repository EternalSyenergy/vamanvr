using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Step : MonoBehaviour
{
    [Header("Step")]
    [SerializeField] private string stepName;

    [Header("Content")]
    [Tooltip("GameObjects that belong to this step.")]
    [SerializeField] private GameObject[] contents;

    [Header("Audio")]
    [Tooltip("Audio clips played sequentially when the step starts.")]
    [SerializeField] private AudioClip[] audios;


    [Header("Auto Complete")]
    [SerializeField] private bool autoComplete = false;

    [Min(0f)]
    [SerializeField] private float autoCompleteDelay = 5f;

    [Header("Events")]
    [SerializeField] private UnityEvent onStart;

    [SerializeField] private UnityEvent onComplete;

    private Chapter chapter;

    private Coroutine autoCompleteCoroutine;
    private Coroutine audioCoroutine;

    private bool isStarted;
    private bool isCompleted;

    public string StepName => stepName;

    public bool IsCompleted => isCompleted;

    public void Initialize(Chapter parentChapter)
    {
        chapter = parentChapter;

        isStarted = false;
        isCompleted = false;

        StopStep();

        // Reset content
        SetContents(false);
    }

    public void StartStep()
    {
        if (isStarted)
            return;

        isStarted = true;
        isCompleted = false;

        Debug.Log($"Starting Step: {stepName}");

        // Show this step's content
        SetContents(true);

        // Run Inspector OnStart events
        onStart?.Invoke();

        // Start audio
        if (audios != null &&
            audios.Length > 0 &&
            GameManager.instance.audioManager.contentAudio != null)
        {
            audioCoroutine = StartCoroutine(PlayAudios());
        }

        // Start automatic completion
        if (autoComplete)
        {
            autoCompleteCoroutine =
                StartCoroutine(AutoCompleteRoutine());
        }
    }

    private IEnumerator AutoCompleteRoutine()
    {
        yield return new WaitForSeconds(autoCompleteDelay);

        // Manual completion may have happened already
        if (!isCompleted)
        {
            CompleteStep();
        }
    }

    private IEnumerator PlayAudios()
    {
        //foreach (AudioClip clip in audios)
        //{
        //    if (clip == null)
        //        continue;

        //    GameManager.instance.audioManager.contentAudio.clip = clip;
        //    GameManager.instance.audioManager.contentAudio.Play();

        //    yield return new WaitForSeconds(clip.length);
        //}

        int audioIndex = GameManager.instance.language;

        if (audioIndex >= 0 && audioIndex < audios.Length)
        {
            AudioClip clip = audios[audioIndex];

            if (clip != null)
            {
                GameManager.instance.audioManager.contentAudio.clip = clip;
                GameManager.instance.audioManager.contentAudio.Play();

                yield return new WaitForSeconds(clip.length);
            }
        }
    }

    // =========================================================
    // MANUAL COMPLETION
    // =========================================================

    public void CompleteStep()
    {
        if (!isStarted)
        {
            Debug.LogWarning(
                $"Cannot complete Step '{stepName}' because it has not started."
            );

            return;
        }

        // Prevent double completion
        if (isCompleted)
            return;

        isCompleted = true;

        Debug.Log($"Completed Step: {stepName}");

        // Stop automatic timer
        if (autoCompleteCoroutine != null)
        {
            StopCoroutine(autoCompleteCoroutine);
            autoCompleteCoroutine = null;
        }

        // Stop audio
        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }

        if (GameManager.instance.audioManager.contentAudio != null)
        {
            GameManager.instance.audioManager.contentAudio.Stop();
        }

        // Run OnComplete
        onComplete?.Invoke();

        // Hide this step's content
        SetContents(false);

        // Tell Chapter to start next step
        if (chapter != null)
        {
            chapter.StartNextStep();
        }
    }

    // =========================================================
    // CONTENT
    // =========================================================

    private void SetContents(bool state)
    {
        if (contents == null)
            return;

        foreach (GameObject content in contents)
        {
            if (content != null)
            {
                content.SetActive(state);
            }
        }
    }

    // =========================================================
    // STOP
    // =========================================================

    public void StopStep()
    {
        if (autoCompleteCoroutine != null)
        {
            StopCoroutine(autoCompleteCoroutine);
            autoCompleteCoroutine = null;
        }

        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }

        if (GameManager.instance.audioManager.contentAudio != null)
        {
            GameManager.instance.audioManager.contentAudio.Stop();
        }

        isStarted = false;
        isCompleted = false;
    }
}