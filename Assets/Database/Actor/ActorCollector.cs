using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActorCollector : MonoBehaviour
{
    [System.Serializable]
    public class FullActorData
    {
        public string name;
        public Sprite avatar;
        public Sprite poster;
        public AudioClip voice;
        public GameObject actorGameObject;
    }

    public Transform acterRoot; // корень всех актёров
    public TMP_Dropdown selectActorBX;
    public Sprite defaultAvatar;
    public Sprite defaultPoster;
    public AudioClip defaultAudio;

    private List<FullActorData> collectedActors = new List<FullActorData>();

    void Start()
    {
        CollectActors();
        PopulateDropdown();
    }

    void CollectActors()
    {
        collectedActors.Clear();

        foreach (Transform child in acterRoot)
        {
            var manager = child.Find("UI/Manager");
            if (manager == null) continue;

            var profileSprite = manager.Find("Profile")?.GetComponent<SpriteRenderer>();
            var posterSprite = manager.Find("Poster")?.GetComponent<SpriteRenderer>();
            var audioSource = manager.Find("Audio")?.GetComponent<AudioSource>();

            FullActorData data = new FullActorData
            {
                name = child.name,
                avatar = profileSprite != null ? profileSprite.sprite : defaultAvatar,
                poster = posterSprite != null ? posterSprite.sprite : defaultPoster,
                voice = audioSource != null ? audioSource.clip : defaultAudio,
                actorGameObject = child.gameObject
            };

            collectedActors.Add(data);
        }
    }

    void PopulateDropdown()
    {
        selectActorBX.ClearOptions();

        List<string> options = new List<string>();
        foreach (var actor in collectedActors)
        {
            options.Add(actor.name);
        }

        selectActorBX.AddOptions(options);
    }

    // Получение всех актёров
    public List<FullActorData> GetAllActors()
    {
        return collectedActors;
    }

    // Получение данных по имени
    public FullActorData GetActorData(string name)
    {
        return collectedActors.Find(actor => actor.name == name);
    }

    // Получение GameObject по имени
    public GameObject GetActor(string name)
    {
        var data = GetActorData(name);
        return data != null ? data.actorGameObject : null;
    }
}
