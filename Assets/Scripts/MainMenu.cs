using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public TMP_Text roomName;

    public void PlayTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }
    public void PlayCalibrador()
    {
        SceneManager.LoadScene("CalibradorScene");
    }    
    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void SetDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("difficulty", difficulty);
        PlayerPrefs.Save();
    }

    public void TutorialButton()
    {
        SceneManager.LoadScene("TutorialOnline");
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (roomName.text != "")
        {
            print("Creando sala " + roomName.text);
            RoomOptions roomSetting = new RoomOptions();
            roomSetting.MaxPlayers = 3;
            PhotonNetwork.JoinOrCreateRoom(roomName.text, roomSetting, TypedLobby.Default);
        }
    }
}
