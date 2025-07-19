using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public Inventory inventory;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (GameManager.Instance.CurrentUIState != GameUIState.None &&
                GameManager.Instance.CurrentUIState != GameUIState.Inventory)
                return;

            bool isActive = inventoryUI.activeSelf;

            inventoryUI.SetActive(!isActive);

            if (!isActive)
            {
                GameManager.Instance.PauseGame();
                CharacterMove.instance.canMove = false;
                GameManager.Instance.OpenUI(GameUIState.Inventory);
            }
            else
            {
                GameManager.Instance.ResumeGame();
                CharacterMove.instance.canMove = true;
                GameManager.Instance.CloseUI();
            }
                
        }
    }
}
