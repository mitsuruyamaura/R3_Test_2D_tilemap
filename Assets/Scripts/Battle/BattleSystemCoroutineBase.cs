using System.Collections;
using UnityEngine;

public class BattleSystemCoroutineBase : MonoBehaviour {
    private float interval = 1.5f;
    private int criticalRatio = 2;  // 外部クラスからセットする場合には、SetCriticalRatio メソッドを使う
    private int pursuitSpeed = 7;

    [SerializeField] private BattleUI battleUI;

    //[SerializeField] private SkillDataSO skillDataSO;  // あるいは DataBaseManager などから取得

    /// <summary>
    /// バトルの一連の流れをコルーチンを利用して処理する
    /// </summary>
    /// <param name="attackerPreviewData"></param>
    /// <param name="targerPreviewData"></param>
    /// <returns></returns>
    public IEnumerator ExecuteBattleCo(BattlePreviewData attackerPreviewData, BattlePreviewData targerPreviewData) {
        // フェーズ1：アタッカーが攻撃
        yield return StartCoroutine(DoAttackCo(attackerPreviewData.attacker, attackerPreviewData.defender,
            attackerPreviewData.attacker.GetBattleAnimator(), attackerPreviewData.defender.GetBattleAnimator()));

        // フェーズ2：反撃(条件：攻撃側と防御側が敵であり、かつ、防御側が生きていて、防御側の攻撃範囲にアタッカーが入っている)
        if (attackerPreviewData.attacker.ownerType != attackerPreviewData.defender.ownerType && attackerPreviewData.defender.IsAlive && IsInCounterRange(attackerPreviewData.defender, attackerPreviewData.attacker)) {
            yield return StartCoroutine(DoAttackCo(targerPreviewData.attacker, targerPreviewData.defender,
                targerPreviewData.attacker.GetBattleAnimator(), targerPreviewData.defender.GetBattleAnimator()));
        }

        // フェーズ3：追撃(アタッカーと防御側の双方が生存していて、かつアタッカーの速度が防御側よりも一定以上高い場合)
        if (attackerPreviewData.attacker.IsAlive && attackerPreviewData.defender.IsAlive && attackerPreviewData.attacker.charaData.speed - attackerPreviewData.defender.charaData.speed >= pursuitSpeed) {
            yield return StartCoroutine(DoAttackCo(attackerPreviewData.attacker, attackerPreviewData.defender,
            attackerPreviewData.attacker.GetBattleAnimator(), attackerPreviewData.defender.GetBattleAnimator()));
        }
    }

    /// <summary>
    /// 単体の攻撃処理（アニメーション → 命中判定 → ダメージ計算 → 演出）
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <param name="attackerAnim"></param>
    /// <param name="defenderAnim"></param>
    /// <returns></returns>
    private IEnumerator DoAttackCo(CharaUnit attacker, CharaUnit defender, BattleAnimator attackerAnim, BattleAnimator defenderAnim) {
        // 攻撃アニメーションの再生を行い、終了するまで待機
        if (attackerAnim != null) {
            yield return StartCoroutine(attackerAnim.PlayAttackAnimationCoroutine());
        }

        // 命中判定（ランダムで決定。命中率に運の差を考慮）
        int hitChance = 100 + attacker.charaData.luck - defender.charaData.luck;
        if (Random.Range(0, 100) >= hitChance) {
            Debug.Log($"{attacker.name} の攻撃は外れた");
            
            // ミス表示
            battleUI.ShowDamageFloatToDefender("Miss!", attacker.ownerType);
            
            // 表示待機
            yield return new WaitForSeconds(interval);

            // 攻撃処理を中断して戻る
            yield break;
        }

        // クリティカル判定（確率で決まる、今回は運の半分）
        bool isCritical = Random.Range(0, 100) < (attacker.charaData.luck / 2);

        // ダメージ計算（攻撃力 - 防御力。最低値は0）
        int damage = Mathf.Max(0, attacker.charaData.attack - defender.charaData.defense);

        // 参照渡しを使い、メソッド内の damage を上の damage に反映させたい場合
        //ApplySkills(ref damage, attacker, defender);

        // 戻り値でもらって更新する場合
        // スキルによるダメージ変化を反映
        //damage = ApplySkills(damage, attacker, defender);

        // クリティカルヒットならダメージ criticalRatio 倍
        if (isCritical) {
            damage *= criticalRatio;
            Debug.Log("クリティカルヒット！！");
        }

        // HPを減らす（0未満にはならないようにする）
        defender.CalcHp(-damage);

        Debug.Log($"{attacker.name} → {defender.name} に {damage} ダメージ（残HP: {defender.hp}）");

        // 防御側の UI にダメージ表示
        battleUI.ShowDamageFloatToDefender(damage.ToString(), attacker.ownerType);
        battleUI.UpdateDisplayHp(defender.hp, attacker.ownerType);

        // TODO SE 

        // TODO 防御側用に攻撃のエフェクトデータ取得
        //EffectData effectData = DataBaseManager.instance.effectDataSO.GetEffectData(attacker.selectSkillId);

        // データがあるなら
        //if (effectData != null) {
        //    // エフェクト
        //    EffectManager.instance.PlayHitEffect(effectData, GridHelper.GridToWorld(defender.gridPosition));
        //}

        if (defenderAnim != null) {
            // ダメージアニメーションの再生を行い、終了するまで待機
            yield return StartCoroutine(defenderAnim.PlayDamageAnimationCoroutine());
        }

        // 表示待機
        yield return new WaitForSeconds(interval);
    }

    /// <summary>
    /// 反撃対象が攻撃範囲内にいるかどうかをチェックする
    /// </summary>
    /// <param name="counterAttacker"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsInCounterRange(CharaUnit counterAttacker, CharaUnit target) {
        // マンハッタン距離（縦+横）で距離を測る
        Vector3Int attackerPos = TilemapController.instance.GetGridPositionFromUnit(counterAttacker);
        Vector3Int targetPos = TilemapController.instance.GetGridPositionFromUnit(target);

        int distance = Mathf.Abs(attackerPos.x - targetPos.x)
                     + Mathf.Abs(attackerPos.y - targetPos.y);
        return distance <= counterAttacker.charaData.range;
    }

    /// <summary>
    /// SkillData は使わず、スキルによってダメージを変化させる（参照渡し版：現在は未使用）
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    private void ApplySkills(ref int damage, CharaData attacker, CharaData target) {
        foreach (var skill in attacker.skillList) {
            switch (skill) {
                case SkillType.PowerAttack:
                    damage += 5;
                    Debug.Log("スキル：パワーアタック発動！");
                    break;

                case SkillType.Magic:
                    damage += 10;
                    Debug.Log("スキル：魔法攻撃発動！");
                    break;

                case SkillType.Heal:
                    attacker.hp += 10;
                    Debug.Log("スキル：自己回復発動！");
                    break;
            }
        }
    }

    /// <summary>
    /// SkillData は使わず、スキルによってダメージを変化させる（戻り値で受け取る版）
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private int ApplySkills(int baseDamage, CharaData attacker, CharaData target) {
        int modifiedDamage = baseDamage;

        foreach (var skill in attacker.skillList) {
            switch (skill) {
                case SkillType.PowerAttack:
                    modifiedDamage += 5;
                    Debug.Log("スキル：パワーアタック発動！");
                    break;

                case SkillType.Magic:
                    modifiedDamage += 10;
                    Debug.Log("スキル：魔法攻撃発動！");
                    break;

                case SkillType.Heal:
                    attacker.hp += 10;
                    Debug.Log("スキル：自己回復発動！");
                    break;
            }
        }

        return modifiedDamage;
    }

    /// <summary>
    /// SkillData を利用する場合
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private int ApplySkillsFromSkillData(int baseDamage, CharaData attacker, CharaData target) {
        int modifiedDamage = baseDamage;

        // CharaData が保持しているスキルIDリストを順番に処理
        foreach (int skillId in attacker.skillIdList) {
            SkillData skill = DataBaseManager.instance.GetSkillData(skillId);

            // 該当IDのスキルが存在しない場合はスキップ
            if (skill == null) {
                Debug.Log($"SkillID {skillId} に対応するデータが見つかりませんでした");
                continue;
            }

            // スキルのタイプに応じて処理を分岐
            switch (skill.skillType) {
                case SkillType.PowerAttack:
                case SkillType.Magic:
                    modifiedDamage += skill.value;
                    Debug.Log($"スキル：{skill.skillName} 発動！（+{skill.value} ダメージ）");
                    break;

                case SkillType.Heal:
                    target.hp += skill.value;
                    Debug.Log($"スキル：{skill.skillName} 発動！（HP +{skill.value} 回復）");
                    break;

                    // 他のスキルタイプが今後増えてもここに追加すればOK
            }

            // 今後エフェクトやSEなどの拡張もここに書ける（skill.effectId / skill.seId など）
        }

        return modifiedDamage;
    }

    /// <summary>
    /// クリティカル時の倍率の設定
    /// </summary>
    /// <param name="newRatio"></param>
    public void SetCriticalRatio(int newRatio) {
        criticalRatio = newRatio;
    }
}