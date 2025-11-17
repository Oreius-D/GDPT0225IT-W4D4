using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct che definisce le statistiche sia del personaggio che dell'arma
[Serializable]
public struct Stats
{
    public int atk; //attacco
    public int def; //difesa fisica
    public int res; //resistenza magica
    public int spd; //velocità
    public int crt; //critico
    public int aim; //mira
    public int eva; //evasione

    //costruttore
    public Stats(int atk, int def, int res, int spd, int crt, int aim, int eva)
    {
        this.atk = atk;
        this.def = def;
        this.res = res;
        this.spd = spd;
        this.crt = crt;
        this.aim = aim;
        this.eva = eva;
    }

    //Metodo che somma due struc di statistiche
    public static Stats Sum(Stats stat1, Stats stat2)
    {
        return new Stats(
            stat1.atk + stat2.atk,
            stat1.def + stat2.def,
            stat1.res + stat2.res,
            stat1.spd + stat2.spd,
            stat1.crt + stat2.crt,
            stat1.aim + stat2.aim,
            stat1.eva + stat2.eva);
    }
}
