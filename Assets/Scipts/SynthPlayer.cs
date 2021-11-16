using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthPlayer : MonoBehaviour
{

    public double frequency = 440.0;
    [Range(0, 3)]
    public int nType = 0;
    private double increment;
    private double phase;
    private double sampling_frequency = 48000.0;

    public float gain;
    public float volume = 0.1f;
    private double Oscillator(double dHertz, double dTime, int nType)
    {
        switch (nType)
        {
            case 0: //Sine wave
                return Mathf.Sin((float)(w(dHertz) * dTime));
            case 1: //Square wave
                return Mathf.Sin((float)(w(dHertz) * dTime)) > 0.0 ? 1.0 : -1.0;
            case 2: //Triangle wave
                return Mathf.Asin((float)(w(dHertz) * dTime));
            case 3: //Anolog Saw wave
                double dOutput = 0.0;

                for (double n = 1.0; n < 100.0; n++)
                {
                    dOutput += (Mathf.Sin((float)(n * w(dHertz) * phase)) / n);
                }
                return dOutput * (2 / Mathf.PI);
            default:
                return 0.0f;
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency;

        for(int i = 0; i < data.Length; i += channels)
        {
            phase += increment;

            data[i] = (float) (gain * Oscillator(frequency, phase, nType));

            if(channels == 2)
            {
                data[i + 1] = data[i];
            }

            if(phase > (Mathf.PI * 2))
            {
                phase = 0.0;
            }
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (OVRInput.Get(OVRInput.Button.One))
        {
            gain = volume;
        }
        else
        {
            gain = 0;
        }
        if (OVRInput.GetDown(OVRInput.Button.Two))
        { 
            frequency += frequency;
        }*/
    }
    private double w(double dHertz)
    {
        return dHertz * 2.0 * Mathf.PI;
    }
}