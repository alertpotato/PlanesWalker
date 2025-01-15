using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct EventWeights
{
    [Tooltip("Event name")]public string Name;
    [Tooltip("Weight of event")]public int Weight;
    public EventWeights( string name,int eventWeight)
    {
        Name = name;Weight=eventWeight;
    }
}
[CreateAssetMenu]
public class EventFactory : ScriptableObject
{
    public List<EventWeights> EventWeights;
    public void InizializeEventFactory()
    {
        EventWeights = new List<EventWeights>();
        FillListOfEvents();
    }

    public List<int> GenerateEventSet(int numberOfEvents)
    {
        List<int> eventsIds = new List<int>();
        for (int e = 0; e < numberOfEvents; e++)
        {
            var localListOfWeights = new int[EventWeights.Count];
            for (int i = 0; i < localListOfWeights.Length; i++)
                { localListOfWeights[i] = EventWeights[i].Weight; }
            int indexOfSelectedReward = WeightFunctions.GetRandomWeightedIndex(localListOfWeights);

            eventsIds.Add(indexOfSelectedReward);
        }
        return eventsIds;
    }

    private void FillListOfEvents()
    {
        EventWeights.Clear();
        
        var OgreCamp = new EventWeights("Ogre camp",3);
        EventWeights.Add(OgreCamp);
        var Caravan = new EventWeights("Caravan ambush",2);
        EventWeights.Add(Caravan);
        var GoblinPatrol = new EventWeights("Goblin patrol",3);
        EventWeights.Add(GoblinPatrol);
        var EvilArmy = new EventWeights("Sorcerer army",1);
        EventWeights.Add(EvilArmy);
    }
    
}
