using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineTutorialManager : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage spriteRenderer;
    public TMP_Text textHelp;
    public Sprite sprite1,sprite2,sprite3,sprite4,sprite5, sprite6, sprite7;
    public int temp = 1;

    List<string> text_help = new List<string>() { "Para empezar a jugar online tendremos que hacer click en el botón 'ONLINE' " 
        ,"En el siguiente menu uno de los jugadrores tenrdá que crear una sala"
        ,"Para crear una sala solo tenemos que ingresar el nombre y darle al botón 'CREAR' "
        ,"En esta ventana podremos esperar por los demás jugadores, para después darle al botón 'Empezar' o 'Lobby' "
        ,"En el lobby podremos ver información acerca de nuestas vidas y cuanto sera el puntaje para pasar el nivel"
        ,"Si tus amigos ya crearon un sala entonces tendriamos que hacer click en el botón 'UNIRSE' "
        ,"En esta ventana apareceran las salas que han sido creadas para que puedas unirte"
        ,"En el modo Online hay dos formas de recibir las ordenes:Ordenes Individuales y Ordenes Compartidas"
        ,"Ordenes Individuales: Cada jugador tendra que hacer una orden en la que el marcador o la palabra sean diferentes"
        ,"Ordenes Compartidas: Todos los jugadores recibiran la misma orden y tendrá que ser resuelta por todos"};
    void ChangeSprite(Sprite spriteChange)
    {
        spriteRenderer.texture = spriteChange.texture;
    }

    void Start()
    {
        spriteRenderer.texture = sprite1.texture;
        textHelp.text = text_help[0];
    }

    public void Salir()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void Siguiente()
    {
        switch (temp)
        {
            case 1:
                temp++;
                spriteRenderer.texture = sprite2.texture;
                textHelp.text = text_help[temp-1];
                break;
            case 2:
                temp++;
                spriteRenderer.texture = sprite3.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 3:
                temp++;
                spriteRenderer.texture = sprite4.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 4:
                temp++;
                spriteRenderer.texture = sprite5.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 5:
                temp++;
                spriteRenderer.texture = sprite6.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 6:
                temp++;
                spriteRenderer.texture = sprite7.texture;
                textHelp.text = text_help[temp - 1];
                break;
            default:
                break;
        }
        //Debug.LogError(temp);
        return;
    }

    public void Anterior()
    {
        switch (temp)
        {
            case 2:
                temp--;
                spriteRenderer.texture = sprite1.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 3:
                temp--;
                spriteRenderer.texture = sprite2.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 4:
                temp--;
                spriteRenderer.texture = sprite3.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 5:
                temp--;
                spriteRenderer.texture = sprite4.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 6:
                temp--;
                spriteRenderer.texture = sprite5.texture;
                textHelp.text = text_help[temp - 1];
                break;
            case 7:
                temp--;
                spriteRenderer.texture = sprite6.texture;
                textHelp.text = text_help[temp-1];
                break;
            default:
                break;
        }
        //Debug.LogError(temp);
        return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
