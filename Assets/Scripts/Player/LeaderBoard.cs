using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

public class LeaderBoard
{
    private List<Stats> statsList;

    public LeaderBoard()
    {
        statsList = new List<Stats>();
    }

    public void setStatsList(List<Stats> stats)
    {
        this.statsList = stats;
    }

    public List<Stats> getStatsList()
    {
        return this.statsList;
    }

    public void WriteToFile(string filepath)
    {
        File.WriteAllText(filepath, "");
        int x = 0;
        StringBuilder statsFileString = new StringBuilder();
        foreach (Stats stats in statsList)
        {
            statsFileString.Append(JsonUtility.ToJson(stats));
            x++;
            if(x < statsList.Count)
            {
                statsFileString.Append("-");
            }
        }

        string encryptedStatsFile = encryptDecrypt(statsFileString.ToString());

        File.AppendAllText(filepath, encryptedStatsFile);

        /*foreach (Stats stats in statsList)
        {
            string json = JsonUtility.ToJson(stats);
            File.AppendAllText(filepath, json);
            x++;
            if (x < statsList.Count)
            {
                File.AppendAllText(filepath, "-");
            }
        }*/
    }

    public static string encryptDecrypt(string textToEncrypt)
    {
        int key = 129;

        StringBuilder inString = new StringBuilder(textToEncrypt);
        StringBuilder outString = new StringBuilder(textToEncrypt.Length);
        char c;
        for (int i = 0; i < textToEncrypt.Length; i++)
        {
            c = inString[i];
            c = (char)(c ^ key);
            outString.Append(c);
        }
        return outString.ToString();
    }

    public static LeaderBoard ReadFromFile(string filepath)
    {
        List<Stats> statsList = new List<Stats>();

        string encryptedContents = File.ReadAllText(filepath);
        string contents = encryptDecrypt(encryptedContents);
        string[] statsObjects = Regex.Split(contents, "-");
        foreach(string stat in statsObjects)
        {
            statsList.Add(JsonUtility.FromJson<Stats>(stat));
        }
        LeaderBoard leaderBoard = new LeaderBoard();
        leaderBoard.setStatsList(statsList);
        return leaderBoard;
    }

    public void addToList(Stats stats)
    {
        statsList.Add(stats);
    }

    public void printThis()
    {
        for (int x = 0; x < 5; x++)
        {
            Debug.Log(statsList.ElementAt<Stats>(x));
        }
    }

    public void sortList()
    {
        for (int i = 0; i < statsList.Count - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                if (statsList.ElementAt(j - 1).getTime() > statsList.ElementAt(j).getTime())
                {
                    var temp = statsList.ElementAt(j - 1);
                    statsList.Insert(j - 1, statsList.ElementAt(j));
                    statsList.RemoveAt(j);
                    statsList.Insert(j, temp);
                    statsList.RemoveAt(j + 1);
                }
            }
        }

        statsList.TrimExcess();

        this.printThis();
    }

    public bool isGoodEnough(Stats stat)
    {
        this.sortList();
        if (stat.getTime() > this.statsList.ElementAt(0).getTime())
        {
            statsList.Add(stat);
            this.sortList();
            this.statsList.RemoveAt(0);
            this.statsList.TrimExcess();
            return true;
        }
        else return false;
    }
}
