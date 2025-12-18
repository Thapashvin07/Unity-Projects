using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public int id;
    public string Name;
    public int cost;
    public Ability ability;
    public Card(int id, string name, int cost, Ability ability)
    {
        this.id = id;
        this.Name = name;
        this.cost = cost;
        this.ability = ability;
    }
}
public class Ability
{
    public int type;
    public int value;
    public Ability(int type, int value)
    {
        this.type = type;
        this.value = value;
    }
}
