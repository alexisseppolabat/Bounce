using UnityEngine;
using System;

public class Effect {
    private float initTime;
    private float maxDuration;
    private Action enactFnc;
    private Action desistFnc;


    public Effect(float maxDuration, Action enactFnc, Action desistFnc) {
        this.maxDuration = maxDuration;
        this.enactFnc = enactFnc;
        this.desistFnc = desistFnc;
    }

    public void startTime() {
        initTime = Time.time;
    }

    public float progress() {
        return (Time.time - initTime) / maxDuration;
    }

    public void enact() {
        enactFnc.Invoke();
    }

    public void desist() {
        desistFnc.Invoke();
    }
}