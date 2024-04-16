using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Server
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public string roomName = "Room1";
        public InputField nicknameInput;
        public GameObject disconnectPanel;
        public GameObject respawnPanel;
        
        #region Unity Event Functions
        private void Awake()
        {
    #if UNITY_STANDALONE
            Screen.SetResolution(960, 540, false);
    #endif
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 60;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }
        
        #endregion
        
        #region PunCallbacks
        
        public override void OnConnectedToMaster()
        {
            Debug.Log("마스터 서버 접속 완료");

            PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 6 }, null);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("새로운 방이 생성되었습니다.");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("방에 참가하는 데 성공했습니다.");

            disconnectPanel.SetActive(false);
            Spawn();
        }
 
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"{message}: 방을 만드는 데 실패했습니다.");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"{message}: 방을 참가하는 데 실패했습니다.");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            disconnectPanel.SetActive(true);
            respawnPanel.SetActive(false);
        }

        #endregion
        
        #region Public Methods
        public void Connect()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public void Spawn()
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
            respawnPanel.SetActive(false);
        }

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }

        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2 });
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public void JoinOrCreateRoom()
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 2 }, null);
        }

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion
        
        #region Private Methods
        #endregion
    }

}
