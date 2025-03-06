using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;
using R3.Triggers;
using R3.Collections;
using System;
using Cysharp.Threading.Tasks;

public class Sample1 : MonoBehaviour
{
    public int max = 10;
    public int value;
    public int subvalue;

    private CompositeDisposable disposables;
    private IDisposable disposable;
    private Subject<int> subject = new();

    public ReactiveProperty<int> Money = new(0);

    void Start() {
        disposables = new CompositeDisposable();

        disposable = Observable.Timer(System.TimeSpan.FromSeconds(2.0f))
            .Subscribe(_ => Debug.Log("1"));

        Observable.Interval(System.TimeSpan.FromSeconds(1.0f))
            .TakeWhile(_ => max >= value)  // max ‚É‚È‚é‚Ü‚Åw“Ç‚µAŽ©“®’âŽ~
            .Subscribe(_ => value++);
            //.AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => max < value)
            .Take(1)   // 1‰ñ‚¾‚¯‚É‚µ‚½‚¢‚È‚çATakeUntil ‚Å‚È‚­‚Ä‚à‚æ‚¢BŽ©“®’âŽ~
            //.TakeUntil(_ => max < value)
            .Subscribe(_ => Debug.Log("3"));

        Observable.Interval(System.TimeSpan.FromSeconds(1.5f))
            .Subscribe(_ => Money.OnNext(Money.Value))
            //.Subscribe(_ => Money.SetValueForceNotify())   // SetValueAndForceNotify ‚ª‚È‚­‚È‚Á‚Ä‚µ‚Ü‚Á‚½‚Ì‚ÅŠg’£ƒƒ\ƒbƒh‚ðì‚Á‚Ä‘ã—p
            .AddTo(disposables);

        Observable.Interval(System.TimeSpan.FromSeconds(0.2f))
            .Subscribe(_ => Money.Value++)
            .AddTo(disposables);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => subject.OnNext(10));

        subject.Subscribe(x => subvalue += x)
            .AddTo(disposables);
    }

    private void OnDestroy() {
        disposable?.Dispose();
        disposables?.Clear();
    }
}