using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

//Classe statica contenente le funzioni che regolano il gioco
public class GameFormulas
{
    public static bool ValidateCombatants(Hero a, Hero b) //Funzione extra per validazione combattenti inseriti
    {
        //Controllo che i combattenti siano impostati nell'Inspector
        if (a == null || b == null)
        {
            Debug.LogError("Entrambi i combattenti devono essere impostati nell'inspector");
            return false;
        }

        //Controllo che i combattenti non partano con 0 hp. Se anche uno parte con 0, la partita non si gioca.
        if (a.GetHp() <= UtilsConstants.FLOOR || b.GetHp() <= UtilsConstants.FLOOR)
        {
            Debug.LogError("Entrambi i combattenti devono avere HP positivi per iniziare lo scontro");
            return false;
        }

        return true;
    }

    //Metodo per calcolare le stats totali di un eroe, quindi base + arma
    public static Stats CalculateFullStats(Hero hero)
    {
        //Faccio caching sulle stats di base e quelle dell'arma
        var baseStats = hero.GetBaseStats();
        var weapon = hero.GetWeapon();

        //Se l'arma non c'è, allora il risultato sono le stat di base non cambiate
        if (weapon == null)
            return baseStats;

        //Se l'arma c'è, allora si invoca la somma
        return Stats.Sum(baseStats, weapon.GetBonusStats());
    }

    //Check per debolezza elementale
    public static bool HasElementAdvantage(ELEMENT attackElement, Hero defender) => attackElement == defender.GetWeakness();


    //Check per resistenza elementale
    public static bool HasElementDisadvantage(ELEMENT attackElement, Hero defender) => attackElement == defender.GetResistance();


    //Calcolo moltiplicatore per debolezza o resistenza elementale
    public static float EvaluateElementalModifier(ELEMENT attackElement, Hero defender)
    {
        float modifier = 1f; //Modificatore iniziale, non influisce sul calcolo dato che moltiplicherebbe per 1

        //Caso in cui l'eroe sia debole e resistente allo stesso elemento, il moltiplicatore rimane quello base
        if (!(HasElementAdvantage(attackElement, defender) && HasElementDisadvantage(attackElement, defender)))
        {
            if (HasElementAdvantage(attackElement, defender)) modifier = UtilsConstants.WEAK_MOD; //Se debole = 1.5 danni
            if (HasElementDisadvantage(attackElement, defender)) modifier = UtilsConstants.RES_MOD; //Se resistente 0.5 danni 
        }

        return modifier;
    }


    //Check per colpo andato a segno
    public static bool HasHit(Stats attacker, Stats defender)
    {
        //Difficoltà per colpire l'eroe. Usiamo Clamp per assicurarci di tenere tutto in un range giocabile.
        //In questo modo il minimo di hitchance è 0, cosa che rende il nemico incolpibile, mentre il massimo è 100 che lo rende sempre colpibile
        int hitChance = Mathf.Clamp(attacker.aim - defender.eva, UtilsConstants.FLOOR, UtilsConstants.ROLL_CEILING);

        //Lancio numero randomico tra 0 e 99
        int roll = Random.Range(UtilsConstants.FLOOR, UtilsConstants.ROLL_CEILING);

        //Logica per colpire, seguiamo una logica roll under, quindi valore pari o inferiore a hitChance per colpire
        bool hit = roll < hitChance;

        //Controllo se ho mancato
        if (!hit)
            //Messaggio in caso di mancato
            ColoredLogs.Miss();

        return hit;
    }

    //Check per colpo critico
    public static bool IsCrit(int critValue)
    {
        //Determiniamo la vera possbilità di critico. Usiamo Clamp per assicurarci di tenere tutto in un range giocabile.
        //Crit value, data la logica roll under, non può mai essere negativo, e nemmeno superiore a 100.
        int chance = Mathf.Clamp(critValue, UtilsConstants.FLOOR, UtilsConstants.ROLL_CEILING);

        //Lancio numero randomico tra 0 e 99
        int roll = Random.Range(UtilsConstants.FLOOR, UtilsConstants.ROLL_CEILING);

        //Logica per critico, seguiamo una logica roll under, quindi valore pari o inferiore a critValue per colpire
        bool crit = roll < chance;

        //Si genera un numero tra 0 e 99 e si confronta con critValue, se minore allora è un critico
        if (crit)
            //Segnialiamo che ha colpito e stampiamo il messaggio in console
            ColoredLogs.Crit();

        return crit;
    }


    public static int CalculateDamage(Hero attacker, Stats attackerFullStats, Hero defender, Stats defenderFullStats)
    {

        //NOTA: La parte di questo metodo che dovrebbe calcolare le statistiche totali è stata messa all'interno della sua funzione 'CalculateFullStats'
        //a fine di compiere il calcolo soltanto una volta e non tutte le volte, evitando la ripetizione inutile di calcoli dato che le statistiche sono fisse per la durata dello scontro


        //Faccio caching della weapon, così non ci accedo sempre
        var weapon = attacker.GetWeapon();

        //Rilevazione della difesa colpita. Def se danno fisico, Res se danno magico. Di default fisica se arma non presente
        int difesaBersaglio = (weapon?.GetDmgType() == Weapon.DAMAGE_TYPE.MAGICAL) ? defenderFullStats.res : defenderFullStats.def;

        //Calcolo del danno base subito. Se il risultato è negativo allora il combattente subisce 0 danni
        float baseDamage = Mathf.Max(UtilsConstants.FLOAT_FLOOR, attackerFullStats.atk - difesaBersaglio);

        //Faccio caching dell'elemento dell'arma se l'arma è presente. Di default settato a NONE
        var elementToEval = weapon?.GetElem() ?? ELEMENT.NONE;

        //Moltiplichiamo il danno base basandoci sulle resistenze elementari
        if(elementToEval != ELEMENT.NONE)
            baseDamage *= EvaluateElementalModifier(elementToEval, defender);

        //Moltiplicatore del critico
        if (IsCrit(attackerFullStats.crt))
            baseDamage *= UtilsConstants.CRIT_MOD;

        //Controllo per non infliggere danno dal valore negativo
        return Mathf.RoundToInt(Mathf.Max(UtilsConstants.FLOAT_FLOOR, baseDamage));
    }

    //Funzione extra che gestisce il singolo turno di uno dei combattenti
    public static bool TakeTurn(Hero attacker, Stats attackerFullStats, Hero defender, Stats defenderFullStats, bool nextTurn)
    {
        //Stampo nome attaccante e difensore
        ColoredLogs.Attacca(attacker.GetName());
        ColoredLogs.Difende(defender.GetName());

        //Controllo se l'attaccante ha colpito
        if (HasHit(attackerFullStats, defenderFullStats))
        {
            //Faccio caching dell'elemento dell'arma se l'arma è presente. Di default settato a NONE
            var element = attacker.GetWeapon()?.GetElem() ?? ELEMENT.NONE;

            //Stampo un messaggio se l'attacco sfrutta una debolezza elementale, evito il controllo se gli elementi non sono coinvolti
            if (element != ELEMENT.NONE)
            {
                if (HasElementAdvantage(element, defender) && !(HasElementDisadvantage(element, defender)))
                    ColoredLogs.Weakness();

                //Stampo un messaggio se l'attacco è indebolito da una resistenza elementale
                if (HasElementDisadvantage(element, defender) && !(HasElementAdvantage(element, defender)))
                    ColoredLogs.Resist();
            }

            //Calcolo il danno inflitto
            int danno = CalculateDamage(attacker, attackerFullStats, defender, defenderFullStats);

            //Lo comunico in console
            ColoredLogs.Danni(attacker.GetName(), danno);

            //Aggiorno gli HP del bersaglio con il danno subito
            defender.TakeDamage(danno);

            //Controllo se gli HP del bersaglio sono arrivati a 0 (Condizione di vittoria). 
            if (defender.GetHp() <= UtilsConstants.FLOOR)
            {
                //Se vero, allora comunico che l'attaccante ha vinto
                ColoredLogs.Vittoria(attacker.GetName());

                //Imposto a false il flag che regola la continuazione della turnistica
                nextTurn = false;
            }
        }

        //Ritorno il flag che indica se eseguire il prossimo turno o meno
        return nextTurn;
    }
}