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
    [SerializeField] private float jawOpenAngle = -44f;
    [SerializeField] private float jawMoveSpeed = 10f;
    [Tooltip("Minimum mic loudness before mouth starts opening")]
    [SerializeField] private float noiseGate = 0.02f;
    [Tooltip("Loudness value that equals fully open mouth")]
    [SerializeField] private float maxLoudness = 0.15f;

    [SynchronizableField] private bool isTalking = false;
    [SynchronizableField] private float loudness = 0f;

    private AudioClip micClip;
    private Alteruna.Avatar avatar;
    private float currentJawAngle = 0f;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        if (avatar == null)
        {
            Debug.LogError("[TalkingPlayer] No Alteruna.Avatar found!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (avatar.IsMe)
        {
            if (Microphone.devices.Length > 0)
            {
                micClip = Microphone.Start(null, true, 1, 44100);
                Debug.Log("<color=green>[TalkingPlayer] Microphone started.</color>");
            }
            else
            {
                Debug.LogError("<color=red>[TalkingPlayer] No microphone!</color>");
            }
        }

        if (talkIndicator != null)
        {
            talkIndicator.Stop();
            talkIndicator.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Always update visuals (local and remote use the same synced fields)
        UpdateJawAnimation();
        UpdateTalkIndicator();

        if (!avatar.IsMe) return; // Only owner handles input, mic, and loudness sync

        if (micClip == null) return;

        bool shouldTalk = pushToTalk ? Input.GetKey(talkKey) : IsSpeaking();

        if (shouldTalk != isTalking)
        {
            isTalking = shouldTalk;
            Commit();
        }

        loudness = GetMicLoudness();
        Commit(); // Sync loudness frequently while talking
    }

    private bool IsSpeaking()
    {
        return GetMicLoudness() > micSensitivity;
    }

    private float GetMicLoudness()
    {
        float[] data = new float[256];
        int pos = Microphone.GetPosition(null) - data.Length;
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