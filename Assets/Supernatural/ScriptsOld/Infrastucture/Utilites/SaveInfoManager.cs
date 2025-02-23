using System;
using UnityEngine;

public static class SaveInfoManager
{
    private static readonly string WorldsOpened = "WorldsOpened";
    private static readonly string LevelsOpened = "LevelsOpened";
    private static readonly string Points = "Points";
    private static readonly string Coins = "Coins";
    private static readonly string Lives = "Lives";
    private static readonly string Bullets = "Bullets";
    private static readonly string Character = "Character";
    private static readonly string ChoosenCharacterID = "choosenCharacterID";
    private static readonly string SoundVal = "SoundVal";
    private static readonly string MusicVal = "MusicVal";

    public static void SaveCoins(int coins) =>
        PlayerPrefs.SetInt(Coins, coins);

    public static void SaveLives(int lives) =>
        PlayerPrefs.SetInt(Lives, lives);

    public static void SaveBullets(int bullet) =>
        PlayerPrefs.SetInt(Bullets, bullet);

    public static void SavePoints(int lvlID, int points) =>
        PlayerPrefs.SetInt(Points + lvlID, points);

    public static void SaveStateCharacterID(int characterID, int val) =>
        PlayerPrefs.SetInt(Character + characterID, val);

    public static void SaveChoosenCharacterID(int characterID) =>
        PlayerPrefs.SetInt(ChoosenCharacterID, characterID);

    public static void SaveWorldsOpened(int count) =>
        PlayerPrefs.SetInt(WorldsOpened, count);

    public static void SaveLevelsOpened(int worldId, int count) =>
        PlayerPrefs.SetInt(LevelsOpened + worldId, count);

    public static void SaveSoundVal(float val) =>
        PlayerPrefs.SetFloat(SoundVal, val);

    public static void SaveMusicVal(float val) =>
    PlayerPrefs.SetFloat(MusicVal, val);


    public static int LoadCoins(int defVal) =>
    PlayerPrefs.GetInt(Coins, defVal);

    public static int LoadLives(int defVal) =>
        PlayerPrefs.GetInt(Lives, defVal);

    public static int LoadBullets(int defVal) =>
        PlayerPrefs.GetInt(Bullets, defVal);

    public static int LoadPoints(int lvlID) =>
        PlayerPrefs.GetInt(Points + lvlID, 0);

    public static bool LoadStateCharacterID(int characterID) =>
        PlayerPrefs.GetInt(Character + characterID, 0) == 1;

    public static int LoadChoosenCharacterID() =>
        PlayerPrefs.GetInt(ChoosenCharacterID, 0);

    public static int LoadWorldsOpened() =>
        PlayerPrefs.GetInt(WorldsOpened, 1);

    public static int LoadLevelsOpened(int worldId) =>
        PlayerPrefs.GetInt(LevelsOpened + worldId, 1);

    public static float LoadSoundVal() =>
        PlayerPrefs.GetFloat(SoundVal, 0.5f);

    public static float LoadMusicVal() =>
        PlayerPrefs.GetFloat(MusicVal, 0.5f);

}