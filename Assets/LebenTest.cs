using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Character))]
public class LebenTest : MonoBehaviour
{
    Character character;

    void Awake()
    {
        character = GetComponent<Character>();
    }

    void Update()
    {
        if (Time.timeScale > 0f &&
            Keyboard.current.uKey.wasPressedThisFrame)
        {
            character.TakeDamage(10f);
        }
    }
}
