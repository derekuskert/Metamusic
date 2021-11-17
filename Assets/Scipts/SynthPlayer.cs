using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthPlayer : MonoBehaviour
{
    OVRPassthroughLayer _passthroughLayer;
    public double frequency = 440.0;
    [Range(0, 3)]
    public int nType = 1;
    private double _increment;
    private double _phase;
    private double sampling_frequency = 48000.0;
    private double _amp;
    
    public float gain;
    public float volume = 0.1f;

    struct EnvelopeADSR
    {
        public double AttackTime;
        public double DelayTime;
        public double ReleaseTime;

        public double SustainAmplitude;
        public double StartAmplitude;

        public double TriggerOnTime;
        public double TriggerOffTime;

        public bool NoteOn;
        
    }

    private EnvelopeADSR _envelope;

    // Start is called before the first frame update
    void Start()
    {
        _passthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
        _passthroughLayer.edgeRenderingEnabled = true;

        _envelope.AttackTime = 0.01;
        _envelope.DelayTime = 0.001;
        _envelope.StartAmplitude = 1.2;
        _envelope.SustainAmplitude = 0.9;
        _envelope.ReleaseTime = 0.5;
        _envelope.TriggerOnTime = 0;
        _envelope.TriggerOffTime = 0;
        _envelope.NoteOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        _passthroughLayer.edgeColor = Color.HSVToRGB(Mathf.InverseLerp(0, 15000, (float)frequency), 1.0f, gain * 10);
        _amp = GetAmplitude(Time.time);
    }
    private double w(double dHertz)
    {
        return dHertz * 2.0 * Mathf.PI;
    }

    private double GetAmplitude(double time)
    {
        double amplitude = 0;
        double lifeTime = time - _envelope.TriggerOnTime;
        
        if (_envelope.NoteOn)
        {
            //Attack
            if (lifeTime <= _envelope.AttackTime)
                amplitude = (lifeTime / _envelope.AttackTime) * _envelope.StartAmplitude;
            //Decay
            if (lifeTime > _envelope.AttackTime && lifeTime < (_envelope.AttackTime + _envelope.DelayTime))
                amplitude = ((lifeTime - _envelope.AttackTime) / _envelope.DelayTime) * (_envelope.SustainAmplitude - _envelope.StartAmplitude) + _envelope.StartAmplitude;
            
            //Sustain
            if (lifeTime > _envelope.AttackTime + _envelope.DelayTime)
            {
                amplitude = _envelope.SustainAmplitude;
            }

        }
        else
        {
            //Release
            amplitude = ((time - _envelope.TriggerOffTime) / _envelope.ReleaseTime) * (0 - _envelope.SustainAmplitude) +
                        _envelope.SustainAmplitude;
        }

        if (amplitude <= 0.0001)
        {
            return 0;
        }

        return amplitude;
    }

    public void NoteOn(double timeOn)
    {
        _envelope.NoteOn = true;
        _envelope.TriggerOnTime = timeOn;
    }
    
    public void NoteOff(double timeOff)
    {
        _envelope.NoteOn = false;
        _envelope.TriggerOffTime = timeOff;
    }
    private double Oscillator(double dHertz, int OscillatorType = 0)
    {
        switch (OscillatorType)
        {
            case 0: //Sine wave
                return Mathf.Sin((float)(w(dHertz)));
            case 1: //Square wave
                return Mathf.Sin((float)(w(dHertz))) > 0.0f ? 1.0f : -1.0f;
            case 2: //Triangle wave
                return Mathf.Asin((float)(w(dHertz)));
            case 3: //Anolog Saw wave
                double dOutput = 0.0;

                for (double n = 1.0; n < 100.0; n++)
                {
                    dOutput += (Mathf.Sin((float)(n * w(dHertz) * _phase)) / n);
                }
                return dOutput * (2 / Mathf.PI);
            default:
                return 0.0f;
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        _increment = frequency * 2.0 * Mathf.PI / sampling_frequency;


        for(int i = 0; i < data.Length; i += channels)
        {
            _phase += _increment;

            data[i] = /*(float)(gain* Mathf.Sin((float)phase));*/(float)_amp * 0.1f * (float)Oscillator(_phase, nType);

            if(channels == 2)
            {
                data[i + 1] = data[i];
            }

            if(_phase > (Mathf.PI * 2))
            {
                _phase = 0.0;
            }
        }
    }

    private void OnDestroy()
    {
        _passthroughLayer.edgeColor = Color.black;
        throw new NotImplementedException();
    }
}