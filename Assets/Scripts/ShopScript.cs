using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopScript : MonoBehaviour
{
    [SerializeField]  List<Button> workerButtons;
    [SerializeField] int workerPrice;

    public void MoneyCheck()
    {
        PlayerController.instance.CheckMoney();
        int money = PlayerController.instance.money;
        Debug.Log(money + " = money");
        if (money > workerPrice)
        {
            
            foreach ( Button buyButton in workerButtons)
            {
                
                buyButton.interactable = true;
            }
        }
        else if (money < workerPrice)
        {
            foreach (Button buyButton in workerButtons)
            {
                buyButton.interactable = false;
            }
        }
    }
    public void BuyWorker()
    {
        PlayerController.instance.RemoveMoney(workerPrice);
    }

}

