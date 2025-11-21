using System;
using UnityEngine;

public class M1ProjectTest : MonoBehaviour
{
    //Dichiarazione Eroi che saranno impostati da inspector
    [SerializeField] Hero a;
    [SerializeField] Hero b;

    //Flag che indica la continuazione della turnistica, parte a true per far fare almeno il primo turno
    bool nextTurn = true;

    //Variabili che gestiscono il primo eroe ad agire
    Hero firstAttacker;
    Stats firstAttackerStats;

    //Variabili che gestiscono il secondo eroe ad agire
    Hero secondAttacker;
    Stats secondAttackerStats;

    // Start is called before the first frame update
    void Start()
    {
        //Controllo che i combattenti siano impostati
        if(!GameFormulas.ValidateCombatants(a, b))
        {
            enabled = false;
            return;
        }

        //Calcolo le statistiche totali dei due combattenti
        Stats attacker1FullStats = GameFormulas.CalculateFullStats(a);
        Stats attacker2FullStats = GameFormulas.CalculateFullStats(b);

        //Determino il primo combattente ad agire in base alla velocità dei combattenti
        switch (attacker1FullStats.spd.CompareTo(attacker2FullStats.spd))
        {
            case > UtilsConstants.FLOOR:
                SetInitiative(a, attacker1FullStats, b, attacker2FullStats);
                break;
            case < UtilsConstants.FLOOR:
                SetInitiative(b, attacker2FullStats, a, attacker1FullStats); ;
                break;
            default:
                //Faccio un 50/50 se entrambi i combattenti hanno la stessa velocità
                bool aFirst = UnityEngine.Random.Range(UtilsConstants.FLOOR, UtilsConstants.SPEED_TIE_BREAK) == UtilsConstants.FLOOR;
                //Chiamo la funzione di iniziativa, passando prima a e stats di a se aPrimo = true, b e stats di b altrimenti
                SetInitiative(aFirst ? a : b, aFirst ? attacker1FullStats : attacker2FullStats, aFirst ? b : a, aFirst ? attacker2FullStats : attacker1FullStats);
                break;
        }

        //Metodo per impostare primo e secondo attaccante
        void SetInitiative(Hero first, Stats firstStats, Hero second, Stats secondStats)
        {
            //Primo ad agire
            firstAttacker = first;
            firstAttackerStats = firstStats;

            //Secondo ad agire
            secondAttacker = second;
            secondAttackerStats = secondStats;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //fino a quando nextTurn non diventa false, indicando quindi la sconfitta di uno dei due combattenti, alterna il turno tra il primo e il secondo
        if (nextTurn)
            //A turno, l'eroe più veloce va per primo
            nextTurn = GameFormulas.TakeTurn(firstAttacker, firstAttackerStats, secondAttacker, secondAttackerStats, nextTurn);

        //Si controlla alla fine di ogni turno per far sì che il gioco si fermi appena qualcuno va a 0 hp
        if (nextTurn)
            //A turno, l'eroe più lento va per secondo
            nextTurn = GameFormulas.TakeTurn(secondAttacker, secondAttackerStats, firstAttacker, firstAttackerStats, nextTurn);

    }
}

/*

TESTING E CASISTICHE

#####################################################################################
=== TEST CASE #1 — Knight vs Mage (Base) ===

Hero A (Knight):
HP: 120
Res: EARTH
Weak: FIRE
Stats: atk 20 / def 15 / res 8 / spd 10 / crt 10 / aim 85 / eva 10
Weapon: Iron Greatsword
 - Bonus: atk 8 / def 3 / res 0 / spd -2 / crt 5 / aim 5 / eva 0
 - Type: PHYSICAL / Element: NONE

Hero B (Mage):
HP: 90
Res: ICE
Weak: FIRE
Stats: atk 18 / def 6 / res 15 / spd 12 / crt 15 / aim 80 / eva 15
Weapon: Volcanic Staff
 - Bonus: atk 10 / def 0 / res 4 / spd 1 / crt 3 / aim 5 / eva 1
 - Type: MAGICAL / Element: FIRE

EXPECTATION:
- Mage va primo (SPD più alta)
- Volcanic Staff colpisce elemento WEAK del Knight
- Knight colpisce mantenendo un DPS costante
- Occasionalmente appaiono Crit e Miss
- Scontro bilanciato, risultato incerto

#####################################################################################
=== TEST CASE #2 — Fire Rogue vs Ice Guardian (Elementi) ===

Hero A — Fire Rogue
HP: 80
Res: DARKNESS
Weak: ICE
Stats: atk 35 / def 8 / res 6 / spd 18 / crt 20 / aim 90 / eva 20
Weapon: Flame Dagger
 - Bonus: atk 6 / def 0 / res 0 / spd 4 / crt 10 / aim 5 / eva 2
 - Type: PHYSICAL / Element: FIRE

Hero B — Ice Guardian
HP: 150
Res: FIRE
Weak: LIGHTNING
Stats: atk 18 / def 20 / res 18 / spd 8 / crt 5 / aim 75 / eva 5
Weapon: Frozen Shield
 - Bonus: atk 3 / def 8 / res 6 / spd -2 / crt 0 / aim 2 / eva 0
 - Type: PHYSICAL / Element: ICE

EXPECTATION:
- Rogue è sempre primo (SPD 18 vs 8 del Guardian)
- Rogue colpisce il Guardian
- Guardian è resistente al fuoco e quindi Rogue fa meno danno
- Guardian fa critico di rado, ma può subire tanti danni
- O il Rogue vince in fretta o il Guardian stalla fino a vincere

#####################################################################################
=== TEST CASE #3 — Barehanded Monk vs Assassin (colpire/schivare e no arma) ===

Hero A — Monk (no weapon)
HP: 140
Res: NONE
Weak: NONE
Stats: atk 50 / def 30 / res 25 / spd 14 / crt 5 / aim 80 / eva 50
Weapon: NONE

Hero B — Assassin
HP: 70
Res: WIND
Weak: LIGHT
Stats: atk 20 / def 6 / res 6 / spd 16 / crt 25 / aim 85 / eva 30
Weapon: Twin Daggers
  Bonus: atk 35 / def 0 / res 0 / spd 3 / crt 50 / aim 50 / eva 5
  Type: PHYSICAL / Element: NONE

EXPECTATION:
- Assassin va primo
- Molti MISS data EVA 70 di Assassin
- Monk basso crit, deve colpire molte volte 
- Assassin fragile, ma se fa crit esplode Monk
- Test per:
  - Logica senza weapon
  - Schivate
  - Scontri basati sulle differenze di crit rat
  - No debolezze elementari
#####################################################################################
*/
