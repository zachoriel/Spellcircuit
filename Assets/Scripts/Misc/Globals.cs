using UnityEngine;

public static class Globals
{
    public struct KeyBinds
    {
        public static KeyCode RUN_KEY = KeyCode.LeftShift;
        public static KeyCode JUMP_KEY = KeyCode.Space;
        public static KeyCode ATTACK_BUTTON = KeyCode.Mouse0;
        public static KeyCode AIM_BUTTON = KeyCode.Mouse1;
    }

    public struct Tags
    {
        public static string PLAYER_TAG = "player";
    }
}
