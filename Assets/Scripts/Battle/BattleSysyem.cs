using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class BattleSysyem {
    private BattleUI battleUI;
    private int interval = 1500;
    private int criticalRatio = 2;
    private int pursuitSpeed = 7;

    public BattleSysyem(BattleUI battleUI) {
        this.battleUI = battleUI;
    }

    public async UniTask ExecuteBattleAsync(BattlePreviewData attackerPreviewData, BattlePreviewData targerPreviewData, CancellationToken token) {
        // フェーズ1：アタッカーの攻撃
        await DoAttack(attackerPreviewData.attacker, attackerPreviewData.defender,
            attackerPreviewData.attacker.GetBattleAnimator(), attackerPreviewData.defender.GetBattleAnimator(), token);

        // フェーズ2：反撃(条件：攻撃側と防御側が敵であり、かつ、防御側が生きていて、防御側の攻撃範囲にアタッカーが入っている)
        if (attackerPreviewData.attacker.ownerType != attackerPreviewData.defender.ownerType && attackerPreviewData.defender.IsAlive && IsInCounterRange(attackerPreviewData.defender, attackerPreviewData.attacker)) {
            await DoAttack(targerPreviewData.attacker, targerPreviewData.defender,
                targerPreviewData.attacker.GetBattleAnimator(), targerPreviewData.defender.GetBattleAnimator(), token);
        }

        // フェーズ3：追撃(アタッカーと防御側の双方が生存していて、かつアタッカーの速度が防御側よりも一定以上高い場合)
        if (attackerPreviewData.defender.IsAlive && attackerPreviewData.attacker.charaData.speed - attackerPreviewData.defender.charaData.speed >= pursuitSpeed) {
            await DoAttack(attackerPreviewData.attacker, attackerPreviewData.defender, 
                attackerPreviewData.attacker.GetBattleAnimator(),attackerPreviewData.defender.GetBattleAnimator(), token);
        }
    }

    private async UniTask DoAttack(CharaUnit attacker, CharaUnit defender, BattleAnimator attackerAnim, BattleAnimator defenderAnim, CancellationToken token) {
        if (attackerAnim != null) {
            await attackerAnim.PlayAttackAnimationAsync();
        }

        // 命中判定
        int hitChance = 100 + attacker.charaData.luck - defender.charaData.luck;
        if (Random.Range(0, 100) >= hitChance) {
            Debug.Log($"{attacker.name} の攻撃は外れた！");
            battleUI.ShowDamageFloatToDefender("Miss!", attacker.ownerType);

            // TODO SE エフェクト

            await UniTask.Delay(interval, cancellationToken: token);
            return;
        }

        // クリティカル判定
        bool isCritical = Random.Range(0, 100) < (attacker.charaData.luck / 2);

        // ダメージ計算
        int damage = Mathf.Max(0, attacker.charaData.attack - defender.charaData.defense);

        // スキル適用
        //foreach (var skill in attacker.skillEffectList) {
        //    skill.Apply(ref damage, attacker, defender);
        //}

        // クリティカルヒットならダメージ criticalRatio 倍
        if (isCritical) {
            damage *= criticalRatio;
            Debug.Log($"クリティカルヒット！！");
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
            await defenderAnim.PlayDamageAnimationAsync();
        }

        await UniTask.Delay(interval, cancellationToken: token);
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
}