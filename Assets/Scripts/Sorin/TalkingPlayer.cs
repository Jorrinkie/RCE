using UnityEngine;
using UnityEngine.Video;

public class TalkingPlayer : MonoBehaviour
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

    private AudioClip micClip;
    private bool isTransmitting = false;
    private float currentJawAngle = 0f;

    void Start()
    {
        Debug.Log("<color=yellow>[TalkingPlayer]</color> Initializing microphone...");

        if (Microphone.devices.Length > 0)
        {
            micClip = Microphone.Start(null, true, 1, 44100);
            Debug.Log("<color=green>[TalkingPlayer]</color> Microphone started: " + Microphone.devices[0]);
        }
        else
        {
            Debug.LogError("<color=red>[TalkingPlayer]</color> No microphone detected!");
        }

        if (talkIndicator != null)
        {
            talkIndicator.Stop();
            talkIndicator.gameObject.SetActive(false);
        }
    }

    void Update()
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

        if (isTransmitting)
        {
            float[] samples = new float[1024];
            int pos = Microphone.GetPosition(null) - samples.Length;

            if (pos >= 0)
            {
                micClip.GetData(samples, pos);

                // TODO: Send samples over network
                // VoiceNetwork.SendVoiceData(samples);
            }
        }

        UpdateJawMovement();
    }

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
        Debug.Log("<color=cyan>[TalkingPlayer] START TALKING</color>");

        if (talkIndicator != null)
        {
            talkIndicator.gameObject.SetActive(true);
            talkIndicator.Play();
        }
    }

    private void StopTalking()
    {
        isTransmitting = false;
        Debug.Log("<color=cyan>[TalkingPlayer] STOP TALKING</color>");

        if (talkIndicator != null)
        {
            talkIndicator.Stop();
            talkIndicator.gameObject.SetActive(false);
        }
    }

    private void UpdateJawMovement()
    {
        if (jawBone == null)
            return;

        float targetAngle = jawClosedAngle;

        if (isTransmitting)
        {
            float rawLoudness = GetMicLoudness();

            if (rawLoudness > noiseGate)
            {
                float cleanLoudness = rawLoudness - noiseGate;
                float t = Mathf.InverseLerp(0f, maxLoudness - noiseGate, cleanLoudness);
                t = Mathf.Clamp01(t);

                targetAngle = Mathf.Lerp(jawClosedAngle, jawOpenAngle, t);
            }
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

    public void PlayIncomingVoice(float[] samples)
    {
        if (voiceSource == null)
            return;

        AudioClip clip = AudioClip.Create("RemoteVoice", samples.Length, 1, 44100, false);
        clip.SetData(samples, 0);

        voiceSource.clip = clip;
        voiceSource.Play();
    }
}