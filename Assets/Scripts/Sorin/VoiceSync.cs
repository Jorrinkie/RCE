using UnityEngine;
using Alteruna;

public class VoiceSync : AttributesSync
{
    [SynchronizableField] public bool IsTalking = false;
    [SynchronizableField] public float Loudness = 0f;
}
