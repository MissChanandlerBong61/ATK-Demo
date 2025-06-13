using Unity.VisualScripting;
using UnityEngine;

public class JailDoorCheck : MonoBehaviour
{
    public GameObject[] criminals;
    public GameObject collusionText;

    private bool isClose = false;
    private string pickedClient;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        collusionText.SetActive(false);
        pickedClient = PlayerPrefs.GetString("PickedClient", "DefaultClient");

        if (criminals == null || criminals.Length == 0)
        {
            Debug.LogWarning("No criminals assigned to the JailDoorCheck script.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isClose && Input.GetKeyDown(KeyCode.E))
        {
            collusionText.SetActive(false);
            isClose = false;
            // Check if the picked client exists in the criminals array
            foreach (GameObject criminal in criminals)
            {
                
            }
            // If not found, log a warning
            Debug.LogWarning($"Criminal {pickedClient} not found in the assigned criminals.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           collusionText.SetActive(true);
            isClose = true;
        }

    }
}
