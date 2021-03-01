using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameCodeGenerator
{
    private static System.Random random = new System.Random();
    private static string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string Generate(int length = 6)
    {
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
