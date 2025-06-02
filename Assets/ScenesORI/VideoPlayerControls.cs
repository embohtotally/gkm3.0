using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoPlayerControls : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float speedIncrement = 1.0f; // Amount to increase playback speed by

    private float originalPlaybackSpeed = 1.0f;

    void Start()
    {
        // Ensure videoPlayer is assigned
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Set the videoPlayer's original speed
        originalPlaybackSpeed = videoPlayer.playbackSpeed;

        // Subscribe to video end event
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    // Method to increase video playback speed
    public void SpeedUpVideo()
    {
        videoPlayer.playbackSpeed += speedIncrement;
    }

    // Method to skip the video and load the next scene
    public void SkipVideo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Triggered when the video ends
    private void OnVideoEnd(VideoPlayer vp)
    {
        // Load the next scene when the video finishes
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
