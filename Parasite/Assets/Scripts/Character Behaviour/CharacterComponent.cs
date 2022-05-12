using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    protected CharacterBase character;
    protected CharacterInfo characterInfo => character.characterInfo;

    protected virtual void Awake() => character = GetComponent<CharacterBase>();
}
