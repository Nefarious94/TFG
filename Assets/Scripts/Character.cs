using UnityEngine;
using System.Collections.Generic;
using System;

public class Character : CellObject
{
    public enum charClass
    {
        Warrior,
        Knight,
        Rogue,
        BlackMage,
        WhiteMage,
        Berserker,
        Archer
    }

    protected BoardManager m_Board;
    private bool initiated = false;
    public Vector2Int Cell => m_Cell;

    public int level = 1;

    public int basestrength;
    public int basedexterity;
    public int basevitality;
    public int baseintelligence;

    public int strength;
    public int dexterity;
    public int vitality;
    public int intelligence;
    public int hpMultiplier;
    public int manaMultiplier;

    public int maxHP;
    public int currentHP;
    public int maxMana;
    public int currentMana;
    public int attack;
    public int m_attack;
    public int defense;
    public int m_defense;
    public int rateCrit;

    public bool isCrtit;
    public bool isBuffed;
    public string buffStat;
    public int buffValue;
    public int buffTurns;

    public charClass m_class;

    public Item.ArmorType armorType;
    public Item.WeaponType weaponType;

    public List<Ability> abilitiesTree = new List<Ability>();
    public List<Ability> abilitiesUnlocked = new List<Ability>();

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_Board = GameManager.Instance.BoardManager;
        if (!initiated)
        {
            CalculateStats();
            currentHP = maxHP;
            currentMana = maxMana;
            AddAbilities();
            CheckAbilities();
            initiated = true;
        }
    }

    public void CalculateStats()
    {
        switch (m_class)
        {
            case charClass.Archer:
                basestrength = 2;
                basedexterity = 5;
                baseintelligence = 1;
                basevitality = 2;
                hpMultiplier = 2;
                manaMultiplier = 3;
                break;

            case charClass.Berserker:
                basestrength = 5;
                basedexterity = 2;
                baseintelligence = 1;
                basevitality = 2;
                hpMultiplier = 3;
                manaMultiplier = 2;
                break;

            case charClass.BlackMage:
                basestrength = 1;
                basedexterity = 3;
                baseintelligence = 5;
                basevitality = 1;
                hpMultiplier = 2;
                manaMultiplier = 4;
                break;

            case charClass.Knight:
                basestrength = 3;
                basedexterity = 1;
                baseintelligence = 1;
                basevitality = 5;
                hpMultiplier = 4;
                manaMultiplier = 2;
                break;

            case charClass.Rogue:
                basestrength = 3;
                basedexterity = 4;
                baseintelligence = 1;
                basevitality = 2;
                hpMultiplier = 3;
                manaMultiplier = 2;
                break;

            case charClass.Warrior:
                basestrength = 4;
                basedexterity = 2;
                baseintelligence = 1;
                basevitality = 3;
                hpMultiplier = 3;
                manaMultiplier = 2;
                break;

            case charClass.WhiteMage:
                basestrength = 1;
                basedexterity = 2;
                baseintelligence = 5;
                basevitality = 2;
                hpMultiplier = 2;
                manaMultiplier = 4;
                break;
        }

        strength = basestrength * level;
        dexterity = basedexterity * level;
        intelligence = baseintelligence * level;
        vitality = basevitality * level;
        
        if (this is AllyController ally)
        {
            ally.CalculateEquipment();
            strength += ally.bonusStrength;
            dexterity += ally.bonusDexterity;
            vitality += ally.bonusVitality;
            intelligence += ally.bonusIntelligence;
        }

        maxHP = vitality * hpMultiplier;
        maxMana = intelligence * manaMultiplier;
        attack = strength;
        m_attack = intelligence;
        defense = strength/2 + vitality/4;
        m_defense = intelligence/2;
        rateCrit = dexterity/3;

        if (this.isBuffed)
        {
            switch (buffStat)
            {
                case "attack":
                    attack += buffValue;
                    break;

                case "m_attack":
                    m_attack += buffValue;
                    break;

                case "defense":
                    defense += buffValue;
                    break;

                case "m_defense":
                    m_defense += buffValue;
                    break;
            }
        }
    }

    public void AddAbilities() 
    {

    }

    public void CheckAbilities()
    {

    }
}