using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe che definisce l'arma impugnata
[Serializable]
public class Weapon
{
    //Enum che definisce tipo di danno dell'arma
    public enum DAMAGE_TYPE { PHYSICAL, MAGICAL };

    [SerializeField] private string name; //Nome dell'arma
    [SerializeField] private DAMAGE_TYPE dmgType; //Tipo di danno
    [SerializeField] private ELEMENT element; //elemento di danno dell'arma
    [SerializeField] private Stats bonusStats; //Struct di statistiche date dall'arma, si sommano a quelle dell'eroe

    //Costruttori
    public Weapon() { }
    public Weapon(string name, DAMAGE_TYPE dmgType, ELEMENT element, Stats bonusStats)
    {
        SetName(name);
        SetDmgType(dmgType);
        SetElement(element);
        SetBonusStats(bonusStats);
    }

    //Getters
    public string GetName() => name;
    public DAMAGE_TYPE GetDmgType() => dmgType;
    public ELEMENT GetElement() => element;
    public Stats GetBonusStats() => bonusStats;

    //Setters
    private void SetName(string name) => this.name = name;
    private void SetDmgType(DAMAGE_TYPE type) => this.dmgType = type;
    private void SetElement(ELEMENT element) => this.element = element;
    private void SetBonusStats(Stats stats) => this.bonusStats = stats;
}
