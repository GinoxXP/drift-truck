using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JsonData
{
    public int number;
    public int[] numbers;
    public Vector3 vector;
    public bool isBool;
    public string str;
}

public class PlayerPrefsAPS : MonoBehaviour
{
    public Text playerPrefsText;

    void Start()
    {
        UpdatePlayerPrefsText();
    }

    public void SetInt()
    {
        PlayerPrefs.SetInt("someInt", Random.Range(0, 100));
        PlayerPrefs.Save();
        UpdatePlayerPrefsText();
    }

    public void SetFloat()
    {
        PlayerPrefs.SetFloat("someFloat", Random.Range(0f, 1f));
        PlayerPrefs.Save();
        UpdatePlayerPrefsText();
    }

    public void SetString()
    {
        PlayerPrefs.SetString("someString", RandomString(8));
        PlayerPrefs.Save();
        UpdatePlayerPrefsText();
    }

    public void SetJson()
    {
        JsonData jsonData = new JsonData();
        jsonData.number = Random.Range(0, 10);
        jsonData.numbers = new int[2];
        jsonData.numbers[0] = Random.Range(10, 20);
        jsonData.numbers[1] = Random.Range(20, 30);
        jsonData.vector = Vector3.one;
        jsonData.isBool = true;
        jsonData.str = RandomString(8);

        string jsonStr = JsonUtility.ToJson(jsonData);

        PlayerPrefs.SetString("someJson", jsonStr);
        PlayerPrefs.Save();
        UpdatePlayerPrefsText();
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        UpdatePlayerPrefsText();
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        UpdatePlayerPrefsText();
    }

    public void UpdatePlayerPrefsText()
    {
        playerPrefsText.text = "Player prefs contents:\n";
        bool hasKey = false;
        if (PlayerPrefs.HasKey("someInt"))
        {
            playerPrefsText.text += "Key \"someInt\" has value " + PlayerPrefs.GetInt("someInt") + "\n";
            hasKey = true;
        }
        if (PlayerPrefs.HasKey("someFloat"))
        {
            playerPrefsText.text += "Key \"someFloat\" has value " + PlayerPrefs.GetFloat("someFloat") + "\n";
            hasKey = true;
        }
        if (PlayerPrefs.HasKey("someString"))
        {
            playerPrefsText.text += "Key \"someString\" has value \"" + PlayerPrefs.GetString("someString") + "\n";
            hasKey = true;
        }
        if (PlayerPrefs.HasKey("someJson"))
        {
            playerPrefsText.text += "Key \"someJson\" has value \"" + PlayerPrefs.GetString("someJson") + "\n";
            hasKey = true;
        }
        if (!hasKey)
        {
            playerPrefsText.text = "No keys in player prefs.";
        }
    }

    private string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
