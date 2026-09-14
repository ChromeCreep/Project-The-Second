using UnityEngine;

public class EnergyDisplay : MonoBehaviour
{
    public GameObject player;
    public Basic2DMovement playerMovement;

    public GameObject energyBar1;
    public GameObject energyBar2;
    public GameObject energyBar3;

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.energyAmount >= 3)
        {
            energyBar1.SetActive(true);
            energyBar2.SetActive(true);
            energyBar3.SetActive(true);
        }
        else if (playerMovement.energyAmount >= 2)
        {
            energyBar1.SetActive(true);
            energyBar2.SetActive(true);
            energyBar3.SetActive(false);
        }
        else if (playerMovement.energyAmount >= 1)
        {
            energyBar1.SetActive(true);
            energyBar2.SetActive(false);
            energyBar3.SetActive(false);
        }
        else if (playerMovement.energyAmount >= 0)
        {
            energyBar1.SetActive(false);
            energyBar2.SetActive(false);
            energyBar3.SetActive(false);
        }
        }
    }
