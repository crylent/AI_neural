using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AI_neural.Algorithm;

public class HammingNetwork: NeutralNetwork
{
    private readonly float[][] _weights;
    
    private readonly float _shift;
    private readonly float _epsilon;
    private const float K1 = 1f / ImageSize;
    private const float Un = 1f / K1;

    private const int MaxIterations = 1000;
    
    public HammingNetwork(IReadOnlyList<BitImage8> samples)
    {
        _weights = new float[ImageSize][];
        for (var i = 0; i < ImageSize; i++)
        {
            _weights[i] = new float[samples.Count];
        }
        for (var x = 0; x < samples.Count; x++)
        {
            var sample = samples[x];
            var y = 0;
            foreach (var point in sample.GetIterator())
            {
                _weights[y][x] = point / 2f;
                y++;
            }
        }

        _shift = ImageSize / 2f;
        _epsilon = 1f / samples.Count;
    }

    public override int FindBestMatch(BitImage8 img)
    {
        var u = new float[_weights[0].Length];
        for (var i = 0; i < _weights[0].Length; i++)
        {
            var z = _shift;
            for (var j = 0; j < ImageSize; j++)
            {
                z += _weights[j][i] * img.GetPoint(j);
            }
            
            u[i] = z switch
            {
                < 0 => 0,
                > Un => Un * K1,
                _ => z * K1
            };
        }
        Debug.Print($"{string.Join(" ", u)}");
        
        int bestMatch, iterations = 0;
        while (MaxnetStep(ref u, out bestMatch) && iterations < MaxIterations)
        {
            iterations++;
        }

        return bestMatch;
    }

    private bool MaxnetStep(ref float[] u, out int lastNotZero)
    {
        var sum = u.Sum();
        var zeros = 0;
        lastNotZero = 0;
        for (var i = 0; i < u.Length; i++)
        {
            var a = u[i] - _epsilon * (sum - u[i]);
            if (a > 0)
            {
                u[i] = a;
                lastNotZero = i;
            }
            else
            {
                u[i] = 0;
                zeros++;
            }
        }
        Debug.Print($"{string.Join(" ", u)}");
        return zeros < u.Length - 1;
    }
}