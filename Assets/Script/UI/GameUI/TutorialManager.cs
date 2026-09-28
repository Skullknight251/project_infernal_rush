using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<TutorialTextBox> tutorialPrefabs;

    public static TutorialManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnTutorial(TutorialCheck.TutorialStep tutorialStep, Transform spawnPoint)
    {
        foreach (TutorialTextBox prefab in tutorialPrefabs)
        {
            if (prefab.step == tutorialStep)
            {
                TutorialTextBox textBox = Instantiate(prefab);
                textBox.transform.SetParent(spawnPoint, false);

                textBox.gameObject.SetActive(true);

                Time.timeScale = 0f;

                return;
            }
        }
    }
}