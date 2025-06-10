using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class CoonectToServer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Menu");
    }
}
