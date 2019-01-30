[System.Serializable]
public class Stats
{
    public string name;
    public int leaksClosed;
    public int minutes;
    public int seconds;
    public int plungersThrown;
    public int timesSlipped;
    public int pumpsUsed;

    public Stats(int leaks, int minutes, int seconds, int plungers, int slips, int pumps)
    {
        this.leaksClosed = leaks;
        this.minutes = minutes;
        this.seconds = seconds;
        this.plungersThrown = plungers;
        this.timesSlipped = slips;
        this.pumpsUsed = pumps;
    }

    public Stats(string name, int leaks, int minutes, int seconds, int plungers, int slips, int pumps)
    {
        this.name = name;
        this.leaksClosed = leaks;
        this.minutes = minutes;
        this.seconds = seconds;
        this.plungersThrown = plungers;
        this.timesSlipped = slips;
        this.pumpsUsed = pumps;
    }

    public float getTime()
    {
        string time = "" + minutes + "." + seconds;
        float timeNum = 0;
        float.TryParse(time, out timeNum);
        return timeNum;
    }

    public override string ToString()
    {
        return name + ": " + leaksClosed + ", " + minutes + ":" + seconds + ", " + plungersThrown + ", " + timesSlipped + ", " + pumpsUsed;
    }
}
