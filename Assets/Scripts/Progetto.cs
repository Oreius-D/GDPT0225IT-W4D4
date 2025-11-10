using System;
using UnityEngine;

//Creiamo una classe statica specifica per gestire i valori hardcoded
public static class Utils
{
    public const int FLOOR = 0;
    public const int SPEED_TIE_BREAK = 2;
    public const int ROLL_CEILING = 100;

    public const float FLOAT_FLOOR = 0f;
    public const float NO_MOD = 1f;
    public const float WEAK_MOD = 1.5f;
    public const float RES_MOD = 0.5f;
    public const float CRIT_MOD = 2f;
}

//Classe statica aggiunta per rendere più leggibili i log
public static class LogColor
{
    //Turnistica
    public static void Attacca(string attaccante) => Debug.Log($"<color=#FFA500><b>{attaccante} ATTACCA!</b></color>");
    public static void Difende(string difensore) => Debug.Log($"<color=#00BFFF><b>{difensore} DIFENDE!</b></color>"); 

    //Mancato
    public static void Miss() => Debug.Log($"<color=#808080>MISS</color>"); 

    // Critico
    public static void Crit() => Debug.Log("<color=#FF00FF><b>CRIT!</b></color>");

    //Debolezza e Resistenza
    public static void Weakness() => Debug.Log("<color=#FF69B4><b>WEAKNESS!</b></color>");
    public static void Resist() => Debug.Log("<color=#87CEFA><b>RESIST!</b></color>");

    // Danno inflitto
    public static void Danni(string attaccante, int danno) => Debug.Log($"<color=#FFD700>{attaccante} infligge <b>{danno}</b> danni!</color>");

    // Vittoria
    public static void Vittoria(string vincitore) => Debug.Log($"<color=#00FF7F><b>{vincitore} HA VINTO!</b></color>");
}

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

//Raccolta degli elementi che contano sia per difesa che per danno
public enum ELEMENT { NONE, FIRE, WATER, WIND, EARTH, ICE, LIGHTNING, LIGHT, DARKNESS }

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
    public void TakeDamage(int damage) => hp = Mathf.Max(Utils.FLOOR, hp - damage);

    //Metodo per controllare se l'eroe è vivo. Ritorna false se hp diventano 0
    public bool IsAlive() => hp > Utils.FLOOR;
}

//Classe statica contenente le funzioni che regolano il gioco
public static class GameFormulas
{
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
            if (HasElementAdvantage(attackElement, defender)) modifier = Utils.WEAK_MOD; //Se debole = 1.5 danni
            if (HasElementDisadvantage(attackElement, defender)) modifier = Utils.RES_MOD; //Se resistente 0.5 danni 
        }

        return modifier;
    }


    //Check per colpo andato a segno
    public static bool HasHit(Stats attacker, Stats defender)
    {
        //Difficoltà per colpire l'eroe. Usiamo Clamp per assicurarci di tenere tutto in un range giocabile.
        //In questo modo il minimo di hitchance è 0, cosa che rende il nemico incolpibile, mentre il massimo è 100 che lo rende sempre colpibile
        int hitChance = Mathf.Clamp(attacker.aim - defender.eva, Utils.FLOOR, Utils.ROLL_CEILING);

        //Lancio numero randomico tra 0 e 99
        int roll = UnityEngine.Random.Range(Utils.FLOOR, Utils.ROLL_CEILING);

        //Logica per colpire, seguiamo una logica roll under, quindi valore pari o inferiore a hitChance per colpire
        bool hit = roll < hitChance;

        //Controllo se ho mancato
        if (!hit)
            //Messaggio in caso di mancato
            LogColor.Miss();

        return hit;
    }

    //Check per colpo critico
    public static bool IsCrit(int critValue)
    {
        //Determiniamo la vera possbilità di critico. Usiamo Clamp per assicurarci di tenere tutto in un range giocabile.
        //Crit value, data la logica roll under, non può mai essere negativo, e nemmeno superiore a 100.
        int chance = Mathf.Clamp(critValue, Utils.FLOOR, Utils.ROLL_CEILING);

        //Lancio numero randomico tra 0 e 99
        int roll = UnityEngine.Random.Range(Utils.FLOOR, Utils.ROLL_CEILING);

        //Logica per critico, seguiamo una logica roll under, quindi valore pari o inferiore a critValue per colpire
        bool crit = roll < chance;

        //Si genera un numero tra 0 e 99 e si confronta con critValue, se minore allora è un critico
        if (crit)
            //Segnialiamo che ha colpito e stampiamo il messaggio in console
            LogColor.Crit();

        return crit;
    }

    //Metodo per calcolare le stats totali di un eroe, quindi base + arma
    public static Stats CalculateTrueStats(Hero hero)
    {
        //Mi salvo le stats di base e l'arma
        var baseStats = hero.GetBaseStats();
        var weapon = hero.GetWeapon();

        //Se l'arma non c'è, allora il risultato sono le stat di base non cambiate
        if (weapon == null)
            return baseStats;

        //Se l'arma c'è, allora si invoca la somma
        return Stats.Sum(baseStats, weapon.GetBonusStats());
    }

    //Funzione per il calcolo del danno
    public static int CalculateDamage(Hero attacker, Hero defender)
    {
        //Calcolo statistiche totali dei due combattenti
        var attackerFullStats = CalculateTrueStats(attacker);
        var defenderFullStats = CalculateTrueStats(defender);

        //Faccio caching della weapon, così non ci accedo sempre
        var weapon = attacker.GetWeapon();


        //Rilevazione della difesa colpita. Def se danno fisico, Res se danno magico. Di default fisica se arma non presente
        int difesaBersaglio = (weapon?.GetDmgType() == Weapon.DAMAGE_TYPE.MAGICAL) ? defenderFullStats.res : defenderFullStats.def;

        //Calcolo del danno base subito. Se il risultato è negativo allora il combattente subisce 0 danni
        float baseDamage = Mathf.Max(Utils.FLOAT_FLOOR, attackerFullStats.atk - difesaBersaglio);

        //Faccio caching dell'elemento dell'arma se l'arma è presente. Di default settato a NONE
        var elementToEval = weapon?.GetElement() ?? ELEMENT.NONE;

        //Moltiplichiamo il danno base basandoci sulle resistenze elementari
        baseDamage *= EvaluateElementalModifier(elementToEval, defender);

        //Moltiplicatore del critico
        if (IsCrit(attackerFullStats.crt))
            baseDamage *= Utils.CRIT_MOD;

        //Controllo per non infliggere danno dal valore negativo
        return Mathf.RoundToInt(Mathf.Max(Utils.FLOAT_FLOOR, baseDamage));
    }

    //Funzione che gestisce il singolo turno di uno dei combattenti
    public static bool TakeTurn(Hero attacker, Stats attackerFullStats, Hero defender, Stats defenderFullStats, bool nextTurn)
    {
        //Stampo nome attaccante e difensore
        LogColor.Attacca(attacker.GetName());
        LogColor.Difende(defender.GetName());


        //Controllo se l'attaccante ha colpito
        if (HasHit(attackerFullStats, defenderFullStats))
        {
            //Faccio caching dell'elemento dell'arma se l'arma è presente. Di default settato a NONE
            var element = attacker.GetWeapon()?.GetElement() ?? ELEMENT.NONE;

            //Stampo un messaggio se l'attacco sfrutta una debolezza elementale
            if (HasElementAdvantage(element, defender) && !(HasElementDisadvantage(element, defender)))
                LogColor.Weakness();

            //Stampo un messaggio se l'attacco è indebolito da una resistenza elementale
            if (HasElementDisadvantage(element, defender) && !(HasElementAdvantage(element, defender)))
                LogColor.Resist();

            //Calcolo il danno inflitto
            int danno = CalculateDamage(attacker, defender);

            //Lo comunico in console
            LogColor.Danni(attacker.GetName(), danno);

            //Aggiorno gli HP del bersaglio con il danno subito
            defender.TakeDamage(danno);

            //Controllo se gli HP del bersaglio sono arrivati a 0 (Condizione di vittoria). 
            if (defender.GetHp() <= Utils.FLOOR)
            {
                //Se vero, allora comunico che l'attaccante ha vinto
                LogColor.Vittoria(attacker.GetName());

                //Imposto a false il flag che regola la continuazione della turnistica
                nextTurn = false;

            }
        }

        //Ritorno il flag che indica se eseguire il prossimo turno o meno
        return nextTurn;
    }
}

public class Progetto : MonoBehaviour
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
        if (a == null || b == null)
        {
            Debug.LogError("Entrambi i combattenti devono essere impostati nell'inspector");
            enabled = false;
            return;
        }


        //Controllo che i combattenti non partano con 0 hp. Se anche uno parte con 0, la partita non si gioca.
        if (a.GetHp() <= Utils.FLOOR || b.GetHp() <= Utils.FLOOR)
        {
            //Messaggio per avvertire che il combattimento non può avvenire
            Debug.LogError("Entrambi i combattenti devono avere HP positivi per iniziare lo scontro");
            enabled = false;
            return;
        }


        //Calcolo le statistiche totali dei due combattenti
        Stats attacker1TrueStats = GameFormulas.CalculateTrueStats(a);
        Stats attacker2TrueStats = GameFormulas.CalculateTrueStats(b);

        //Determino il primo combattente ad agire in base alla velocità dei combattenti
        if (attacker1TrueStats.spd > attacker2TrueStats.spd)
        {
            //Se a è più veloce di b, a diventa il primo attaccante
            Iniziativa(a, attacker1TrueStats, b, attacker2TrueStats);
        }
        else if (attacker1TrueStats.spd < attacker2TrueStats.spd)
        {
            //Se b è più veloce di a, b diventa il primo attaccante
            Iniziativa(b, attacker2TrueStats, a, attacker1TrueStats);
        }
        else
        {
            //Faccio un 50/50 se entrambi i combattenti hanno la stessa velocità
            bool aPrimo = UnityEngine.Random.Range(Utils.FLOOR, Utils.SPEED_TIE_BREAK) == Utils.FLOOR;
            //Chiamo la funzione di iniziativa, passando prima a e stats di a se aPrimo = true, b e stats di b altrimenti
            Iniziativa(aPrimo ? a : b, aPrimo ? attacker1TrueStats : attacker2TrueStats, aPrimo ? b : a, aPrimo ? attacker2TrueStats : attacker1TrueStats);
        }

        //Metodo per impostare primo e secondo attaccante
        void Iniziativa(Hero first, Stats firstStats, Hero second, Stats secondStats)
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
Stats: atk20 / def15 / res8 / spd10 / crt10 / aim85 / eva10
Weapon: Iron Greatsword
 - Bonus: atk+8 / def+3 / res+0 / spd-2 / crt+5 / aim+5 / eva+0
 - Type: PHYSICAL / Element: NONE

Hero B (Mage):
HP: 90
Res: ICE
Weak: FIRE
Stats: atk18 / def6 / res15 / spd12 / crt15 / aim80 / eva15
Weapon: Volcanic Staff
 - Bonus: atk+10 / def0 / res+4 / spd+1 / crt+3 / aim+5 / eva+1
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
Stats: atk35 / def8 / res6 / spd18 / crt20 / aim90 / eva20
Weapon: Flame Dagger
 - Bonus: atk+6 / def0 / res0 / spd+4 / crt+10 / aim+5 / eva+2
 - Type: PHYSICAL / Element: FIRE

Hero B — Ice Guardian
HP: 150
Res: FIRE
Weak: LIGHTNING
Stats: atk18 / def20 / res18 / spd8 / crt5 / aim75 / eva5
Weapon: Frozen Shield
 - Bonus: atk+3 / def+8 / res+6 / spd-2 / crt0 / aim+2 / eva0
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
HP: 110
Res: NONE
Weak: NONE
Stats: atk15 / def12 / res10 / spd14 / crt5 / aim70 / eva15
Weapon: NONE

Hero B — Assassin
HP: 70
Res: WIND
Weak: LIGHT
Stats: atk20 / def6 / res6 / spd16 / crt25 / aim85 / eva30
Weapon: Twin Daggers
  Bonus: atk+4 / def0 / res0 / spd+3 / crt+10 / aim+3 / eva+5
  Type: PHYSICAL / Element: NONE

EXPECTATION:
- Assassin va primo (SPD troppo alta)
- Molti MISS data EVA30 di Assassin
- Monk basso crit e basso danno, deve colpire molte volte 
- Assassin fragile, ma se fa crit esplode Monk
- Test per:
  - Logica senza weapon
  - Schivate
  - Scontri basati sulle differenze di crit rate
#####################################################################################
*/
