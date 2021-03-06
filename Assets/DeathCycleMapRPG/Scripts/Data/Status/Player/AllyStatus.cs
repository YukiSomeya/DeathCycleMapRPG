﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 仲間の識別番号
public enum AllyId
{
    Player,
    Witch
}

[Serializable]
[CreateAssetMenu(fileName = "AllyStatus", menuName = "CreateAllyStatus")]
public class AllyStatus : CharacterStatus
{
    // 識別番号
    [SerializeField]
    private AllyId id = 0;
    //　獲得経験値
    [SerializeField]
    private int earnedExperience = 0;
    //　装備している武器
    [SerializeField]
    private Weapon equipWeapon = null;
    //　装備している鎧
    [SerializeField]
    private Armor equipArmor = null;
    //装備しているアクセサリ
    [SerializeField]
    private Accessory equipAccessory1 = null;
    [SerializeField]
    private Accessory equipAccessory2 = null;

    //　レベルアップデータ
    [SerializeField]
    private LevelUpData levelUpData = null;

    //　初期ステータスデータ
    [SerializeField]
    private AllyStatus initialStatus = null;

    //　バトル時のオブジェクト
    [SerializeField]
    private GameObject BattleObject = null;

    // ステータスの更新
    public void StatusUpdate(int raisedPower, int raisedAgility, int raisedStrikingStrength, int raisedMagicPower)
    {
        // 力を反映
        this.SetPower(this.GetPower() + raisedPower);
        // 素早さを反映
        this.SetAgility(this.GetAgility() + raisedAgility);
        // 体力を反映
        this.SetStrikingStrength(this.GetStrikingStrength() + raisedStrikingStrength);
        // 魔力を反映
        this.SetMagicPower(this.GetMagicPower() + raisedMagicPower);
        // 最大HPの計算
        this.SetMaxHp();
        // 最大MPの計算
        this.SetMaxMp();
        // 攻撃力の計算
        this.SetEquippedAttackPower();
        // 防御力の計算
        this.SetEquippedDefencePower();
    }

    //ステータスの初期化
    public void StatusInit()
    {
        //レベルを初期化
        this.SetLevel(1);
        //スキルを初期化
        this.SetSkillList(this.initialStatus.GetSkillList());
        // 力を初期化
        this.SetPower(this.initialStatus.GetPower());
        // 素早さを初期化
        this.SetAgility(this.initialStatus.GetAgility());
        // 体力を初期化
        this.SetStrikingStrength(this.initialStatus.GetStrikingStrength());
        // 魔力を初期化
        this.SetMagicPower(this.initialStatus.GetMagicPower());
        //経験値をゼロに
        this.SetEarnedExperience(0);
        //武器を初期化
        this.SetEquipWeapon(this.initialStatus.GetEquipWeapon());
        //防具を初期化
        this.SetEquipArmor(this.initialStatus.GetEquipArmor());
        //アクセサリーを反映
        this.SetEquipAccessory1(this.initialStatus.GetEquipAccessory1());
        this.SetEquipAccessory2(this.initialStatus.GetEquipAccessory2());
        // 最大HPの計算
        this.SetMaxHp();
        // 最大MPの計算
        this.SetMaxMp();
        // 攻撃力の計算
        this.SetEquippedAttackPower();
        // 防御力の計算
        this.SetEquippedDefencePower();
    }

    //　レベルアップデータを返す
    public LevelUpData GetLevelUpData()
    {
        return levelUpData;
    }

    //　バトル時のオブジェクトを返す
    public GameObject GetBattleObject()
    {
        return BattleObject;
    }

    public void SetEarnedExperience(int earnedExperience)
    {
        this.earnedExperience = earnedExperience;
    }

    public int GetEarnedExperience()
    {
        return earnedExperience;
    }

    public void SetEquipWeapon(Weapon weaponItem)
    {
        //何か装備をしているならば
        if (this.equipWeapon != null)
        {
            RemoveSkill(this.equipWeapon.GetSkill());
        }
        this.equipWeapon = weaponItem;
        // 装備を外す時でなければ
        if (this.equipWeapon != null)
        {
            AddSkill(this.equipWeapon.GetSkill());
        }
    }

    public Weapon GetEquipWeapon()
    {
        return equipWeapon;
    }

    public void SetEquipArmor(Armor armorItem)
    {
        //何か装備をしているならば
        if (this.equipArmor != null)
        {
            RemoveSkill(this.equipArmor.GetSkill());
        }
        this.equipArmor = armorItem;
        // 装備を外す時でなければ
        if (this.equipArmor != null)
        {
            AddSkill(this.equipArmor.GetSkill());
        }
    }

    public Armor GetEquipArmor()
    {
        return equipArmor;
    }

    public void SetEquipAccessory1(Accessory equipAccessory1)
    {
        //何か装備をしているならば
        if (this.equipAccessory1 != null)
        {
            RemoveSkill(this.equipAccessory1.GetSkill());
        }
        this.equipAccessory1 = equipAccessory1;
        // 装備を外す時でなければ
        if (this.equipAccessory1 != null)
        {
            AddSkill(this.equipAccessory1.GetSkill());
        }
    }

    public Accessory GetEquipAccessory1()
    {
        return equipAccessory1;
    }

    public void SetEquipAccessory2(Accessory equipAccessory2)
    {
        //何か装備をしているならば
        if (this.equipAccessory2 != null)
        {
            RemoveSkill(this.equipAccessory2.GetSkill());
        }
        this.equipAccessory2 = equipAccessory2;
        // 装備を外す時でなければ
        if (this.equipAccessory2 != null)
        {
            AddSkill(this.equipAccessory2.GetSkill());
        }
    }

    public Accessory GetEquipAccessory2()
    {
        return equipAccessory2;
    }

    // スキルの追加
    public void AddSkill(Skill skill)
    {
        if (skill == null)
        {
            return;
        }
        List<Skill> newSkillList = this.GetSkillList();
        newSkillList.Add(skill);
        this.SetSkillList(newSkillList);
    }
    // スキルの削除
    public void RemoveSkill(Skill skill)
    {
        if (skill == null)
        {
            return;
        }
        List<Skill> newSkillList = this.GetSkillList();
        newSkillList.Remove(skill);
        this.SetSkillList(newSkillList);
    }

    // 装備後の攻撃力の計算
    public void SetEquippedAttackPower()
    {
        Weapon weapon = this.equipWeapon;
        int attackPower;
        if (weapon!=null)
        {
            int baseAttackPower = weapon.GetAmount();
            if (weapon.GetWeaponAttackType() == Weapon.WeaponAttackType.Power)
            {
                attackPower = baseAttackPower + this.GetPower() * 3;
            }
            else if (weapon.GetWeaponAttackType() == Weapon.WeaponAttackType.Agility)
            {
                attackPower = baseAttackPower + this.GetAgility();
            }
            else if (weapon.GetWeaponAttackType() == Weapon.WeaponAttackType.Balance)
            {
                attackPower = baseAttackPower + this.GetPower() + this.GetStrikingStrength() / 2;
            }
            else if (weapon.GetWeaponAttackType() == Weapon.WeaponAttackType.Magic)
            {
                attackPower = baseAttackPower + this.GetMagicPower() * 3;
            }
            else
            {
                attackPower = baseAttackPower;
            }
        }else
        {
            attackPower = this.GetPower() * 3;
        }
        
        this.SetAttackPower(attackPower);
    }

    // 装備後の防御力の計算
    public void SetEquippedDefencePower()
    {
        Armor armor = this.equipArmor;
        int defencePower;
        if (armor!=null)
        {
            int baseDefencePower = armor.GetAmount();
            if (armor.GetArmorType() == Armor.ArmorType.HeavyArmor)
            {
                defencePower = baseDefencePower + this.GetPower() + this.GetStrikingStrength() * 2;
            }
            else if (armor.GetArmorType() == Armor.ArmorType.LightArmor)
            {
                defencePower = baseDefencePower + this.GetAgility() + this.GetStrikingStrength();
            }
            else if (armor.GetArmorType() == Armor.ArmorType.Clothes)
            {
                defencePower = baseDefencePower + this.GetAgility() * 3 / 2;
            }
            else
            {
                defencePower = baseDefencePower;
            }
        }
        else
        {
            defencePower = this.GetStrikingStrength();
        }
        
        this.SetDefencePower(defencePower);
    }

    // ステータスをセーブ用にコピー
    public void StatusLoad(SaveAllyStatus saveAllyStatus)
    {
        //　キャラクターの名前
        SetCharacterName(saveAllyStatus.GetCharacterName());
        SetPoisonState(saveAllyStatus.IsPoisonState());
        SetNumbness(saveAllyStatus.IsNumbnessState());
        //　キャラクターのレベル
        SetLevel(saveAllyStatus.GetLevel());
        //　素早さ
        SetAgility(saveAllyStatus.GetAgility());
        //　力
        SetPower(saveAllyStatus.GetPower());
        //　打たれ強さ
        SetStrikingStrength(saveAllyStatus.GetStrikingStrength());
        //　魔法力
        SetMagicPower(saveAllyStatus.GetMagicPower());
        //　最大HP
        SetMaxHp();
        //　HP
        SetHp(saveAllyStatus.GetHp());
        //　最大MP
        SetMaxMp();
        //　MP
        SetMp(saveAllyStatus.GetMp());
        //　持っているスキル
        SetSkillList(saveAllyStatus.GetSkillList());
        //属性カット率
        SetCutFlame(saveAllyStatus.GetCutFlame());
        SetCutThunder(saveAllyStatus.GetCutThunder());
        SetCutIce(saveAllyStatus.GetcutIce());
        //　獲得経験値
        SetEarnedExperience(saveAllyStatus.GetEarnedExperience());
        //　装備している武器
        SetEquipWeapon(saveAllyStatus.GetEquipWeapon());
        //　装備している鎧
        SetEquipArmor(saveAllyStatus.GetEquipArmor());
        //装備しているアクセサリ
        SetEquipAccessory1(saveAllyStatus.GetEquipAccessory1());
        SetEquipAccessory2(saveAllyStatus.GetEquipAccessory2());
        // 攻撃力
        SetAttackPower(saveAllyStatus.GetAttackPower());
        // 守備力
        SetDefencePower(saveAllyStatus.GetDefencePower());
    }
}