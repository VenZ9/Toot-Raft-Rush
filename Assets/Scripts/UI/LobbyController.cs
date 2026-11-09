using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TootRaftRush.Multiplayer;

namespace TootRaftRush.UI
{
    public class LobbyController : MonoBehaviour
    {
        [Header("Room UI")]
        [SerializeField] private InputField roomCodeInput;
        [SerializeField] private Text activeRoomCodeText;
        [SerializeField] private Text statusText;

        [Header("Player Slots (Up to 4 Players)")]
        [SerializeField] private Text[] playerSlotNames;
        [SerializeField] private Text[] playerSlotStatuses;

        [Header("Buttons")]
        [SerializeField] private Button readyButton;
        [SerializeField] private Button startMatchButton;
        [SerializeField] private Text readyButtonText;

        private bool isReady = false;

        private void Start()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnRoomJoined += HandleRoomJoined;
                NetworkManager.Instance.OnPlayerListUpdated += UpdatePlayerList;
                NetworkManager.Instance.OnStatusChanged += UpdateStatus;
            }

            // Generate initial room code if hosting
            if (activeRoomCodeText != null)
            {
                activeRoomCodeText.text = "ROOM: " + GenerateRoomCode();
            }

            UpdateStatus("Ready to create or join online room.");
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            char[] code = new char[4];
            for (int i = 0; i < 4; i++)
            {
                code[i] = chars[Random.Range(0, chars.Length)];
            }
            return new string(code);
        }

        public void OnCreateRoomClicked()
        {
            string code = GenerateRoomCode();
            if (activeRoomCodeText != null)
            {
                activeRoomCodeText.text = "ROOM: " + code;
            }
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.CreateRoom(code);
            }
            else
            {
                UpdateStatus($"Created Room {code} (Offline / Host Mode)");
                SetSlot(0, "You (Host)", "Ready");
                SetSlot(1, "AI Companion 1", "Ready");
                SetSlot(2, "AI Companion 2", "Ready");
                SetSlot(3, "AI Companion 3", "Ready");
            }
        }

        public void OnJoinRoomClicked()
        {
            string code = roomCodeInput != null ? roomCodeInput.text.Trim().ToUpper() : "RUSH";
            if (string.IsNullOrEmpty(code))
            {
                UpdateStatus("Please enter a 4-letter room code!");
                return;
            }

            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.JoinRoom(code);
            }
            else
            {
                UpdateStatus($"Joined Room {code} (Offline Sync Mode)");
                SetSlot(0, "Player 1 (Host)", "Ready");
                SetSlot(1, "You (Client)", "Ready");
                SetSlot(2, "AI Companion", "Ready");
                SetSlot(3, "AI Companion", "Ready");
            }
        }

        public void OnToggleReadyClicked()
        {
            isReady = !isReady;
            if (readyButtonText != null)
            {
                readyButtonText.text = isReady ? "UNREADY" : "READY!";
            }
            UpdateStatus(isReady ? "You are READY!" : "Waiting for all players to be ready...");
        }

        public void OnStartMatchClicked()
        {
            SceneManager.LoadScene("SunnySlide");
        }

        public void OnBackClicked()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void HandleRoomJoined(string roomCode)
        {
            if (activeRoomCodeText != null)
            {
                activeRoomCodeText.text = "ROOM: " + roomCode;
            }
            UpdateStatus($"Connected to Room {roomCode}!");
        }

        private void UpdatePlayerList(string[] names, bool[] readies)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i < names.Length && !string.IsNullOrEmpty(names[i]))
                {
                    SetSlot(i, names[i], readies[i] ? "Ready" : "Waiting");
                }
                else
                {
                    SetSlot(i, $"[Slot {i + 1} Open]", "Waiting");
                }
            }
        }

        private void SetSlot(int slotIndex, string name, string status)
        {
            if (playerSlotNames != null && slotIndex < playerSlotNames.Length && playerSlotNames[slotIndex] != null)
            {
                playerSlotNames[slotIndex].text = name;
            }
            if (playerSlotStatuses != null && slotIndex < playerSlotStatuses.Length && playerSlotStatuses[slotIndex] != null)
            {
                playerSlotStatuses[slotIndex].text = status;
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
    }
}
