using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorMechanics : MonoBehaviour
{
    public GameObject enteredDoor;
    public GameObject teleportedDoor;
    public GameObject player;
    
    public TMPro.TextMeshProUGUI teleportText;
    private bool isNearDoor;
      
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isNearDoor = false;
        teleportText.text = "Press F to Teleport";
        teleportText.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (isNearDoor)
        {
                        
            if (Input.GetKeyUp(KeyCode.F))
            {
                teleportText.gameObject.SetActive(false);
                Teleport();


                string doorName = enteredDoor.name;
                

                if (doorName == "Door - 1st Floor to Public Records")
                {
                   SceneController.Instance.LoadScene(SceneController.SceneName.PublicRecords);
                }
                else if (doorName == "Door - 1st Floor to DA Office")
                {
                    SceneController.Instance.LoadScene(SceneController.SceneName.DAOffice);
                }
                else if (doorName == "Door - 1st Floor to Attorney Lounge")
                {
                    SceneController.Instance.LoadScene(SceneController.SceneName.AttorneyLounge);
                }
                
            }

        }


    }
    private void Teleport()
    {
        if (enteredDoor != null && teleportedDoor != null && player != null)
        {
            
            player.transform.position = teleportedDoor.transform.position;
            player.transform.hasChanged = true; 
            player.GetComponent<Rigidbody2D>().position = teleportedDoor.transform.position;
            
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
                
        if (collision.CompareTag("Player"))
        {
            isNearDoor = true;
            enteredDoor = gameObject;
            teleportedDoor = gameObject.GetComponent<DoorMechanics>().teleportedDoor;
            
            teleportText.gameObject.SetActive(true);
            teleportText.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + new Vector3(0, -1.5f, 0));
            
        }
        else
        {
            isNearDoor = false;            
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {                            
            isNearDoor = false;
            teleportText.gameObject.SetActive(false);
        }
    }
}

