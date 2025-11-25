using Alteruna;
using UnityEngine;
using TMPro;

public class AvatarNameTag : MonoBehaviour
{
    public TMP_Text nameTagText;

    private Alteruna.Avatar _avatar;

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        InvokeRepeating(nameof(TrySetName), 0.1f, 0.1f);
    }

    void TrySetName()
    {
        if (_avatar != null && _avatar.Owner != null)
        {
            nameTagText.text = _avatar.Owner.Name;
            CancelInvoke(nameof(TrySetName));
        }
    }
}
