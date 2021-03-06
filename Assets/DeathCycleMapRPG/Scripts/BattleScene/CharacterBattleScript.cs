﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattleScript : MonoBehaviour
{
    //　戦闘中のキャラクターの状態
    public enum BattleState
    {
        Idle,
        DirectAttack,
        MagicAttack,
        Healing,
        UseHPRecoveryItem,
        UseMPRecoveryItem,
        UseNumbnessRecoveryItem,
        UsePoisonRecoveryItem,
        IncreaseAttackPowerMagic,
        IncreaseDefencePowerMagic,
        NumbnessRecoveryMagic,
        PoisonnouRecoveryMagic,
        Damage,
        Guard,
        Dead,
    }

    private BattleManager battleManager;
    private PartyStatus partyStatus;
    private BattleStatusScript battleStatusScript;
    public CharacterStatus characterStatus = null;
    private Animator animator;
    private BattleState battleState;

    //　HP
    private int hp = 0;
    //　MP
    private int mp = 0;

    //　補助の素早さ
    private int auxiliaryAgility = 0;
    //　補助の力
    private int auxiliaryPower = 0;
    //　補助の打たれ強さ
    private int auxiliaryStrikingStrength = 0;
    //　痺れ状態か
    private bool isNumbness;
    //　毒状態か
    private bool isPoison;
    // ガードのカットりつ
    private double guardPercentage;

    //　今選択したスキル
    private Skill currentSkill;
    //　今のターゲット
    private GameObject currentTarget;
    //　今使用したアイテム
    private Item currentItem;
    //　ターゲットのCharacterBattleScript
    private CharacterBattleScript targetCharacterBattleScript;
    //　ターゲットのCharacterStatus
    private CharacterStatus targetCharacterStatus;
    //　攻撃選択後のアニメーションが終了したかどうか
    private bool isDoneAnimation;
    //　キャラクターが死んでいるかどうか
    private bool isDead;

    //　攻撃力アップしているかどうか
    private bool isIncreasePower;
    //　攻撃力アップしているポイント
    private int increasePowerPoint;
    //　攻撃力アップしているターン
    private int numOfTurnsIncreasePower = 3;
    //　攻撃力アップしてからのターン
    private int numOfTurnsSinceIncreasePower = 0;
    //　防御力アップしているかどうか
    private bool isIncreaseStrikingStrength;
    //　防御力アップしているポイント
    private int increaseStrikingStrengthPoint;
    //　防御力アップしているターン
    private int numOfTurnsIncreaseStrikingStrength = 3;
    //　防御力アップしてからのターン
    private int numOfTurnsSinceIncreaseStrikingStrength = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        //　元データから設定
        hp = characterStatus.GetHp();
        mp = characterStatus.GetMp();
        isNumbness = characterStatus.IsNumbnessState();
        isPoison = characterStatus.IsPoisonState();

        //　状態の設定
        battleState = BattleState.Idle;
        //ガードの状態
        guardPercentage = 1;
        //　コンポーネントの取得
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        partyStatus = battleManager.partyStatus;
        battleStatusScript = GameObject.Find("BattleUICanvas/PlayerUI/StatusPanel").GetComponent<BattleStatusScript>();
        //　既に死んでいる場合は倒れている状態にする
        if (characterStatus.GetHp() <= 0)
        {
            animator.CrossFade("Dead", 0f, 0, 1f);
            isDead = true;
        }
    }

    void Update()
    {
        //　既に死んでいたら何もしない
        if (isDead)
        {
            return;
        }

        //　自分のターンでなければ何もしない
        if (battleState == BattleState.Idle)
        {
            return;
        }

        /*
        //　アニメーションが終わっていなければ何もしない
        if (!isDoneAnimation)
        {
            return;
        }
        */

        //　選択したアニメーションによって処理を分ける
        if (battleState == BattleState.DirectAttack)
        {
            //ShowEffectOnTheTarget();
            DirectAttack();
            //　自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
        }
        else if (battleState == BattleState.MagicAttack)
        {
            //ShowEffectOnTheTarget();
            MagicAttack();
            //　自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
        }
        else if (battleState == BattleState.Healing
          || battleState == BattleState.NumbnessRecoveryMagic
          || battleState == BattleState.PoisonnouRecoveryMagic
          )
        {
            //ShowEffectOnTheTarget();
            UseMagic();
            //　自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
        }
        else if (battleState == BattleState.IncreaseAttackPowerMagic)
        {
            //ShowEffectOnTheTarget();
            UseMagic();
            //　自身の攻撃力をアップした場合はターン数をカウントしない
            if (currentTarget == this.gameObject)
            {
                CheckIncreaseStrikingStrength();
            }
            else
            {
                CheckIncreaseAttackPower();
                CheckIncreaseStrikingStrength();
            }
        }
        else if (battleState == BattleState.IncreaseDefencePowerMagic)
        {
            //ShowEffectOnTheTarget();
            UseMagic();
            //　自身の防御力をアップした場合はターン数をカウントしない
            if (currentTarget == this.gameObject)
            {
                CheckIncreaseAttackPower();
            }
            else
            {
                CheckIncreaseAttackPower();
                CheckIncreaseStrikingStrength();
            }
        }
        else if (battleState == BattleState.UseHPRecoveryItem
          || battleState == BattleState.UseMPRecoveryItem
          || battleState == BattleState.UseNumbnessRecoveryItem
          || battleState == BattleState.UsePoisonRecoveryItem
          )
        {
            UseItem();
            //　自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
        }
        //　ターゲットのリセット
        currentTarget = null;
        currentSkill = null;
        currentItem = null;
        targetCharacterBattleScript = null;
        targetCharacterStatus = null;
        battleState = BattleState.Idle;
        //　自身の選択が終了したら次のキャラクターにする
        battleManager.ChangeNextChara();
        isDoneAnimation = false;
    }

    public CharacterStatus GetCharacterStatus()
    {
        return characterStatus;
    }

    public void SetHp(int hp)
    {
        this.hp = Mathf.Max(0, Mathf.Min(characterStatus.GetMaxHp(), hp));

        if (this.hp <= 0)
        {
            Dead();
        }
    }

    public int GetHp()
    {
        return hp;
    }

    public void SetMp(int mp)
    {
        this.mp = Mathf.Max(0, Mathf.Min(characterStatus.GetMaxMp(), mp));
    }

    public int GetMp()
    {
        return mp;
    }

    public bool IsDoneAnimation()
    {
        return isDoneAnimation;
    }

    public int GetAuxiliaryAgility()
    {
        return auxiliaryAgility;
    }

    public int GetAuxiliaryPower()
    {
        return auxiliaryPower;
    }

    public int GetAuxiliaryStrikingStrength()
    {
        return auxiliaryStrikingStrength;
    }

    //　補正の素早さを設定
    public void SetAuxiliaryAgility(int value)
    {
        auxiliaryAgility = value;
    }

    //　補正の力を設定
    public void SetAuxiliaryPower(int value)
    {
        auxiliaryPower = value;
    }

    //　補正の打たれ強さを設定
    public void SetAuxiliaryStrikingStrength(int value)
    {
        auxiliaryStrikingStrength = value;
    }

    public bool IsNumbness()
    {
        return isNumbness;
    }

    public bool IsPoison()
    {
        return isPoison;
    }

    public void SetNumbness(bool isNumbness)
    {
        this.isNumbness = isNumbness;
    }

    public void SetPoison(bool isPoison)
    {
        this.isPoison = isPoison;
    }

    public bool IsIncreasePower()
    {
        return isIncreasePower;
    }

    public void SetIsIncreasePower(bool isIncreasePower)
    {
        this.isIncreasePower = isIncreasePower;
    }

    public bool IsIncreaseStrikingStrength()
    {
        return isIncreaseStrikingStrength;
    }

    public void SetIsIncreaseStrikingStrength(bool isIncreaseStrikingStrength)
    {
        this.isIncreaseStrikingStrength = isIncreaseStrikingStrength;
    }

    public void SetBattleState(BattleState state)
    {
        this.battleState = state;
    }

    public void SetIsDoneAnimation()
    {
        isDoneAnimation = true;
    }

    //　選択肢から選んだモードを実行
    public void ChooseAttackOptions(BattleState selectOption, GameObject target, Skill skill = null, Item item = null)
    {

        //　スキルやターゲットの情報をセット
        currentTarget = target;
        currentSkill = skill;
        targetCharacterBattleScript = target.GetComponent<CharacterBattleScript>();
        targetCharacterStatus = targetCharacterBattleScript.GetCharacterStatus();

        //　選択したキャラクターの状態を設定
        battleState = selectOption;

        if (selectOption == BattleState.DirectAttack)
        {
            //animator.SetTrigger("DirectAttack");
        }
        else if (selectOption == BattleState.MagicAttack
          || selectOption == BattleState.Healing
          || selectOption == BattleState.IncreaseAttackPowerMagic
          || selectOption == BattleState.IncreaseDefencePowerMagic
          || selectOption == BattleState.NumbnessRecoveryMagic
          || selectOption == BattleState.PoisonnouRecoveryMagic
          )
        {
            //animator.SetTrigger("MagicAttack");
            //　魔法使用者のMPを減らす
            SetMp(GetMp() - ((Magic)skill).GetAmountToUseMagicPoints());
            //　使用者が味方キャラクターであればStatusPanelの更新
            if (GetCharacterStatus() as AllyStatus != null)
            {
                battleStatusScript.UpdateStatus(GetCharacterStatus(), BattleStatusScript.Status.MP, GetMp());
            }
            //Instantiate(((Magic)skill).GetSkillUserEffect(), transform.position, ((Magic)skill).GetSkillUserEffect().transform.rotation);
        }
        else if (selectOption == BattleState.UseHPRecoveryItem
          || selectOption == BattleState.UseMPRecoveryItem
          || selectOption == BattleState.UseNumbnessRecoveryItem
          || selectOption == BattleState.UsePoisonRecoveryItem
          )
        {
            currentItem = item;
            //animator.SetTrigger("UseItem");
        }
    }

    //　ターゲットエフェクトの表示
    public void ShowEffectOnTheTarget()
    {
        Instantiate<GameObject>(currentSkill.GetSkillReceivingSideEffect(), currentTarget.transform.position, currentSkill.GetSkillReceivingSideEffect().transform.rotation);
    }

    public void DirectAttack()
    {
        /*
        //アニメーション
        var targetAnimator = currentTarget.GetComponent<Animator>();
        targetAnimator.SetTrigger("Damage");
        */
        var damage = 0;

        //　攻撃相手のStatus
        if (targetCharacterStatus as AllyStatus != null)
        {
            var castedTargetStatus = (AllyStatus)targetCharacterBattleScript.GetCharacterStatus();
            //　攻撃相手の通常の防御力＋相手のキャラの補助値
            var targetDefencePower = castedTargetStatus.GetStrikingStrength() + (castedTargetStatus.GetEquipArmor()?.GetAmount() ?? 0) + targetCharacterBattleScript.GetAuxiliaryStrikingStrength();
            damage = Mathf.Max(0, (int)(((characterStatus.GetPower() + auxiliaryPower) - targetDefencePower)* guardPercentage));
            //　相手のステータスのHPをセット
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() - damage);
            //　ステータスUIを更新
            battleStatusScript.UpdateStatus(targetCharacterStatus, BattleStatusScript.Status.HP, targetCharacterBattleScript.GetHp());
        }
        else if (targetCharacterStatus as EnemyStatus != null)
        {
            var castedTargetStatus = (EnemyStatus)targetCharacterBattleScript.GetCharacterStatus();
            //　攻撃相手の通常の防御力＋相手のキャラの補助値
            var targetDefencePower = castedTargetStatus.GetStrikingStrength() + targetCharacterBattleScript.GetAuxiliaryStrikingStrength();
            damage = Mathf.Max(0, (characterStatus.GetPower() + (((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) + auxiliaryPower) - targetDefencePower);
            //　敵のステータスのHPをセット
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() - damage);
        }
        else
        {
            Debug.LogError("直接攻撃でターゲットが設定されていない");
        }

        battleManager.ShowMessage(gameObject.name + "は" + currentTarget.name + "に" + currentSkill.GetKanjiName() + "をして" + damage + "を与えた。");
    }

    public void MagicAttack()
    {
        /*
        //アニメーション
        var targetAnimator = currentTarget.GetComponent<Animator>();
        targetAnimator.SetTrigger("Damage");
        */
        var damage = 0;

        //　攻撃相手のStatus
        if (targetCharacterStatus as AllyStatus != null)
        {
            var castedTargetStatus = (AllyStatus)targetCharacterBattleScript.GetCharacterStatus();
            var targetDefencePower = castedTargetStatus.GetStrikingStrength() + (castedTargetStatus.GetEquipArmor()?.GetAmount() ?? 0);
            damage = Mathf.Max(0, ((Magic)currentSkill).GetMagicPower() - targetDefencePower);
            ////　相手のステータスのHPをセット
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() - damage);
            //　ステータスUIを更新
            battleStatusScript.UpdateStatus(targetCharacterStatus, BattleStatusScript.Status.HP, targetCharacterBattleScript.GetHp());
        }
        else if (targetCharacterStatus as EnemyStatus != null)
        {
            var castedTargetStatus = (EnemyStatus)targetCharacterBattleScript.GetCharacterStatus();
            var targetDefencePower = castedTargetStatus.GetStrikingStrength();
            damage = Mathf.Max(0, ((Magic)currentSkill).GetMagicPower() - targetDefencePower);
            //　相手のステータスのHPをセット
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() - damage);
        }
        else
        {
            Debug.LogError("魔法攻撃でターゲットが設定されていない");
        }

        battleManager.ShowMessage(gameObject.name + "は" + currentTarget.name + "に" + currentSkill.GetKanjiName() + "をして" + damage + "を与えた。");

    }

    public void UseMagic()
    {
        //　アニメーション状態を作ってなかったのでDamageにする
        //currentTarget.GetComponent<Animator>().SetTrigger("Damage");

        var magicType = ((Magic)currentSkill).GetSkillType();
        if (magicType == Skill.Type.RecoveryMagic)
        {
            /*
            if (GetHp() == GetCharacterStatus().GetMaxHp())
            {
                Debug.Log(currentTarget.name + "は全快です。");
                battleManager.ShowMessage(currentTarget.name + "は全快です。");
                return;
            }
            */
            var recoveryPoint = ((Magic)currentSkill).GetMagicPower() + characterStatus.GetMagicPower();
            if (targetCharacterStatus as AllyStatus != null)
            {
                targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() + recoveryPoint);
                battleStatusScript.UpdateStatus(targetCharacterStatus, BattleStatusScript.Status.HP, targetCharacterBattleScript.GetHp());
            }
            else
            {
                targetCharacterBattleScript.SetHp(GetHp() + recoveryPoint);
            }
            battleManager.ShowMessage(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "を" + recoveryPoint + "回復した。");
        }
        else if (magicType == Skill.Type.IncreaseAttackPowerMagic)
        {
            if (IsIncreasePower())
            {
                Debug.Log("既に攻撃力を上げています。");
                battleManager.ShowMessage("既に攻撃力を上げている。");
                return;
            }
            increasePowerPoint = ((Magic)currentSkill).GetMagicPower() + characterStatus.GetMagicPower();
            targetCharacterBattleScript.SetAuxiliaryPower(targetCharacterBattleScript.GetAuxiliaryPower() + increasePowerPoint);
            targetCharacterBattleScript.SetIsIncreasePower(true);
            battleManager.ShowMessage(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の力を" + increasePowerPoint + "増やした。");
        }
        else if (magicType == Skill.Type.IncreaseDefencePowerMagic)
        {
            if (IsIncreaseStrikingStrength())
            {
                Debug.Log("既に防御力を上げています。");
                battleManager.ShowMessage("既に防御力を上げています。");
                return;
            }
            increaseStrikingStrengthPoint = ((Magic)currentSkill).GetMagicPower() + characterStatus.GetMagicPower();
            targetCharacterBattleScript.SetAuxiliaryStrikingStrength(targetCharacterBattleScript.GetAuxiliaryStrikingStrength() + increaseStrikingStrengthPoint);
            targetCharacterBattleScript.SetIsIncreaseStrikingStrength(true);
            battleManager.ShowMessage(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の打たれ強さを" + increaseStrikingStrengthPoint + "増やした。");
        }
        else if (magicType == Skill.Type.NumbnessRecoveryMagic)
        {
            if (!IsNumbness())
            {
                Debug.Log(currentTarget.name + "は痺れ状態ではなかった。");
                battleManager.ShowMessage(currentTarget.name + "は痺れ状態ではなかった。");
                return;
            }
            targetCharacterStatus.SetNumbness(false);
            battleManager.ShowMessage(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の痺れを消した");
        }
        else if (magicType == Skill.Type.PoisonnouRecoveryMagic)
        {
            if (!IsPoison())
            {
                Debug.Log(currentTarget.name + "は毒状態ではなかった。");
                battleManager.ShowMessage(currentTarget.name + "は毒状態ではなかった。");
                return;
            }
            targetCharacterStatus.SetPoisonState(false);
            battleManager.ShowMessage(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の毒を消した");
        }
    }

    public void UseItem()
    {
        //currentTarget.GetComponent<Animator>().SetTrigger("Damage");

        //　バーティののアイテム数を減らす
        partyStatus.SetItemNum(currentItem, partyStatus.GetItemNum(currentItem) - 1);

        if (currentItem.GetItemType() == Item.Type.HPRecovery)
        {
            //　回復力
            var recoveryPoint = currentItem.GetAmount();
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() + recoveryPoint);
            battleStatusScript.UpdateStatus(targetCharacterStatus, BattleStatusScript.Status.HP, targetCharacterBattleScript.GetHp());
            battleManager.ShowMessage(gameObject.name + "は" + currentItem.GetKanjiName() + "を使って" + currentTarget.name + "のHPを" + recoveryPoint + "回復した。");
        }
        else if (currentItem.GetItemType() == Item.Type.MPRecovery)
        {
            //　回復力
            var recoveryPoint = currentItem.GetAmount();
            targetCharacterBattleScript.SetMp(targetCharacterBattleScript.GetMp() + recoveryPoint);
            battleStatusScript.UpdateStatus(targetCharacterStatus, BattleStatusScript.Status.MP, targetCharacterBattleScript.GetMp());
            battleManager.ShowMessage(gameObject.name + "は" + currentItem.GetKanjiName() + "を使って" + currentTarget.name + "のMPを" + recoveryPoint + "回復した。");
        }
        else if (currentItem.GetItemType() == Item.Type.NumbnessRecovery)
        {
            targetCharacterStatus.SetNumbness(false);
            battleManager.ShowMessage(gameObject.name + "は" + currentItem.GetKanjiName() + "を使って" + currentTarget.name + "の痺れを消した。");
        }
        else if (currentItem.GetItemType() == Item.Type.PoisonRecovery)
        {
            targetCharacterStatus.SetPoisonState(false);
            battleManager.ShowMessage(gameObject.name + "は" + currentItem.GetKanjiName() + "を使って" + currentTarget.name + "の毒を消した。");
        }

        //　アイテム数が0になったらItemDictionaryからそのアイテムを削除
        if (partyStatus.GetItemNum(currentItem) == 0)
        {
            partyStatus.GetItemDictionary().Remove(currentItem);
        }
    }

    //　防御
    public void Guard()
    {
        //　自分のターンが来たので上がったパラメータのチェック
        CheckIncreaseAttackPower();
        CheckIncreaseStrikingStrength();
        //animator.SetBool("Guard", true);
        guardPercentage = 0.5;
    }
    //　防御を解除
    public void UnlockGuard()
    {
        //animator.SetBool("Guard", false);
        guardPercentage = 1;
    }

    //　死んだときに実行する処理
    public void Dead()
    {
        //animator.SetTrigger("Dead");
        battleManager.DeleteAllCharacterInBattleList(this.gameObject);
        if (GetCharacterStatus() as AllyStatus != null)
        {
            battleStatusScript.UpdateStatus(GetCharacterStatus(), BattleStatusScript.Status.HP, GetHp());
            battleManager.DeleteAllyCharacterInBattleList(this.gameObject);
        }
        else if (GetCharacterStatus() as EnemyStatus != null)
        {
            battleManager.DeleteEnemyCharacterInBattleList(this.gameObject);
        }
        isDead = true;
    }

    public void CheckIncreaseAttackPower()
    {
        //　自分のターンが来た時に何らかの効果魔法を使ってたらターン数を増やす
        if (IsIncreasePower())
        {
            numOfTurnsSinceIncreasePower++;
            if (numOfTurnsSinceIncreasePower >= numOfTurnsIncreasePower)
            {
                numOfTurnsSinceIncreasePower = 0;
                SetAuxiliaryPower(GetAuxiliaryPower() - increasePowerPoint);
                SetIsIncreasePower(false);
                battleManager.ShowMessage(gameObject.name + "の攻撃力アップの効果が消えた");
            }
        }
    }

    public void CheckIncreaseStrikingStrength()
    {
        if (IsIncreaseStrikingStrength())
        {
            numOfTurnsSinceIncreaseStrikingStrength++;
            if (numOfTurnsSinceIncreaseStrikingStrength >= numOfTurnsIncreaseStrikingStrength)
            {
                numOfTurnsSinceIncreaseStrikingStrength = 0;
                SetAuxiliaryStrikingStrength(GetAuxiliaryStrikingStrength() - increaseStrikingStrengthPoint);
                SetIsIncreaseStrikingStrength(false);
                battleManager.ShowMessage(gameObject.name + "の防御力アップの効果が消えた");
            }
        }
    }
}
