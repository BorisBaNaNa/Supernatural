
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum PlayerWeapon
{
    None,
    Knife,
    Rifle,
}

public static class Util
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Sort<T>(this List<T> list, System.Func<T, float> func)
    {
        list.Sort((a, b) => (int)Mathf.Sign(func(a) - func(b)));
    }
}
