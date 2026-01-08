using UnityEngine;
using Alteruna;


public class NewspaperAppear : AttributesSync
{
    [SynchronizableField] public bool newspaperActive;

    [SerializeField] GameObject newspaper;
    [SerializeField] GameObject hands; 

    private Alteruna.Avatar avatar;

    void Start()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }

    void Update()
    {
        // (Owner only)
        if (avatar.IsMe && Input.GetKeyDown(KeyCode.Mouse1))
        {
            newspaperActive = !newspaperActive;
            Commit();
        }

        //Other Players
        if (!avatar.IsMe)
        {
            newspaper.SetActive(newspaperActive);
            hands.SetActive(!newspaperActive);
        }
        else
        {
            // Local player never sees their own newspaper and hads are back
            newspaper.SetActive(false);
            hands.SetActive(true);
        }
    }
}


