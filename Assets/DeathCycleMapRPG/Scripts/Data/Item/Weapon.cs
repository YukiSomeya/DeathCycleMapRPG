﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "CreateWeapon")]
public class Weapon : Equipment
{
    public enum WeaponType
    {
        None,
        Fist,
        Axe,
        Sword,
        Dagger,
        Cane,
        Bow,
        Gun
    }

    public enum WeaponAttackType
    {
        None,
        Power,
        Agility,
        Magic,
        Balance
    }

    //武器の種類
    [SerializeField]
    private WeaponType weaponType = WeaponType.None;
    //武器の攻撃タイプ
    [SerializeField]
    private WeaponAttackType weaponAttackType = WeaponAttackType.None;

    private Weapon()
    {
        itemType = Type.WeaponAll;
    }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public WeaponAttackType GetWeaponAttackType()
    {
        return weaponAttackType;
    }
}
