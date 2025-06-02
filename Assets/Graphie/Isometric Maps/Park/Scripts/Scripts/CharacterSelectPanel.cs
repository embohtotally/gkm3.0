using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button[] characterButtons; // Tambahkan referensi ke tombol karakter di panel

    private void Start()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i; // Gunakan variabel lokal untuk memastikan nilai tetap
            characterButtons[i].onClick.AddListener(() => SelectCharacter(index));
        }
    }

    // Fungsi untuk menyimpan indeks karakter yang dipilih
    public void SelectCharacter(int characterIndex)
    {
        PlayerPrefs.SetInt("SelectedCharacterIndex", characterIndex); // Simpan indeks karakter
        PlayerPrefs.Save();
        LoadGameplayScene();
    }

    private void LoadGameplayScene()
    {
        SceneManager.LoadScene("Level 1"); // Pastikan nama scene sesuai
    }
}