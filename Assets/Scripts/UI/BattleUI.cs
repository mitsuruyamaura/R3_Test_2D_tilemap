using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using System;
using UnityEngine.Events;

public class BattleUI : MonoBehaviour {
    [SerializeField] private CanvasGroup cgBattleUIPanel;         // バトルUIの親オブジェクト
    [SerializeField] private CanvasGroup cgBtns;                  // ボタン用
    [SerializeField] private Image imgPlayerChara, imgEnemyChara; // 画像
    [SerializeField] private TMP_Text txtPlayerCharaName, txtPlayerCharaHP, txtPlayerCharaPreviewDamage, txtPlayerCharaHit, txtPlayerCharaCrit;
    [SerializeField] private TMP_Text txtEnemyCharaName, txtEnemyCharaHP, txtEnemyCharaPreviewDamage, txtEnemyCharaHit, txtEnemyCharaCrit;
    [SerializeField] private Transform playerDamageTran, enemyDamageTarn;
    [SerializeField] private TMP_Text damageFloatViewPrefab;
    [SerializeField] private Button btnSubmit, btnCancel;
    private Subject<bool> onClickDecision = new();

    public Observable<bool> OnClickDecision => onClickDecision;
    //private bool? decision = null;

    public void Setup() {
        HideBattlePreview();

        // 決定
        btnSubmit.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1f))
            .Subscribe(_ => onClickDecision.OnNext(true))
            .AddTo(this);

        // キャンセル
        btnCancel.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1f))
            .Subscribe(_ => onClickDecision.OnNext(false))
            .AddTo(this);

        //btnSubmit.onClick.AddListener(() => decision = true);
        //btnCancel.onClick.AddListener(() => decision = false);

        //bool isConfirmed = decision.HasValue;

    }

    /// <summary>
    /// BattleSystemCoroutineBase 利用のためのボタンの登録
    /// </summary>
    /// <param name="onSubmit"></param>
    /// <param name="onCancel"></param>
    public void SetCallbacksForBattleSystemCoroutine(UnityAction onSubmit, UnityAction onCancel) {
        btnSubmit.onClick.AddListener(() => {
            onSubmit?.Invoke(); HideButtons();
        });

        btnCancel.onClick.AddListener(() => {
            onCancel?.Invoke(); HideButtons();
        });
    }


    private void ShowButtons() {
        cgBtns.alpha = 1f;
        cgBtns.blocksRaycasts = true;
    }

    private void HideButtons() {
        cgBtns.alpha = 0f;
        cgBtns.blocksRaycasts = false;
    }

    /// <summary>
    /// プレビュー表示
    /// </summary>
    /// <param name="attackerPreviewData"></param>
    /// <param name="targerPreviewData"></param>
    public void ShowBattlePreview(BattlePreviewData attackerPreviewData, BattlePreviewData targerPreviewData) {
        // Name
        txtPlayerCharaName.text = attackerPreviewData.attacker.charaData.name;
        txtEnemyCharaName.text = targerPreviewData.attacker.charaData.name;

        // HP
        txtPlayerCharaHP.text = $"HP : {attackerPreviewData.attacker.hp}";
        txtEnemyCharaHP.text = $"HP : {targerPreviewData.attacker.hp}";

        // ダメージ
        txtPlayerCharaPreviewDamage.text = $"DMG : {attackerPreviewData.damage}";
        txtEnemyCharaPreviewDamage.text = $"DMG : {targerPreviewData.damage}";

        // 命中率とクリティカル率
        txtPlayerCharaHit.text = $"HIT : {attackerPreviewData.hit} %";
        txtEnemyCharaHit.text = $"HIT : {targerPreviewData.hit} %";
        txtPlayerCharaCrit.text = $"CRIT : {attackerPreviewData.crit} %";
        txtEnemyCharaCrit.text = $"CRIT : {targerPreviewData.crit} %";

        // ボタン表示
        ShowButtons();

        // プレビュー表示
        cgBattleUIPanel.alpha = 1.0f;
        cgBattleUIPanel.blocksRaycasts = true;
    }

    public void HideBattlePreview() {
        cgBattleUIPanel.alpha = 0f;
        cgBattleUIPanel.blocksRaycasts = false;
    }

    /// <summary>
    /// ダメージのフロート表示
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="ownerType"></param>
    public void ShowDamageFloatToDefender(string damage, OwnerType ownerType) {
        if (damageFloatViewPrefab == null) {
            return;
        }

        TMP_Text damageFloatView = null;
        if (ownerType == OwnerType.Player) {
            damageFloatView = Instantiate(damageFloatViewPrefab, enemyDamageTarn, false);
        } else {
            damageFloatView = Instantiate(damageFloatViewPrefab, playerDamageTran, false);
        }
        damageFloatView.text = damage;

        Destroy(damageFloatView.gameObject, 1.0f);
    }

    /// <summary>
    /// HP 表示の更新
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="ownerType"></param>
    public void UpdateDisplayHp(int amount, OwnerType ownerType) {
        if (ownerType == OwnerType.Player) {
            txtEnemyCharaHP.text = amount.ToString();
        } else {
            txtPlayerCharaHP.text = amount.ToString();
        }
    }
}