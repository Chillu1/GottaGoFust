//using System;

[System.Serializable]
public class Data
{
    public static Data Instance;

    //Encrypted file with highscores?

    //Arrays are a little more efficient, but they have a max value, while lists are dynamic so they're "infinite"
    public float[] timesForFirstMap; //Use list rather than array?

    public float[] timesForSecondMap;

    public float[] timesForSingleSegmentRun;

    public float time;
    public float[] timePerMap;
    public int currentMap;

    public Data(float newTime, float[] newTimePerMap, int newCurrentMap)
    {
        this.time = newTime;
        this.timePerMap = newTimePerMap;
        this.currentMap = newCurrentMap;
    }
}