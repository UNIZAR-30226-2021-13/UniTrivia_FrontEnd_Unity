using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ShopBehaviourScript : MonoBehaviour
{
    public Button AvatarShopButton;
    public Button BannerShopButton;
    public Button TokenShopButton;
    public Image ObjectImage;
    public Button LeftButton;
    public Button RightButton;
    public Button BuyButton;
    public Button ActiveButton;

    private int index = 0;
    private int maxIndex = 5;
    private string path = "Avatar/";
    private string objectName = "avatar";

    // Start is called before the first frame update
    void Start()
    {
        LeftButton.onClick.AddListener(LeftButtonOnClick);
        RightButton.onClick.AddListener(RightButtonOnClick);
        AvatarShopButton.onClick.AddListener(AvatarShopButtonOnClick);
        BannerShopButton.onClick.AddListener(BannerShopButtonOnClick);
        TokenShopButton.onClick.AddListener(TokenShopButtonOnClick);

        ObjectImage.sprite = Resources.Load<Sprite>(path + objectName + index);
        checkActualItem();
    }

    void AvatarShopButtonOnClick()
    {
        path = "Avatar/";
        objectName = "avatar";
        index = 0;
        maxIndex = 5;
        ObjectImage.sprite = Resources.Load<Sprite>(path + objectName + index);
        checkActualItem();
    }

    void BannerShopButtonOnClick()
    {
        path = "Banner/";
        objectName = "banner";
        index = 0;
        maxIndex = 5;
        ObjectImage.sprite = Resources.Load<Sprite>(path + objectName + index);
        checkActualItem();
    }

    void TokenShopButtonOnClick()
    {
        path = "Token/";
        objectName = "ficha";
        index = 0;
        maxIndex = 3;
        ObjectImage.sprite = Resources.Load<Sprite>(path + objectName + index);
        checkActualItem();
    }

    void LeftButtonOnClick()
    {
        if(index <= 0)
        {
            index = maxIndex;
        } else
        {
            index--;
        }
        ObjectImage.sprite = Resources.Load<Sprite>(path + objectName + index);
        checkActualItem();
    }

    void RightButtonOnClick()
    {
        if (index >= maxIndex)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        ObjectImage.sprite = Resources.Load<Sprite>(path + objectName + index);
        checkActualItem();
    }

    void BuyButtonOnClick()
    {

    }

    void ActiveButtonOnClick()
    {
        StartCoroutine(ActiveRequest(objectName + index, objectName));
    }

    void checkActualItem()
    {
        if(UserDataScript.isItem(objectName + index))
        {
            BuyButton.interactable = false;
            BuyButton.GetComponentInChildren<Text>().text = "EN POSESIÓN";

            if (UserDataScript.getInfo(objectName).Equals(objectName + index))
            {
                ActiveButton.interactable = true;
                ActiveButton.GetComponentInChildren<Text>().text = "ACTIVAR";
            }
            else
            {
                ActiveButton.interactable = false;
                ActiveButton.GetComponentInChildren<Text>().text = "ACTIVADO";
            }
            
        } else
        {
            if(UserDataScript.getCoins() < 100)
            {
                BuyButton.interactable = false;
                BuyButton.GetComponentInChildren<Text>().text = "COMPRAR | 100c";
                BuyButton.GetComponentInChildren<Text>().color = Color.red;
            } else
            {
                BuyButton.interactable = true;
                BuyButton.GetComponentInChildren<Text>().text = "COMPRAR | 100c";
            }
            
            ActiveButton.interactable = false;
            ActiveButton.GetComponentInChildren<Text>().text = "ACTIVAR";
        }
    }

    //Class for JSON deserializing
    [System.Serializable]
    public class ErrorReturn
    {
        public int code;
        public string message;

        public static ErrorReturn CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ErrorReturn>(jsonString);
        }
    }

    //Request to the server for Login
    private IEnumerator BuyRequest(string item)
    {
        UnityWebRequest requestBuy = null;

        requestBuy = UnityWebRequest.Get("https://unitrivia.herokuapp.com/api/tienda/comprar");
        requestBuy.SetRequestHeader("nombre", item);
        requestBuy.SetRequestHeader("jwt", UserDataScript.getInfo("token"));
        yield return requestBuy.SendWebRequest();

        Debug.Log("ResponseCode: " + requestBuy.responseCode);

        if (requestBuy.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION ACTIVESHOP:" + requestBuy.result);

            ErrorDataScript.setErrorText("Error de conexión");
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else if (requestBuy.responseCode != 200)
        {
            Debug.Log("ERROR ACTIVESHOP:" + requestBuy.downloadHandler.text);
            ErrorReturn result = ErrorReturn.CreateFromJSON(requestBuy.downloadHandler.text);

            ErrorDataScript.setErrorText(result.message);
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("EXITO ACTIVESHOP:" + requestBuy.downloadHandler.text);

            //Obtenemos la información del usuario

            //Añadir a los objetos del usuario
        }
    }

        //Request to the server for Login
        private IEnumerator ActiveRequest(string item, string type)
    {
        UnityWebRequest requestActive = null;

        switch (type)
        {
            case "avatar":
                requestActive = UnityWebRequest.Get("https://unitrivia.herokuapp.com/api/profile/modify/avatar");
                requestActive.SetRequestHeader("idavatar", item);
                break;
            case "banner":
                requestActive = UnityWebRequest.Get("https://unitrivia.herokuapp.com/api/profile/modify/banner");
                requestActive.SetRequestHeader("idbanner", item);
                break;
            case "ficha":
                requestActive = UnityWebRequest.Get("https://unitrivia.herokuapp.com/api/profile/modify/formFicha");
                requestActive.SetRequestHeader("idformficha", item);
                break;
            default:
                ErrorDataScript.setErrorText("Error inesperado");
                ErrorDataScript.setButtonMode(1);
                SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
                break;
        }
        
        if(requestActive == null)
        {
            requestActive.SetRequestHeader("jwt", UserDataScript.getInfo("token"));
            yield return requestActive.SendWebRequest();

            Debug.Log("ResponseCode: " + requestActive.responseCode);

            if (requestActive.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("ERROR CONNECTION ACTIVESHOP:" + requestActive.result);

                ErrorDataScript.setErrorText("Error de conexión");
                ErrorDataScript.setButtonMode(1);
                SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
            }
            else if (requestActive.responseCode != 200)
            {
                Debug.Log("ERROR ACTIVESHOP:" + requestActive.downloadHandler.text);
                ErrorReturn result = ErrorReturn.CreateFromJSON(requestActive.downloadHandler.text);

                ErrorDataScript.setErrorText(result.message);
                ErrorDataScript.setButtonMode(1);
                SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
            }
            else
            {
                Debug.Log("EXITO ACTIVESHOP:" + requestActive.downloadHandler.text);

                //Obtenemos la información del usuario

                switch (type)
                {
                    case "avatar"://avatar
                        UserDataScript.setInfo("avatar", item);
                        break;
                    case "banner"://banner
                        UserDataScript.setInfo("banner", item);
                        break;
                    case "ficha"://ficha
                        UserDataScript.setInfo("ficha", item);
                        break;
                    default:
                        ErrorDataScript.setErrorText("Error inesperado");
                        ErrorDataScript.setButtonMode(1);
                        SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
                        break;
                }
            }
        }
    }
}
