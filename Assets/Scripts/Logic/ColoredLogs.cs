using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColoredLogs
{
    //Classe statica aggiunta per rendere più leggibili i log

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
