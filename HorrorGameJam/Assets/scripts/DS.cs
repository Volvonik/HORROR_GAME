using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DS : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] float[] globalLightStrength;
    [SerializeField] float[] playerLightStrength;
    [SerializeField] float[] groundLightStrength;

    [Header("GameObjects")]
    [SerializeField] GameObject[] signsPresets;
    [SerializeField] GameObject[] dinoArenasPresets;

    [Header("Baby Chase")]
    [SerializeField] float[] babySpeed;

    [Header("Hand")]
    [SerializeField] float[] babyAttackDelay;
    [SerializeField] int[] babyLifeTotal;
    [SerializeField] float[] throwHazardDelay;
    public float[] hazardMoveSpeed; //The speed applies in the hazard script at start

    [Header("Dino The Shark")]
    [SerializeField] float[] dinoMoveSpeed;
    [SerializeField] float[] dinoEatingTime;

    [Header("The Actuall Stuff")]
    [SerializeField] Light2D globalLightObject;
    [SerializeField] Light2D playerLightObject;
    [SerializeField] Light2D groundLightObject;

    [SerializeField] ArmScript hand;
    [SerializeField] DinoScript dino;

    private void Start()
    {
        globalLightObject.intensity = globalLightStrength[MainMenuScript.difficulty];
        playerLightObject.intensity = playerLightStrength[MainMenuScript.difficulty];
        groundLightObject.intensity = groundLightStrength[MainMenuScript.difficulty];

        for (int i = 0; i < 3; i++)
        {
            if (i == MainMenuScript.difficulty)
            {
                signsPresets[i].SetActive(true);
                dinoArenasPresets[i].SetActive(true);
            }
            else
            {
                signsPresets[i].SetActive(false);
                dinoArenasPresets[i].SetActive(false);
            }
        }

        hand.SetStats(babyAttackDelay[MainMenuScript.difficulty], babyLifeTotal[MainMenuScript.difficulty], throwHazardDelay[MainMenuScript.difficulty]);

        dino.SetStats(dinoMoveSpeed[MainMenuScript.difficulty], dinoEatingTime[MainMenuScript.difficulty]);
    }

    private void Update()
    {
        if (FindObjectOfType<BabyScript>() != null)
        {
            FindObjectOfType<BabyScript>().SetStats(babySpeed[MainMenuScript.difficulty]);
        }
    }
}
