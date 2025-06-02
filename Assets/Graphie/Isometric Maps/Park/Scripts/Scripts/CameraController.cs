using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Target transform yang akan diikuti kamera (yaitu karakter player)
    private Transform target;

    // Kecepatan kamera mengikuti target, bisa disesuaikan di Inspector
    [SerializeField] private float smoothSpeed = 0.125f;

    // Fungsi Start() akan dipanggil saat pertama kali script aktif
    private void Start()
    {
        // Memanggil fungsi untuk mencari GameObject yang memiliki tag "Player"
        // dan menyetelnya sebagai target kamera
        FindPlayer();
    }

    // Fungsi Update() dipanggil setiap frame
    private void Update()
    {
        // Jika target belum ditemukan, coba cari lagi
        if (target == null)
        {
            FindPlayer();
        }

        // Jika target ditemukan, lakukan Lerp untuk menggerakkan kamera mengikuti target
        if (target != null)
        {
            // Perpindahan kamera menggunakan Lerp untuk membuat pergerakan halus (smooth)
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(target.position.x, target.position.y, transform.position.z), 
                smoothSpeed * Time.deltaTime);
        }
    }

    // Fungsi untuk mencari GameObject yang memiliki tag "Player" dan menetapkannya sebagai target
    private void FindPlayer()
    {
        // Mencari GameObject dengan tag "Player" di scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Jika player ditemukan, ambil komponen transform-nya dan tetapkan sebagai target
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            // Jika player belum ditemukan, tampilkan pesan peringatan di Console
            Debug.LogWarning("Player tidak ditemukan! Pastikan objek player memiliki tag 'Player'.");
        }
    }
}
