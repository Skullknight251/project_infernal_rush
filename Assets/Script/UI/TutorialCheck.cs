using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TutorialCheck : MonoBehaviour
{
    public enum TutorialStep
    {
        basicSkill,
        parry,
        stun,
        block,
        dropItem,
        blockEnemy
    }
    public static TutorialCheck Instance { get; private set; }
    private Dictionary<TutorialStep, bool> tutorialStatus = new();

    private void Awake()
    {
        Instance = this;
        //undoCompleteTutorial(TutorialStep.basicSkill);
        //undoCompleteTutorial(TutorialStep.parry);
        //undoCompleteTutorial(TutorialStep.dropItem);
        //undoCompleteTutorial(TutorialStep.stun);
        //undoCompleteTutorial(TutorialStep.block);
        foreach (TutorialStep step in Enum.GetValues(typeof(TutorialStep)))
        {
            tutorialStatus[step] = PlayerPrefs.GetInt(GetKey(step), 0) == 1;
            Debug.Log(step + " " + PlayerPrefs.GetInt(GetKey(step),0));
        }
    }

    private string GetKey(TutorialStep step)
    {
        return $"Tutorial_{step}";
    }

    public bool IsCompleted(TutorialStep step)
    {
        return tutorialStatus[step];
    }

    public void CompleteTutorial(TutorialStep step)
    {
        tutorialStatus[step] = true;
        PlayerPrefs.SetInt(GetKey(step), 1);
        PlayerPrefs.Save();
    }

    public void undoCompleteTutorial(TutorialStep step)
    {
        tutorialStatus[step] = false;
        PlayerPrefs.SetInt(GetKey(step), 0);
        PlayerPrefs.Save();
    }
}