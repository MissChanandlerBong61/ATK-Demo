    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ElevatorControls : MonoBehaviour
    {
        [SerializeField] private List<string> unavailableFloors = new List<string> { "14", "15" };
        private Dictionary<string, Transform> floorTargets = new Dictionary<string, Transform>();
       
        public GameObject player;
        public TextMeshPro elevatorText;

        [Header("Floor Settings")]
        public string currentFloor;

        private UIController uiController;
        private bool isNearElevator = false;
        private VisualElement elevatorUI;
        private Label warningText;
        


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.isGamePaused)
                return;

            for (int i = 0; i < 20; i++)
            {
                string name = i + "FloorElevator";
                GameObject targetObj = GameObject.Find(name);
                if (targetObj != null)
                {
                    floorTargets[i.ToString()] = targetObj.transform;
                }
                foreach (var key in floorTargets.Keys)
                    Debug.Log("Cached floor target: " + key);
            }

            uiController = FindAnyObjectByType<UIController>();
            if (uiController != null)
            {
                VisualElement root = uiController.uiDocument.rootVisualElement;            
                elevatorUI = root.Q<VisualElement>("ElevatorUI");
                warningText = root.Q<Label>("ElevatorWarningText");
                if (elevatorUI != null)
                    elevatorUI.style.display = DisplayStyle.None;

                var buttons = elevatorUI.Query<Button>().ToList();
                foreach (var button in buttons)
                {
                    if (!button.name.EndsWith("FloorButton"))
                        continue;

                string displayName = button.text.Trim().ToUpperInvariant();    
                string floorNum = button.name.Replace("thFloorButton", "");



                    if (floorNum == currentFloor)
                    {
                        button.SetEnabled(false);
                        continue;
                    }


                    bool available = !unavailableFloors.Contains(floorNum);
                    button.SetEnabled(available);
                    if (available)
                    {
                        button.style.backgroundColor = new StyleColor(Color.black);
                        button.clicked += () => GoToFloor(floorNum);
                    }
                    else
                    {
                        button.style.backgroundColor = new StyleColor(Color.red);
                        button.clicked += () => ShowElevatorWarning();
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isNearElevator && Input.GetKeyDown(KeyCode.F))
            {
                if (elevatorUI != null)
                {
                    elevatorUI.style.display = DisplayStyle.Flex;
                    GameManager.Instance?.PauseGame(PauseSource.UI);
                }
            }

            if (elevatorUI != null && elevatorUI.style.display == DisplayStyle.Flex)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    elevatorUI.style.display = DisplayStyle.None;
                    GameManager.Instance?.ResumeGame();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Something is in the elevator trigger: " + collision.name);
            if (collision.CompareTag("Player"))
            {
                isNearElevator = true;
                elevatorText.gameObject.SetActive(true);
                Debug.Log("Player is near the elevator. Press F to interact.");
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isNearElevator = false;
                elevatorText.gameObject.SetActive(false);
                if (elevatorUI != null)
                    elevatorUI.style.display = DisplayStyle.None;            
            }
        }

        private void ShowElevatorWarning()
        {
            warningText.style.display = DisplayStyle.Flex;

            CancelInvoke(nameof(HideElevatorWarning));
            Invoke(nameof(HideElevatorWarning), 2f);
        }
        private void HideElevatorWarning()
        {
            if (warningText != null)
                warningText.style.display = DisplayStyle.None;
        }
        private void GoToFloor(string floorNum)
        {
            if (!floorTargets.TryGetValue(floorNum, out var target))
            {
                ShowElevatorWarning();
                Debug.LogWarning($"No GameObject named '{floorNum}thFloorElevator' found in scene.");
                return;
            }

            if (player != null)
            {
                Debug.Log($"Teleporting player to floor {floorNum}");
                player.transform.position = target.position;
                elevatorUI.style.display = DisplayStyle.None;
                GameManager.Instance?.ResumeGame();
            }
        }
    }
