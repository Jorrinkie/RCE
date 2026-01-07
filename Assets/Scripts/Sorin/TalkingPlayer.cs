using UnityEngine;
using UnityEngine.Video;
using Alteruna;

public class TalkingPlayer : AttributesSync
{
    [Header("Voice Settings")]
    [SerializeField] private bool pushToTalk = true;
    [SerializeField] private KeyCode talkKey = KeyCode.V;
    [SerializeField] private float micSensitivity = 0.01f;

    [Header("Components")]
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private VideoPlayer talkIndicator;

    [Header("Mouth / Jaw Animation")]
    [SerializeField] private Transform jawBone;
    [SerializeField] private float jawClosedAngle = 0f;
    [SerializeField] private float jawOpenAngle = 30f;
    [SerializeField] private float jawMoveSpeed = -7f;

    [Tooltip("Minimum mic loudness before mouth starts opening")]
    [SerializeField] private float noiseGate = 0.02f;

    [Tooltip("Loudness value that equals fully open mouth")]
    [SerializeField] private float maxLoudness = 0.5f;

    private Alteruna.Avatar avatar;
    private AudioClip micClip;
    private bool isTransmitting = false;
    private float currentJawAngle = 0f;

   
    [SynchronizableField] private bool syncedTalking;
    [SynchronizableField] private float syncedLoudness;

    void Start()
    {
        avatar = GetComponent<Alteruna.Avatar>();

        if (avatar == null)
        {
            Debug.LogError("[TalkingPlayer] No Alteruna.Avatar found!");
            enabled = false;
            return;
        }

        if (talkIndicator != null)
        {
            talkIndicator.Stop();
            talkIndicator.gameObject.SetActive(false);
        }

        // Only local player uses microphone
        if (avatar.IsMe)
        {
            if (Microphone.devices.Length > 0)
            {
                micClip = Microphone.Start(null, true, 1, 44100);
                Debug.Log("[TalkingPlayer] Microphone started");
            }
            else
            {
                Debug.LogError("[TalkingPlayer] No microphone detected!");
            }
        }
    }

    void Update()
    {
        if (avatar.IsMe)
        {
            HandleLocalTalking();
        }
        else
        {
            HandleRemoteTalking();
        }
    }

    // ---------------- LOCAL PLAYER ----------------
    private void HandleLocalTalking()
    {
        if (micClip == null)
            return;

        bool shouldTalk =
            (pushToTalk && Input.GetKey(talkKey)) ||
            (!pushToTalk && IsSpeaking());

        if (shouldTalk && !isTransmitting)
            StartTalking();
        else if (!shouldTalk && isTransmitting)
            StopTalking();

        float loudness = isTransmitting ? GetMicLoudness() : 0f;

     
        syncedTalking = isTransmitting;
        syncedLoudness = loudness;

        UpdateJawFromLoudness(loudness);
    }

    // ---------------- REMOTE PLAYERS ----------------
    private void HandleRemoteTalking()
    {
        if (talkIndicator != null)
        {
            if (syncedTalking && !talkIndicator.isPlaying)
            {
                talkIndicator.gameObject.SetActive(true);
                talkIndicator.Play();
            }
            else if (!syncedTalking && talkIndicator.isPlaying)
            {
                talkIndicator.Stop();
                talkIndicator.gameObject.SetActive(false);
            }
        }

        UpdateJawFromLoudness(syncedLoudness);
    }

    // ---------------- SHARED ----------------
    private bool IsSpeaking()
    {
        return GetMicLoudness() > micSensitivity;
    }

    private float GetMicLoudness()
    {
        float[] data = new float[256];
        int pos = Microphone.GetPosition(null) - data.Length;

        if (pos < 0)
            return 0f;

        micClip.GetData(data, pos);

        float level = 0f;
        for (int i = 0; i < data.Length; i++)
            level += Mathf.Abs(data[i]);

        return level;
    }

    private void StartTalking()
    {
        isTransmitting = true;

        if (talkIndicator != null)
        {
            talkIndicator.gameObject.SetActive(true);
            talkIndicator.Play();
        }
    }

    private void StopTalking()
    {
        isTransmitting = false;

        if (talkIndicator != null)
        {
            talkIndicator.Stop();
            talkIndicator.gameObject.SetActive(false);
        }
    }

    private void UpdateJawFromLoudness(float loudness)
    {
        if (jawBone == null)
            return;

        float targetAngle = jawClosedAngle;

        if (loudness > noiseGate)
        {
            float clean = loudness - noiseGate;
            float t = Mathf.InverseLerp(0f, maxLoudness - noiseGate, clean);
            t = Mathf.Clamp01(t);

            targetAngle = Mathf.Lerp(jawClosedAngle, jawOpenAngle, t);
        }

        currentJawAngle = Mathf.Lerp(
            currentJawAngle,
            targetAngle,
            Time.deltaTime * jawMoveSpeed
        );

        Vector3 rot = jawBone.localEulerAngles;
        rot.y = currentJawAngle;
        jawBone.localEulerAngles = rot;
    }
}
