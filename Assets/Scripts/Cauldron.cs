using System;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    public event Action<bool> Finished;
    public float TimeLeft { get; private set; }
    public int Progress { get { return index; } }

    string[] expected;
    int index;
    bool active;

    public void Begin(PotionData potion, float time)
    {
        expected = potion.recipe;
        index = 0;
        TimeLeft = time;
        active = true;
    }

    public void Add(string ingredient)
    {
        if (!active) return;
        if (ingredient == expected[index])
        {
            index++;
            if (index >= expected.Length) Finish(true);
        }
        else
        {
            Finish(false);
        }
    }

    void Update()
    {
        if (!active) return;
        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            Finish(false);
        }
    }

    void Finish(bool success)
    {
        active = false;
        if (Finished != null) Finished(success);
    }
}
