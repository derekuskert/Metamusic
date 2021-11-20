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

    private bool flip;
    
    public float gain;
    public float volume = 0.1f;

    public struct EnvelopeADSR
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

    public EnvelopeADSR _envelope;

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
        if (_amp != 0)
        {
            flip = true;
            _passthroughLayer.edgeColor = Color.HSVToRGB(Mathf.InverseLerp(0, 15000, (float)frequency), 1.0f, (float)_amp);
        }else if (flip)
        {
            flip = false;
            _passthroughLayer.edgeColor = Color.HSVToRGB(Mathf.InverseLerp(0, 15000, (float)frequency), 1.0f, 0);
        }

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

    public void SetADSR(String type, String value)
    {
        switch (type)
        {
            case "attack":
                switch (value)
                {
                    case "Low": _envelope.AttackTime = 0.1; break;
                    case "Medium": _envelope.AttackTime = 0.3; break;
                    case "High": _envelope.AttackTime = 1; break;
                }
                break;
            case "decay":
                switch (value)
                {
                    case "Low": _envelope.DelayTime = 0; break;
                    case "Medium": _envelope.DelayTime = 0.05; break;
                    case "High": _envelope.DelayTime = 0.7; break;
                }
                break;
            case "sustain":
                switch (value)
                {
                    case "Low": _envelope.SustainAmplitude = 0.1; break;
                    case "Medium": _envelope.SustainAmplitude = 0.5; break;
                    case "High": _envelope.SustainAmplitude = 1.0; break;
                }
                break;
            case "release":
                switch (value)
                {
                    case "Low": _envelope.ReleaseTime = 0.1; break;
                    case "Medium": _envelope.ReleaseTime = 0.5; break;
                    case "High": _envelope.ReleaseTime = 3; break;
                }
                break;
            case "start amplitude":
                switch (value)
                {
                    case "Low": _envelope.StartAmplitude = 0.13; break;
                    case "Medium": _envelope.StartAmplitude = 0.7; break;
                    case "High": _envelope.StartAmplitude = 1.2; break;
                }
                break;
            default:
                break;
        }
    }
    private double Oscillator(double dHertz, double  time, int OscillatorType = 0, double modulationHertz = 0, double modulationAmplitude = 0)
    {
        double freq = w(dHertz) + modulationAmplitude * dHertz * Mathf.Sin((float)(w(modulationHertz) * time));
        switch (OscillatorType)
        {
            case 0: //Sine wave
                return Mathf.Sin((float)(w(freq)));
            case 1: //Square wave
                return Mathf.Sin((float)(w(freq))) > 0.0f ? 1.0f : -1.0f;
            case 2: //Triangle wave
                return Mathf.Asin((float)(w(freq)));
            case 3: //Anolog Saw wave
                double dOutput = 0.0;

                for (double n = 1.0; n < 100.0; n++)
                {
                    dOutput += (Mathf.Sin((float)(n * w(freq) * _phase)) / n);
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
            
            data[i] = /*(float)(gain* Mathf.Sin((float)phase));*/(float)_amp * 0.1f * (float)Oscillator(_phase, AudioSettings.dspTime, 0, 1, 1);

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