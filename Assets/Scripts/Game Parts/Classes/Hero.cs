using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe che definisce l'eroe
[Serializable]
public class Hero
{

    [SerializeField] private string name; //Nome 
    [SerializeField] private int hp; //Hp, o punti vita

    [SerializeField] private Stats baseStats; //Statistiche base dell'eroe

    [SerializeField] private ELEMENT resistance; //Elemento a cui è resistente, applica un moltiplicatore di 0.5 al danno subito
    [SerializeField] private ELEMENT weakness; //Elemento a cui è debole, applica un moltiplicatore di 1.5 al danno subito

    [SerializeField] private Weapon weapon; //Arma impugnata, vedi classe


    //Costruttori
    public Hero() { }
    public Hero(string name, int hp, Stats baseStats, ELEMENT resistance, ELEMENT weakness, Weapon weapon)
    {
        SetName(name);
        SetHp(hp);
        SetBaseStats(baseStats);
        SetResistance(resistance);
        SetWeakness(weakness);
        SetWeapon(weapon);
    }


    //Setter
    private void SetName(string name) => this.name = name;
    private void SetHp(int hp) => this.hp = Mathf.Max(0, hp);
    private void SetBaseStats(Stats stats) => this.baseStats = stats;
    private void SetResistance(ELEMENT elem) => this.resistance = elem;
    private void SetWeakness(ELEMENT elem) => this.weakness = elem;
    private void SetWeapon(Weapon weapon) => this.weapon = weapon;


    //Getter
    public string GetName() => name;
    public int GetHp() => hp;
    public Stats GetBaseStats() => baseStats;
    public ELEMENT GetResistance() => resistance;
    public ELEMENT GetWeakness() => weakness;
    public Weapon GetWeapon() => weapon;


    //Metodo per curare l'erore. Aggiunge hp
    public void AddHp(int amount) => hp += amount;

    //Metodo per ferire l'erore. Sottrae hp. Se diventano negativi li setta a 0
    public void TakeDamage(int damage) => hp = Mathf.Max(UtilsConstants.FLOOR, hp - damage);

    //Metodo per controllare se l'eroe è vivo. Ritorna false se hp diventano 0
    public bool IsAlive() => hp > UtilsConstants.FLOOR;
}
