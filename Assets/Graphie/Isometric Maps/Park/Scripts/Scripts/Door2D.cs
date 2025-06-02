using UnityEngine;
using UnityEngine.SceneManagement;

public class Door2D : MonoBehaviour
{
    // Nama scene yang akan dituju
    public string nextScene;

    // Untuk mengetahui apakah kita ingin kembali ke scene sebelumnya
    public bool isBackDoor = false;

    // Fungsi untuk mendeteksi apakah player menyentuh pintu
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Mengecek apakah yang menyentuh pintu adalah player
        if (other.CompareTag("Player"))
        {
            if (isBackDoor)
            {
                // Cek apakah kita punya data untuk kembali ke scene sebelumnya
                if (PlayerPrefs.HasKey("LastScene"))
                {
                    // Ambil nama scene sebelumnya
                    string lastScene = PlayerPrefs.GetString("LastScene");
                    // Pindah ke scene sebelumnya
                    SceneManager.LoadScene(lastScene);
                }
            }
            else
            {
                // Simpan nama scene saat ini ke PlayerPrefs sebagai scene terakhir
                PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
                // Pindah ke scene berikutnya
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}
