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

    private AudioClip micClip;
    private bool isTransmitting = false;

    void Start()
    {
        Debug.Log("<color=yellow>[TalkingPlayer]</color> Initializing microphone...");

        if (Microphone.devices.Length > 0)
        {
            micClip = Microphone.Start(null, true, 1, 44100);
            Debug.Log("<color=green>[TalkingPlayer]</color> Microphone started successfully: " + Microphone.devices[0]);
        }
        else
        {
            Debug.LogError("<color=red>[TalkingPlayer]</color> No microphone detected!");
        }

        if (talkIndicator != null)
        {
            Debug.Log("[TalkingPlayer] Talk indicator found. Hiding it on start.");
            talkIndicator.Stop();
            talkIndicator.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[TalkingPlayer] No talk indicator assigned!");
        }
    }

    void Update()
    {
        if (micClip == null)
        {
            Debug.LogWarning("[TalkingPlayer] No micClip. Mic failed to start?");
            return;
        }

        bool shouldTalk =
            (pushToTalk && Input.GetKey(talkKey)) ||
            (!pushToTalk && IsSpeaking());

        // Debug push-to-talk
        if (pushToTalk && Input.GetKeyDown(talkKey))
            Debug.Log("[TalkingPlayer] Push-to-talk key pressed.");

        if (shouldTalk && !isTransmitting)
            StartTalking();

        else if (!shouldTalk && isTransmitting)
            StopTalking();

        // If transmitting, collect samples
        if (isTransmitting)
        {
            float[] samples = new float[1024];
            int pos = Microphone.GetPosition(null) - samples.Length;

            if (pos < 0)
            {
                Debug.LogWarning("[TalkingPlayer] Mic sample position < 0, skipping.");
                return;
            }

            micClip.GetData(samples, pos);

            Debug.Log("[TalkingPlayer] Sending " + samples.Length + " mic samples to network...");

            // TODO: send samples here
            // VoiceNetwork.SendVoiceData(samples);
        }
    }

    private bool IsSpeaking()
    {
        float[] data = new float[256];
        int pos = Microphone.GetPosition(null) - data.Length;

        if (pos < 0)
            return false;

        micClip.GetData(data, pos);

        float level = 0f;
        for (int i = 0; i < data.Length; i++)
            level += Mathf.Abs(data[i]);

        bool speaking = level > micSensitivity;

        Debug.Log("[TalkingPlayer] Mic level: " + level.ToString("F4") +
                  " | Threshold: " + micSensitivity +
                  " | Speaking: " + speaking);

        return speaking;
    }

    private void StartTalking()
    {
        isTransmitting = true;
        Debug.Log("<color=cyan>[TalkingPlayer] START TALKING</color>");

        if (talkIndicator != null)
        {
            Debug.Log("[TalkingPlayer] Activating talk indicator.");
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
            Debug.Log("[TalkingPlayer] Hiding talk indicator.");
            talkIndicator.Stop();
            talkIndicator.gameObject.SetActive(false);
        }
    }

    public void PlayIncomingVoice(float[] samples)
    {
        if (voiceSource == null)
        {
            Debug.LogError("[TalkingPlayer] VoiceSource missing! Can't play incoming audio.");
            return;
        }

        Debug.Log("[TalkingPlayer] Received incoming voice: " + samples.Length + " samples");

        AudioClip clip = AudioClip.Create("RemoteVoice", samples.Length, 1, 44100, false);
        clip.SetData(samples, 0);

        voiceSource.clip = clip;
        voiceSource.Play();
    }
}