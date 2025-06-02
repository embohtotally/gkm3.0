using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs;  // Array untuk menyimpan semua prefab karakter

    private void Start()
    {
        LoadSelectedCharacter();
    }

    private void LoadSelectedCharacter()
    {
        // Ambil indeks karakter yang dipilih dari PlayerPrefs
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        
        // Pastikan indeks sesuai dengan array prefab yang diinspector
        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterPrefabs.Length)
        {
            GameObject characterPrefab = characterPrefabs[selectedCharacterIndex];
            
            if (characterPrefab != null)
            {
                Instantiate(characterPrefab, Vector3.zero, Quaternion.identity); // Spawn karakter di posisi awal
                Debug.Log("Character Loaded: " + characterPrefab.name);
            }
            else
            {
                Debug.LogWarning("Selected character prefab is missing.");
            }
        }
        else
        {
            Debug.LogError("Character index is out of range or not set.");
        }
    }
}