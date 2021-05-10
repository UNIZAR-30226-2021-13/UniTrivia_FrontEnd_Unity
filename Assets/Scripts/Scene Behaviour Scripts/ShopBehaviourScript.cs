using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private string name = "avatar";

    // Start is called before the first frame update
    void Start()
    {
        LeftButton.onClick.AddListener(LeftButtonOnClick);
        RightButton.onClick.AddListener(RightButtonOnClick);
        AvatarShopButton.onClick.AddListener(AvatarShopButtonOnClick);
        BannerShopButton.onClick.AddListener(BannerShopButtonOnClick);
        TokenShopButton.onClick.AddListener(TokenShopButtonOnClick);

        ObjectImage.sprite = Resources.Load<Sprite>(path + name + index);
        checkActualItem();
    }

    void AvatarShopButtonOnClick()
    {
        path = "Avatar/";
        name = "avatar";
        index = 0;
        maxIndex = 5;
        ObjectImage.sprite = Resources.Load<Sprite>(path + name + index);
    }

    void BannerShopButtonOnClick()
    {
        path = "Banner/";
        name = "banner";
        index = 0;
        maxIndex = 5;
        ObjectImage.sprite = Resources.Load<Sprite>(path + name + index);
    }

    void TokenShopButtonOnClick()
    {
        path = "Token/";
        name = "ficha";
        index = 0;
        maxIndex = 3;
        ObjectImage.sprite = Resources.Load<Sprite>(path + name + index);
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
        ObjectImage.sprite = Resources.Load<Sprite>(path + name + index);
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
        ObjectImage.sprite = Resources.Load<Sprite>(path + name + index);
        checkActualItem();
    }

    void BuyButtonOnClick()
    {

    }

    void ActiveButtonOnClick()
    {

    }

    void checkActualItem()
    {
        if(!UserDataScript.isItem(name + index) | index == 0)
        {
            BuyButton.interactable = false;
            ActiveButton.interactable = true;
        } else
        {
            BuyButton.interactable = true;
            ActiveButton.interactable = false;
        }
    }
}
