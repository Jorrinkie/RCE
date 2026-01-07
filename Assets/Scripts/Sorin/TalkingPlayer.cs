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
    [SerializeField] private AudioSource localMonitorSource; // NEW: Assign this for local mic test (volume 0.5)

    [Header("Mouth / Jaw Animation")]
    [SerializeField] private Transform jawBone;
    [SerializeField] private float jawClosedAngle = 0f;
    [SerializeField] private float jawOpenAngle = -44f;
    [SerializeField] private float jawMoveSpeed = 10f;
    [Tooltip("Minimum mic loudness before mouth starts opening")]
    [SerializeField] private float noiseGate = 0.005f; // Lowered for testing
    [Tooltip("Loudness value that equals fully open mouth")]
    [SerializeField] private float maxLoudness = 0.15f;

    [SynchronizableField] private bool isTalking = false;
    [SynchronizableField] private float loudness = 0f;

    // For optimized syncing
    private float lastLoudness = -1f;
    private bool lastIsTalking = false;

    private Alteruna.Avatar avatar;
    private VoiceSynchronizable voiceSync;
    private float currentJawAngle = 0f;
    private AudioClip micClip; // For local monitor

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        if (avatar == null)
        {
            Debug.LogError("[TalkingPlayer] No Alteruna.Avatar found!");
            enabled = false;
            return;
        }

        voiceSync = GetComponent<VoiceSynchronizable>();
        if (voiceSync == null)
        {
            Debug.LogError("[TalkingPlayer] No VoiceSynchronizable component found! Add it to the prefab.");
            enabled = false;
            return;
        }

        if (voiceSource != null)
        {
            voiceSync.PlaybackSource = voiceSource;
        }
    }

    private void Start()
    {
        if (talkIndicator != null)
        {
            talkIndicator.Stop();
            talkIndicator.gameObject.SetActive(false);
        }

        if (avatar.IsMe)
        {
            Debug.Log("[TalkingPlayer] Available mics: " + string.Join(", ", Microphone.devices));

            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("[TalkingPlayer] NO MICROPHONE DETECTED! Check system settings.");
            }

            if (!pushToTalk)
            {
                VoiceSynchronizable.SetDevice("Microphone Array (Realtek(R) Audio)");
                Debug.Log("[TalkingPlayer] Microphone started (always-on mode with real mic).");
            }
        }
    }

    private void Update()
    {
        UpdateJawAnimation();
        UpdateTalkIndicator();

        if (!avatar.IsMe) return;

        // Debug: Core voice status every frame
        Debug.Log("[Voice Debug] IsActive: " + voiceSync.IsActive +
                  " | PeakVolume: " + voiceSync.PeakVolume.ToString("F4") +
                  " | IsTalking: " + isTalking +
                  " | OldLoudness: " + GetMicLoudness().ToString("F4")); // NEW: Compare to old calculation

        bool shouldTalk = false;
        if (pushToTalk)
        {
            bool keyHeld = Input.GetKey(talkKey);
            shouldTalk = keyHeld;

            if (keyHeld && !voiceSync.IsActive)
            {
                VoiceSynchronizable.SetDevice("Microphone Array (Realtek(R) Audio)");
                Debug.Log("[TalkingPlayer] PTT Key pressed -> Microphone STARTED (real mic)");

                // NEW: Start local monitor
                if (localMonitorSource != null)
                {
                    micClip = Microphone.Start("Microphone Array (Realtek(R) Audio)", true, 1, 44100);
                    localMonitorSource.clip = micClip;
                    localMonitorSource.loop = true;
                    while (Microphone.GetPosition("Microphone Array (Realtek(R) Audio)") <= 0) { }
                    localMonitorSource.Play();
                    Debug.Log("[TalkingPlayer] Local mic monitor started - you should hear yourself!");
                }
            }
            else if (!keyHeld && voiceSync.IsActive)
            {
                VoiceSynchronizable.ClearDevice();
                Debug.Log("[TalkingPlayer] PTT Key released -> Microphone STOPPED");

                // NEW: Stop local monitor
                if (localMonitorSource != null)
                {
                    localMonitorSource.Stop();
                    Microphone.End("Microphone Array (Realtek(R) Audio)");
                    Debug.Log("[TalkingPlayer] Local mic monitor stopped");
                }
            }
        }
        else
        {
            shouldTalk = voiceSync.PeakVolume > micSensitivity;
        }

        bool needsCommit = false;

        if (shouldTalk != isTalking)
        {
            isTalking = shouldTalk;
            Debug.Log("[TalkingPlayer] isTalking changed to: " + isTalking);
            needsCommit = true;
        }

        float currentPeak = voiceSync.PeakVolume;
        loudness = currentPeak;

        if (Mathf.Abs(loudness - lastLoudness) > 0.001f)
        {
            lastLoudness = loudness;
            needsCommit = true;
        }

        if (voiceSync.IsActive && currentPeak < 0.001f)
        {
            Debug.LogWarning("[Voice Debug] Mic ACTIVE but PeakVolume near zero - speak louder or check mic!");
        }
        else if (voiceSync.IsActive && currentPeak > 0.05f)
        {
            Debug.Log("[Voice Debug] Good volume detected - audio should be transmitting!");
        }

        if (needsCommit)
        {
            Commit();
        }
    }

    // NEW: Your old loudness function for comparison debug
    private float GetMicLoudness()
    {
        if (micClip == null) return 0f;
        float[] data = new float[256];
        int pos = Microphone.GetPosition("Microphone Array (Realtek(R) Audio)") - data.Length;
        if (pos < 0) return 0f;
        micClip.GetData(data, pos);
        float level = 0f;
        for (int i = 0; i < data.Length; i++)
        {
            level += Mathf.Abs(data[i]);
        }
        return level / data.Length;
    }

    private void UpdateJawAnimation()
    {
        if (jawBone == null) return;

        float targetAngle = jawClosedAngle;

        if (isTalking && loudness > noiseGate)
        {
            float cleanLoudness = loudness - noiseGate;
            float t = Mathf.InverseLerp(0f, maxLoudness - noiseGate, cleanLoudness);
            t = Mathf.Clamp01(t);
            targetAngle = Mathf.Lerp(jawClosedAngle, jawOpenAngle, t);
        }

        currentJawAngle = Mathf.Lerp(currentJawAngle, targetAngle, Time.deltaTime * jawMoveSpeed);

        Vector3 rot = jawBone.localEulerAngles;
        rot.y = currentJawAngle;
        jawBone.localEulerAngles = rot;
    }

    private void UpdateTalkIndicator()
    {
        if (talkIndicator == null) return;

        if (isTalking)
        {
            if (!talkIndicator.gameObject.activeSelf)
            {
                talkIndicator.gameObject.SetActive(true);
                talkIndicator.Play();
            }
        }
        else
        {
            if (talkIndicator.gameObject.activeSelf)
            {
                talkIndicator.Stop();
                talkIndicator.gameObject.SetActive(false);
            }
        }
    }
}