using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class StageSummaryController : MonoBehaviour
{
    [SerializeField]
    private int stage;

    [SerializeField]
    GameLobbyController gameController;

    [SerializeField]
    TMP_Text stageDescription;

    [SerializeField]
    Button button;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void ButtonOnCLick()
    {

        if (gameController.GetStageMinimunScore(stage) >= 0)
        {
            stageDescription.text = "Puntaje Mínimo: " + gameController.GetStageMinimunScore(stage).ToString() + "\n\n" +
                                "Puntaje Obtenido: " + gameController.GetStageLastScore(stage).ToString() + "\n\n" +
                                "Rondas: " + gameController.GetStageRounds(stage).ToString() + "\n\n" +
                                "Dificultad: " + gameController.GetStageDifficulty(stage).ToString();
        }
        else
        {
            stageDescription.text = "Aún no se ha obtenido la información del servidor";
        }
    }

    public void UpdateButtonColor()
    {

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Stage"))
        {
            int currentStage = (int)PhotonNetwork.CurrentRoom.CustomProperties["Stage"];
            Debug.Log("Stage actual: "+currentStage.ToString());
            if(currentStage == stage)
            {
                button.image.color = new Color(0.0f, 0.8f, 0.0f);
            }
            else if(currentStage > stage)
            {
                button.image.color = new Color(0.8f, 0.8f, 0.0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
