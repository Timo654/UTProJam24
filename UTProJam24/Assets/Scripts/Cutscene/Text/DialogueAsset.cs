using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Cutscenes/New Dialogue")]
[Serializable]
public class DialogueAsset : ScriptableObject
{
    [TextArea]
    public string[] dialogue;
    public bool isInputRelated = false; // when true, specify override lines
    [TextArea]
    public string[] dialogueMobile;
    [TextArea]
    public string[] dialogueXBOX;
    [TextArea]
    public string[] dialoguePS;
    [TextArea]
    public string[] dialogueTilt;
}